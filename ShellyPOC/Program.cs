using Microsoft.Extensions.Configuration;
using NrjSolutions.Shelly.Net.Clients;
using NrjSolutions.Shelly.Net.Options;
using ShellySharp;
using ShellySharp.Settings;
using ShellySharp.v3;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime;

namespace ShellyPOC
{
    internal class Program
    {
        static ShellyAppSettings _appSettings = new ShellyAppSettings();

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
            return name.Version?.ToString() ?? "UNKNOWN";
        }

        static private void DisplayBanner()
        {
            OutputToConsole($"ShellyPOC v{GetCodeVersion()}");
        }

        static private void DisplayEnvironment()
        {
            OutputToConsole($"Running on .NET CLR: {Environment.Version.ToString()}");
        }

        static private ShellyAppSettings GetSettings()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("solarSettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var settings = new ShellyAppSettings();
            configuration.GetSection("shelly").Bind(settings);

            return settings;
        }

        private static async Task Main(string[] args)
        {
            DisplayBanner();
            DisplayEnvironment();

            _appSettings = GetSettings();

            OutputToConsole($"Devices: ({_appSettings?.Devices?.Length})");
            var devices = _appSettings?.Devices;
            foreach (var device in devices)
            {
                OutputToConsole($"=== Address: {device.Address}, Type: {device.Type} ===");
                await TestDirect(device.Address);
                //await TestUsingShellyNetSharp(device.Address);
                //await TestUsingShellySharp(device.Address);
                OutputToConsole($"=== End ===");
            }

        }

        #region Formatters

        public static DateTime? UnixTimeStampToDateTime(string? unixTimeStamp)
        {
            double ts = 0;
            double.TryParse(unixTimeStamp, out ts);
            return UnixTimeStampToDateTime(ts);
        }

        public static DateTime? UnixTimeStampToDateTime(double? unixTimeStamp)
        {
            if (unixTimeStamp == null)
            {
                return null;
            }
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp.GetValueOrDefault()).ToLocalTime();
            return dtDateTime;
        }

        public static string FormatKw(decimal? w)
        {
            if (w == null)
            {
                return "not present";
            }
            decimal watts = w.GetValueOrDefault();
            if (Math.Abs(watts) > 1000)
            {
                return $"{(watts / 1000).ToString("0.00")} KW";
            }
            if (watts != 0)
            {
                return $"{(watts).ToString("0.00")} W";
            }
            return "0 W";
        }

        public static string FormatAmps(decimal? a)
        {
            if (a == null)
            {
                return "not present";
            }
            decimal amps = a.GetValueOrDefault();
            if (amps != 0)
            {
                return $"{(amps).ToString("0")} A";
            }
            return "0 A";
        }

        public static string FormatVolts(decimal? v)
        {
            if (v == null)
            {
                return "not present";
            }
            decimal volts = v.GetValueOrDefault();
            if (volts != 0)
            {
                return $"{(volts).ToString("0.0")} V";
            }
            return "0 V";
        }

        static public string FormatSeconds(string? seconds)
        {
            long sec = 0;
            Int64.TryParse(seconds, out sec);
            return FormatSeconds(sec);
        }

        static public string FormatSeconds(decimal? seconds)
        {
            if (seconds == null)
            {
                return "not present";
            }
            if (seconds < 0)
            {
                return "less than zero seconds";
            }
            long sec = decimal.ToInt64(seconds.GetValueOrDefault());
            return FormatSeconds(sec);
        }

        static public string FormatSeconds(long? s)
        {
            if (s == null)
            {
                return "not present";
            }

            long seconds = s.GetValueOrDefault();

            long minutes = 0;
            double hours = 0;
            double days = 0;

            if (seconds > 0)
            {
                minutes = (seconds / 60);
            }
            if (minutes > 0)
            {
                hours = (minutes / 60);
            }
            if (hours > 0)
            {
                days = (hours / 24);
            }

            if (days > 1)
            {
                return $"{days.ToString("0")} days";
            }
            if (hours > 0)
            {
                var minsInHour = minutes - hours * 60;
                return $"{hours.ToString("0")}:{minsInHour.ToString("00")}";
            }
            if (minutes > 1)
            {
                return $"{minutes.ToString("0")} minutes";
            }
            return $"{seconds.ToString("0")} seconds";
        }

        #endregion

        private static async Task TestDirect(string? address)
        {
            if (address == null)
            {
                OutputToConsole("null address");
                return;
            }

            var shelly = new Shelly(address);
            var deviceInfo = await shelly.GetDeviceInfoAsync();
            if (deviceInfo != null)
            {
                OutputToConsole($"Device: {deviceInfo.Id}, {deviceInfo.App}, {deviceInfo.Fw_Id}, Authentication: {deviceInfo.Auth_En}, Matter: {deviceInfo.Matter}");
            }

            var deviceStatus = await shelly.GetDeviceStatusAsync();
            if (deviceStatus != null)
            {
                OutputSwitchStatus("switch:0", deviceStatus.Switch0);
                OutputSwitchStatus("switch:1", deviceStatus.Switch1);
                OutputSwitchStatus("switch:2", deviceStatus.Switch2);
                OutputSwitchStatus("switch:3", deviceStatus.Switch3);
            }

            var schedule = await shelly.GetDeviceScheduleAsync();
            if (schedule != null)
            {
                OutputToConsole($"scdule rev = {schedule.Rev}, num jobs = {schedule.Jobs?.Length}, enabled jobs = {schedule.HasEnabledJobs}");
                if (schedule.Jobs != null)
                {
                    foreach (var job in schedule.Jobs)
                    {
                        OutputJob(job);
                    }
                }
            }

            var switchConfig = await shelly.GetSwitchConfigAsync(0);
            if (switchConfig != null)
            {
                OutputSwitchConfig("switch ID 0 - config", switchConfig);
            }

            var switchStatus = await shelly.GetSwitchStatusAsync(0);
            if (switchStatus != null)
            {
                OutputSwitchStatus("switch ID 0 - status", switchStatus);
                /*
                var result =  await shelly.ChangeSwitchAsync(0, false);
                if (result != null)
                {
                    OutputToConsole($"switch ID 0 ison = {result.IsOn}");
                }
                */
            }
        }

        private static void OutputJob(ShellyScheduleJob? job)
        {
            if (job == null)
            {
                return;
            }
            OutputToConsole($"ID: {job.Id}, Enabled = {job.Enable}, Timespec = {job.Timespec}");
            if (job.Calls != null)
            {
                foreach (var call in job.Calls)
                {
                    OutputToConsole($"    Call: {call.Method}");
                }
            }
        }

        private static void OutputSwitchStatus(string name, ShellySwitchStatus? status)
        {
            if (status == null)
            {
                OutputToConsole($"{name} not present");
                return;
            }
            OutputToConsole($"{name} ID: {status.Id}, On = {status.Output}");
            OutputToConsole($"    Started At: {UnixTimeStampToDateTime(status.Timer_Started_At)}, Duration: = {FormatSeconds(status.Timer_Duration)} , Remaining: = {FormatSeconds(status.Timer_Remaining)}");
            OutputToConsole($"    Load: {FormatKw(status.Apower)}, Voltage = {FormatVolts(status.Voltage)}");
            OutputToConsole($"    Active Energy: {FormatKw(status.Aenergy?.Total)}, TS = {UnixTimeStampToDateTime(status.Aenergy?.Minute_Ts)}, Mins = {status.Aenergy?.By_Minute_Str}");
            OutputToConsole($"    Returned Active Energy: {FormatKw(status.Ret_Aenergy?.Total)}, TS = {UnixTimeStampToDateTime(status.Ret_Aenergy?.Minute_Ts)}, Mins = {status.Ret_Aenergy?.By_Minute_Str}");
            OutputToConsole($"    Temperature: {status.Temperature?.TC} C, {status.Temperature?.TF} F");
        }

        private static void OutputSwitchConfig(string name, ShellySwitchConfig? config)
        {
            if (config == null)
            {
                OutputToConsole($"{name} not present");
                return;
            }
            OutputToConsole($"{name} ID: {config.Id}, Mode = {config.In_Mode}");
            OutputToConsole($"    Auto On = {config.Auto_On}, Auto On Delay = {FormatSeconds(config.Auto_On_Delay)}");
            OutputToConsole($"    Auto Off = {config.Auto_Off}, Auto On Delay = {FormatSeconds(config.Auto_Off_Delay)}");
            OutputToConsole($"    Power Limit: {FormatKw(config.Power_Limit)}, Voltage Limit = {FormatVolts(config.Voltage_Limit)}, Current Limit {FormatAmps(config.Current_Limit)}");
        }

        private static async Task TestUsingShellyNetSharp(string? address)
        {
            Shelly1PmOptions options = new Shelly1PmOptions()
            {
                UserName = "",
                Password = "",
                ServerUri = new Uri("http://" + address)
            };

            var shelly = new Shelly1PmClient(new HttpClient(), options);

            var response = await shelly.GetStatus(CancellationToken.None);

            OutputToConsole($"Success: {response.IsSuccess}");
            OutputToConsole($"Uptime: {response.Value.Uptime}");
            OutputToConsole($"Meter Power: {response.Value.Meters[0].Power}");
        }

        private static async Task TestUsingShellySharp(string? address)
        {
            var deviceInfo = Discover.GetDeviceInformation("http://" + address);
            OutputToConsole($"Device: Type: {deviceInfo.Type}, Model: {deviceInfo.Model}, Hostname: {deviceInfo.Hostname}, MAC: {deviceInfo.Mac}");

            //dynamic device = await ShellySharp.v3.Dynamic.ShellyDeviceFactory.CreateDynamicDeviceAsync(SHELLY_IP);
            //var client = await GetClientAsync(SHELLY_IP);
            //OutputToConsole($"Device: {device.GetType().Name}");

            var device = await ShellyDeviceFactory.CreateDeviceAsync(address);
            OutputToConsole($"RPC Url {device.RpcUrl}");
        }

        private static async Task<ShellyDeviceBase> GetClientAsync(string ip)
        {
            return await ShellyDeviceFactory.CreateDeviceAsync(ip);
        }
    }
}


