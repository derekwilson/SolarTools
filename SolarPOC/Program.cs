// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using NEnvoy;
using NEnvoy.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;


namespace SolarPOC
{
    internal class Program
    {
        const string ENVOY_IP = "192.168.0.225";
        const string ENVOY_USER = "mail@stephanieslater.com";
        const string ENVOY_PASSWORD = "c0st!e";

        const long METER_PRODUCTION = 704643328;
        const long METER_NET_CONSUMPTION = 704643584;

        static void OutputToConsole(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }

        static void OutputToLogger(string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        static private string GetCodeVersion()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            return name.Version.ToString();
        }

        static private void DisplayBanner()
        {
            OutputToConsole($"SolarPOC v{GetCodeVersion()}");
        }

        static private void DisplayEnvironment()
        {
            OutputToConsole($"Running on .NET CLR: {Environment.Version.ToString()}");
        }

        static private EnphaseAppSettings GetSettings()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("solarSettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var settings = new EnphaseAppSettings();
            configuration.GetSection("enphase").Bind(settings);

            return settings;
        }

        static private EnvoyConnectionInfo ValidateConfig(EnphaseAppSettings? appSettings)
        {
            if (appSettings == null || appSettings.Credentials == null || appSettings.Gateway == null)
            {
                OutputToConsole($"settings json is not in the correct format");
                throw new Exception("settings json is not in the correct format");
            }
            if (string.IsNullOrEmpty(appSettings.Gateway.Address))
            {
                OutputToConsole($"missing gateway");
                throw new ArgumentException("missing config", "Gateway.Address");
            }
            if (string.IsNullOrEmpty(appSettings.Credentials.User))
            {
                OutputToConsole($"missing user");
                throw new ArgumentException("missing config", "Credentials.User");
            }
            if (string.IsNullOrEmpty(appSettings.Credentials.Password))
            {
                OutputToConsole($"missing password");
                throw new ArgumentException("missing config", "Credentials.Password");
            }

            return new EnvoyConnectionInfo
            {
                EnvoyHost = appSettings.Gateway.Address,
                Username = appSettings.Credentials.User,
                Password = appSettings.Credentials.Password,
            };
        }

        private static async Task Main(string[] args)
        {
            DisplayBanner();
            DisplayEnvironment();

            var appSettings = GetSettings();
            OutputToConsole($"Gateway Address: {appSettings.Gateway.Address}");
            OutputToConsole($"User: {appSettings.Credentials.User}");
            var envoyConfig = ValidateConfig(appSettings);

            var client = await GetClientAsync(envoyConfig, "token.txt");

            var deviceinfo = await client.GetEnvoyInfoAsync().ConfigureAwait(false);
            OutputToConsole($"SN: {deviceinfo.Device.Serial}");

            var production = await client.GetProductionAsync().ConfigureAwait(false);
            OutputToConsole($"Count: {production.Production.FirstOrDefault()?.ActiveCount}");

            var meters = await client.GetMetersAsync().ConfigureAwait(false);
            foreach (var meter in meters)
            {
                OutputToConsole($"ID: {meter.EId}, Type {meter.MeasurementType}");
            }

            var meterreadings = await client.GetMeterReadingsAsync().ConfigureAwait(false);
            foreach ( var meter in meterreadings )
            {
                OutputToConsole($"ID: {meter.EId}, Power: {meter.ActivePower}, Time: {meter.Time}");
            }

            var productionMeter = meterreadings.Where(mr => mr.EId == METER_PRODUCTION).First();
            if (productionMeter != null)
            {
                OutputToConsole($"Production: {FormatKw(productionMeter.ActivePower)}");
            }

            var netConsumptionMeter = meterreadings.Where(mr => mr.EId == METER_NET_CONSUMPTION).First();
            if (productionMeter != null && netConsumptionMeter != null)
            {
                var currentLoad = productionMeter.ActivePower + netConsumptionMeter.ActivePower;
                OutputToConsole($"Current Load {FormatKw(currentLoad)}");

                if (netConsumptionMeter.ActivePower < 0)
                {
                    OutputToConsole($"Export to grid: {FormatKw(-netConsumptionMeter.ActivePower)}");
                }
                else if (netConsumptionMeter.ActivePower > 0)
                {
                    OutputToConsole($"Import from grid: {FormatKw(netConsumptionMeter.ActivePower)}");
                }
                else
                {
                    OutputToConsole($"Idle");
                }
            }
        }

        private static string FormatKw(decimal watts)
        {
            if (Math.Abs(watts) > 1000)
            {
                return $"{(watts/1000).ToString("0.00")} KW";
            }
            if (watts != 0)
            {
                return $"{(watts).ToString("0.00")} W";
            }
            return "0";
        }

        private static async Task<IEnvoyClient> GetClientAsync(EnvoyConnectionInfo envoyConnectionInfo, string tokenfile, CancellationToken cancellationToken = default)
        {
            // If we don't have a session token, we create a client by logging in, else we create one from the session token
            if (!File.Exists(tokenfile))
            {
                OutputToConsole($"Refreshing token");
                var client = await EnvoyClient.FromLoginAsync(envoyConnectionInfo, cancellationToken).ConfigureAwait(false);
                await File.WriteAllTextAsync(tokenfile, client.GetToken(), cancellationToken).ConfigureAwait(false);
                return client;
            }
            return EnvoyClient.FromToken(await File.ReadAllTextAsync(tokenfile, cancellationToken).ConfigureAwait(false), envoyConnectionInfo);
        }
    }
}


