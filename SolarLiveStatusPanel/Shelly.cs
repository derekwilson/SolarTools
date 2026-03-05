using System.Text.Json;
using System.Text.Json.Serialization;

namespace SolarLiveStatusPanel
{
    internal class ShellyAppSettings
    {
        public const int HOTWATER_SWITCH_TYPE = 1;

        public ShellyAppSettingsDevice[]? Devices { get; set; }

        public ShellyAppSettingsDevice? GetFirstDevice(int type)
        {
            if (Devices == null)
            {
                return null;
            }
            return Devices.First(d => d.Type == 1 && !string.IsNullOrEmpty(d.Address));
        }
    }

    internal class ShellyAppSettingsDevice
    {
        public string? Address { get; set; }
        public int? Type { get; set; }
    }

    internal class ShellyDeviceInfo
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public string? Mac { get; set; }
        public int Slot { get; set; }
        public string? Model { get; set; }
        public int Gen { get; set; }
        public string? Fw_Id { get; set; }
        public string? Ver { get; set; }
        public string? App { get; set; }
        public bool Auth_En { get; set; }
        public string? Auth_Domain { get; set; }
        public bool? Matter { get; set; }
    }
    internal class ShellyDeviceStatus
    {
        [JsonPropertyName("switch:0")]
        public ShellySwitchStatus? Switch0 { get; set; }
        [JsonPropertyName("switch:1")]
        public ShellySwitchStatus? Switch1 { get; set; }
        [JsonPropertyName("switch:2")]
        public ShellySwitchStatus? Switch2 { get; set; }
        [JsonPropertyName("switch:3")]
        public ShellySwitchStatus? Switch3 { get; set; }
    }

    internal class ShellySwitchStatus
    {
        public int Id { get; set; }
        public string? Source { get; set; }
        public bool Output { get; set; }
        public double? Timer_Started_At { get; set; }
        public decimal? Timer_Duration { get; set; }
        [JsonIgnore]
        public decimal? Timer_Remaining
        {
            get
            {
                if (Timer_Started_At == null || Timer_Duration == null)
                {
                    return null;
                }
                var elapsed = Convert.ToDouble(DateTimeOffset.UtcNow.ToUnixTimeSeconds()) - Timer_Started_At;
                return Timer_Duration - Convert.ToDecimal(elapsed);
            }
        }
        public decimal Apower { get; set; }
        public decimal Voltage { get; set; }
        public decimal Freq { get; set; }
        public decimal Current { get; set; }
        public ShellyEnergyCounter? Aenergy { get; set; }
        public ShellyEnergyCounter? Ret_Aenergy { get; set; }
        public ShellyTemperatureInformation? Temperature { get; set; }
    }

    internal class ShellyEnergyCounter
    {
        public decimal Total { get; set; }
        public decimal[]? By_Minute { get; set; }
        [JsonIgnore]
        public string? By_Minute_Str
        {
            get
            {
                if (By_Minute == null)
                {
                    return null;
                }
                return string.Join(", ", By_Minute);
            }
        }
        public double Minute_Ts { get; set; }
    }

    internal class ShellyTemperatureInformation
    {
        public decimal? TC { get; set; }
        public decimal? TF { get; set; }
    }

    internal class ShellySwitchConfig
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? In_Mode { get; set; }
        public string? Initial_State { get; set; }
        public bool Auto_On { get; set; }
        public decimal Auto_On_Delay { get; set; }
        public bool Auto_Off { get; set; }
        public decimal Auto_Off_Delay { get; set; }
        public decimal Power_Limit { get; set; }
        public decimal Voltage_Limit { get; set; }
        public decimal? Undervoltage_Limit { get; set; }
        public bool Autorecover_Voltage_Errors { get; set; }
        public decimal? Current_Limit { get; set; }
        public bool? Reverse { get; set; }
    }

    internal class ShellySchedule
    {
        public ShellyScheduleJob[]? Jobs { get; set; }
        public int Rev { get; set; }
        [JsonIgnore]
        public bool HasEnabledJobs
        {
            get
            {
                if (Jobs == null || Jobs.Length < 1)
                {
                    return false;
                }
                foreach (var job in Jobs)
                {
                    if (job != null && job.Enable)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    internal class ShellyScheduleJob
    {
        public int Id { get; set; }
        public bool Enable { get; set; }
        /// <summary>
        /// cron string
        /// </summary>
        public string? Timespec { get; set; }
        public ShellyScheduleJobCall[]? Calls { get; set; }
    }

    internal class ShellyScheduleJobCall
    {
        public string? Method { get; set; }
    }

    internal class ShellySwitchChangeResult
    {
        public bool IsOn { get; set; }
        public bool Has_Timer { get; set; }
        public decimal Timer_Started_At { get; set; }
        public decimal timer_duration { get; set; }
        public decimal timer_remaining { get; set; }
        public bool Overpower { get; set; }
        public string? Source { get; set; }
    }

    internal class Shelly
    {
        private const string DEVICE_INFO_URL = "shelly";
        private const string DEVICE_STATUS_URL = "rpc/Shelly.getstatus";
        private const string DEVICE_SCHEDULE_LIST_URL = "rpc/Schedule.List";
        private const string SWITCH_STATUS_URL = "rpc/Switch.GetStatus?id=";
        private const string SWITCH_CONFIG_URL = "rpc/Switch.GetConfig?id=";

        // Use a single, static HttpClient instance for performance (recommended pattern).
        private static HttpClient client = new HttpClient();

        private string _ip;

        public Shelly(string ip)
        {
            _ip = ip;
            client.BaseAddress = new Uri(GetBaseUrl());
        }

        private string GetBaseUrl()
        {
            return $"http://{_ip}/";
        }

        private string GetSwitchStatusUrl(int id)
        {
            return $"{SWITCH_STATUS_URL}{id}";
        }

        private string GetSwitchConfigUrl(int id)
        {
            return $"{SWITCH_CONFIG_URL}{id}";
        }

        private string GetSwitchOnUrl(int id)
        {
            return $"/relay/{id}?turn=on";
        }

        private string GetSwitchOffUrl(int id)
        {
            return $"/relay/{id}?turn=off";
        }

        private string GetSwitchToggleUrl(int id)
        {
            return $"/relay/{id}?turn=toggle";
        }

        private JsonSerializerOptions GetSerialiserOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ShellyDeviceInfo?> GetDeviceInfoAsync()
        {
            return await GetAsync<ShellyDeviceInfo?>(DEVICE_INFO_URL);
        }
        public async Task<ShellySchedule?> GetDeviceScheduleAsync()
        {
            return await GetAsync<ShellySchedule?>(DEVICE_SCHEDULE_LIST_URL);
        }

        public async Task<ShellyDeviceStatus?> GetDeviceStatusAsync()
        {
            return await GetAsync<ShellyDeviceStatus?>(DEVICE_STATUS_URL);
        }
        public async Task<ShellySwitchStatus?> GetSwitchStatusAsync(int id)
        {
            return await GetAsync<ShellySwitchStatus?>(GetSwitchStatusUrl(id));
        }

        public async Task<ShellySwitchConfig?> GetSwitchConfigAsync(int id)
        {
            return await GetAsync<ShellySwitchConfig?>(GetSwitchConfigUrl(id));
        }

        public async Task<ShellySwitchChangeResult?> ChangeSwitchAsync(int id, bool on)
        {
            if (on)
            {
                return await GetAsync<ShellySwitchChangeResult?>(GetSwitchOnUrl(id));
            }
            else
            {
                return await GetAsync<ShellySwitchChangeResult?>(GetSwitchOffUrl(id));
            }
        }

        public async Task<ShellySwitchChangeResult?> ToggleSwitchAsync(int id)
        {
            return await GetAsync<ShellySwitchChangeResult?>(GetSwitchToggleUrl(id));
        }

        private async Task<T?> GetAsync<T>(string url)
        {
            string jsonResponse = await client.GetStringAsync(url);
            //Console.WriteLine($"response: {jsonResponse}");
            T? info = JsonSerializer.Deserialize<T>(jsonResponse, GetSerialiserOptions());
            return info;
        }
    }
}

