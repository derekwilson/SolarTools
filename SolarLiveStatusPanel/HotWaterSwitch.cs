using NEnvoy.Models;
using NLog;

namespace SolarLiveStatusPanel
{
    internal class HotWaterSwitchStatus
    {
        public bool Configured;

        public DateTime Updated;
        public int Id;
        public decimal LoadWatts;
        public bool IsOn;
        public decimal? RemainingDelay;

        public bool AutoOn;
        public decimal AutoOnDelay;
        public bool AutoOff;
        public decimal AutoOffDelay;

        public bool EnabledScheduledJobs;
    }

    internal class HotWaterSwitch
    {
        public HotWaterSwitchStatus LatestStatus = new HotWaterSwitchStatus
        {
            Configured = false,

            Updated = DateTime.MinValue,
            LoadWatts = 0,
            IsOn = false,
            RemainingDelay = null,

            AutoOn = false,
            AutoOnDelay = 0,
            AutoOff = false,
            AutoOffDelay = 0,
        };

        private int _id = -1;

        private Shelly? shelly = null;
        private ILogger Logger;

        public HotWaterSwitch(ILogger logger)
        {
            this.Logger = logger;
        }

        public async Task InitAsync(ShellyAppSettingsDevice? device)
        {
            if (device != null)
            {
                try
                {
                    LatestStatus.Configured = true;
                    shelly = new Shelly(device.Address!);
                    var deviceStatus = await shelly.GetDeviceStatusAsync();
                    if (deviceStatus != null && deviceStatus.Switch0 != null)
                    {
                        _id = deviceStatus.Switch0.Id;
                        Logger.Info(() => $"HotWaterSwitch.InitAsync, id = {_id}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(() => "HotWaterSwitch.InitAsync", ex);
                    LatestStatus.Configured = false;
                }
            }
        }

        public async Task GetStatusAsync()
        {
            try
            {
                if (shelly == null || _id < 0)
                {
                    return;
                }

                var switchStatus = await shelly.GetSwitchStatusAsync(_id);
                if (switchStatus != null)
                {
                    LatestStatus.Updated = DateTime.Now;
                    LatestStatus.Id = switchStatus.Id;
                    LatestStatus.LoadWatts = switchStatus.Apower;
                    LatestStatus.IsOn = switchStatus.Output;
                    LatestStatus.RemainingDelay = switchStatus.Timer_Remaining;
                }

                var switchConfig = await shelly.GetSwitchConfigAsync(_id);
                if (switchConfig != null)
                {
                    LatestStatus.AutoOn = switchConfig.Auto_On;
                    LatestStatus.AutoOnDelay = switchConfig.Auto_On_Delay;
                    LatestStatus.AutoOff = switchConfig.Auto_Off;
                    LatestStatus.AutoOffDelay = switchConfig.Auto_Off_Delay;
                }

                var deviceSchedule = await shelly.GetDeviceScheduleAsync();
                if (deviceSchedule != null)
                {
                    LatestStatus.EnabledScheduledJobs = deviceSchedule.HasEnabledJobs;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(() => "HotWaterSwitch.GetStatusAsync", ex);
            }
        }

        public async Task SetSwitchAsync(bool on)
        {
            try
            {
                if (shelly == null || _id < 0)
                {
                    return;
                }

                var result = await shelly.ChangeSwitchAsync(_id, on);
            }
            catch (Exception ex)
            {
                Logger.LogException(() => "HotWaterSwitch.SetSwitchAsync", ex);
            }

        }

        public async Task ToggleSwitchAsync()
        {
            try
            {
                if (shelly == null || _id < 0)
                {
                    return;
                }

                var result = await shelly.ToggleSwitchAsync(_id);
                if (result != null)
                {
                    // this will cause the UI to change
                    LatestStatus.IsOn = result.IsOn;
                    Logger.Info(() => $"HotWaterSwitch.ToggleSwitchAsync, is on = {result.IsOn}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(() => "HotWaterSwitch.ToggleSwitchAsync", ex);
            }
        }
    }
}
