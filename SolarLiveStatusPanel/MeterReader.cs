using NEnvoy;
using NEnvoy.Models;
using Refit;

namespace SolarLiveStatusPanel
{

    internal struct MeterReadings
    {
        public bool Configured;
        public DateTime Updated;
        public decimal Generating;
        public decimal Load;
        public decimal ExportImport;
        public bool IsExporting;
    }

    internal class MeterReader
    {
        const long METER_PRODUCTION = 704643328;
        const long METER_NET_CONSUMPTION = 704643584;

        private ILogger Logger;
        private IEnvoyClient? Client = null;
        private EnvoyConnectionInfo? ConnectionConfig = null;

        public MeterReadings LatestReadings = new MeterReadings
            {
                Configured = false,
                Updated = DateTime.MinValue,
                Generating = 0,
                Load = 0,
                ExportImport = 0,
                IsExporting = false,
            };

        public MeterReader(ILogger logger)
        {
            this.Logger = logger;
        }

        public async Task InitAsync(EnvoyConnectionInfo? connectionConfig)
        {
            Logger.Info(() => "MeterReader.InitAsync");
            if (connectionConfig != null)
            {
                ConnectionConfig = connectionConfig;
                try
                {
                    LatestReadings.Configured = true;
                    Client = await GetClientAsync(connectionConfig, "token.txt");
                    Logger.Info(() => $"MeterReader.InitAsync, got client = {Client != null}");
                    await GetReadingsAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating client {ex.Message}");
                    Logger.LogException(() => "MeterReader.InitAsync", ex);
                    LatestReadings.Configured = false;
                }
            }
        }

        public async Task GetReadingsAsync()
        {
            try
            {
                if (Client == null)
                {
                    return;
                }

                var meterreadings = await Client.GetMeterReadingsAsync().ConfigureAwait(false);
                var generaingMeter = meterreadings.Where(mr => mr.EId == METER_PRODUCTION).First();
                if (generaingMeter != null)
                {
                    LatestReadings.Updated = DateTime.Now;
                    LatestReadings.Generating = generaingMeter.ActivePower;
                }

                var netConsumptionMeter = meterreadings.Where(mr => mr.EId == METER_NET_CONSUMPTION).First();
                if (generaingMeter != null && netConsumptionMeter != null)
                {
                    var currentLoad = generaingMeter.ActivePower + netConsumptionMeter.ActivePower;
                    LatestReadings.Load = currentLoad;
                    if (netConsumptionMeter.ActivePower < 0)
                    {
                        LatestReadings.ExportImport = -netConsumptionMeter.ActivePower;
                        LatestReadings.IsExporting = true;
                    }
                    else if (netConsumptionMeter.ActivePower > 0)
                    {
                        LatestReadings.ExportImport = netConsumptionMeter.ActivePower;
                        LatestReadings.IsExporting = false;
                    }
                    else
                    {
                        LatestReadings.ExportImport = 0;
                        LatestReadings.IsExporting = false;
                    }
                }
            }
            catch (ApiException apiEx)
            {
                Logger.LogException(() => "MeterReader.GetReadingsAsync", apiEx);
                System.Net.HttpStatusCode statusCode = apiEx.StatusCode;
                if (statusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // hmmmm things have gone badly wrong we need to try and restart the connection
                    await InitAsync(ConnectionConfig);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(() => "MeterReader.GetReadingsAsync", ex);
            }
        }

        private async Task<IEnvoyClient> GetClientAsync(EnvoyConnectionInfo envoyConnectionInfo, string tokenfile, CancellationToken cancellationToken = default)
        {
            // If we don't have a session token, we create a client by logging in, else we create one from the session token
            if (!File.Exists(tokenfile))
            {
                Logger.Info(() => $"MeterReader.GetClientAsync, getting new token");
                var client = await EnvoyClient.FromLoginAsync(envoyConnectionInfo, cancellationToken).ConfigureAwait(false);
                var token = client.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    throw new InvalidOperationException("cannot generate token, check credentials");
                }
                await File.WriteAllTextAsync(tokenfile, token, cancellationToken).ConfigureAwait(false);
                return client;
            }
            return EnvoyClient.FromToken(await File.ReadAllTextAsync(tokenfile, cancellationToken).ConfigureAwait(false), envoyConnectionInfo);
        }
    }
}
