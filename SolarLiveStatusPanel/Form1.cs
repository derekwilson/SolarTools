using Microsoft.Extensions.Configuration;
using NEnvoy.Models;
using SolarPOC;
using System.Reflection;
using System.Text;

namespace SolarLiveStatusPanel
{
    public partial class Form1 : Form
    {
        const string NOT_CONFIGURED = "Not Configured";

        EnvoyConnectionInfo? EnvoyConnection = null;
        ShellyAppSettings? ShellySettings = null;

        MeterReader Reader = new MeterReader();
        HotWaterSwitch HotWaterSwitch = new HotWaterSwitch();

        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private string GetCodeVersion()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            return name.Version?.ToString() ?? "UNKNOWN";
        }

        private void GetSettings()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("solarSettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var enphaseSettings = new EnphaseAppSettings();
            configuration.GetSection("enphase").Bind(enphaseSettings);

            EnvoyConnection = ValidateEnvoyConfig(enphaseSettings);

            ShellySettings = new ShellyAppSettings();
            configuration.GetSection("shelly").Bind(ShellySettings);

            ShellySettings = ValidateShellySettings(ShellySettings);
        }

        private EnvoyConnectionInfo? ValidateEnvoyConfig(EnphaseAppSettings? appSettings)
        {
            if (appSettings == null || appSettings.Credentials == null || appSettings.Gateway == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(appSettings.Gateway.Address))
            {
                return null;
            }
            if (string.IsNullOrEmpty(appSettings.Credentials.User))
            {
                return null;
            }
            if (string.IsNullOrEmpty(appSettings.Credentials.Password))
            {
                return null;
            }

            return new EnvoyConnectionInfo
            {
                EnvoyHost = appSettings.Gateway.Address,
                Username = appSettings.Credentials.User,
                Password = appSettings.Credentials.Password,
            };
        }

        private ShellyAppSettings? ValidateShellySettings(ShellyAppSettings? settings)
        {
            if (settings == null || settings.Devices == null || settings.Devices.Length < 1)
            {
                return null;
            }
            // we are just going to use the first device that is type 1 as the hotwater device
            var device = settings.GetFirstDevice(ShellyAppSettings.HOTWATER_SWITCH_TYPE);
            if (device == null) {
                return null;
            }
            return settings;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetSettings();

            _ = Reader.InitAsync(EnvoyConnection);
            _ = HotWaterSwitch.InitAsync(ShellySettings?.GetFirstDevice(ShellyAppSettings.HOTWATER_SWITCH_TYPE));

            PopulateTitle();
            PopulateSolarPanelLabels();
            PopulateHotWaterSwitch();

            AdjustAllLabels();
        }

        private void AdjustAllLabels()
        {
            ScaleLabelFont(labelGenerating);
            ScaleLabelFont(labelLoad);
            ScaleLabelFont(labelExportImport);
            ScaleLabelFont(labelHotWater);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized || this.WindowState == FormWindowState.Normal)
            {
                AdjustAllLabels();
            }
        }

        private void timerUI_Tick(object sender, EventArgs e)
        {
            if (!Reader.LatestReadings.Configured)
            {
                // this can happen some time after startup
                PopulateTitle();
                PopulateSolarPanelLabels();
            }
            if (Reader.LatestReadings.Updated != DateTime.MinValue)
            {
                // we have got an update
                PopulateTitle();
                PopulateSolarPanelLabels();
            }
            if (HotWaterSwitch.LatestStatus.Updated != DateTime.MinValue)
            {
                PopulateHotWaterSwitch();
            }

            AdjustAllLabels();
        }

        private void timerGetData_Tick(object sender, EventArgs e)
        {
            // maybe sync lock this
            _ = Reader.GetReadingsAsync();
            _ = HotWaterSwitch.GetStatusAsync();
        }

        private void checkBoxHotWater_Click(object sender, EventArgs e)
        {
            _ = HotWaterSwitch.ToggleSwitchAsync();
        }

        private void PopulateTitle()
        {
            var envoyIp = EnvoyConnection?.EnvoyHost;
            if (Reader.LatestReadings.Updated == DateTime.MinValue)
            {
                this.Text = $"Solar Live Status v{GetCodeVersion()}, {envoyIp}";
                return;
            }
            this.Text = $"Solar Live Status v{GetCodeVersion()}, {envoyIp}, Updated {Reader.LatestReadings.Updated.ToString("HH:mm:ss")}";
        }

        private void PopulateSolarPanelLabels()
        {
            if (EnvoyConnection == null || !Reader.LatestReadings.Configured)
            {
                labelGenerating.Text = NOT_CONFIGURED;
                labelLoad.Text = NOT_CONFIGURED;
                labelExportImport.Text = NOT_CONFIGURED;
                return;
            }
            labelGenerating.Text = Utils.FormatKw(Reader.LatestReadings.Generating);
            labelLoad.Text = Utils.FormatKw(Reader.LatestReadings.Load);
            groupBoxExport.Text = Reader.LatestReadings.IsExporting ? "Exporting to Grid" : "Importing from Grid";
            labelExportImport.Text = Utils.FormatKw(Reader.LatestReadings.ExportImport);
            labelExportImport.ForeColor = Reader.LatestReadings.IsExporting ? Color.Green : Color.Red;
        }

        private void PopulateHotWaterSwitch()
        {
            if (ShellySettings == null)
            {
                labelHotWater.Text = NOT_CONFIGURED;
                return;
            }

            // the switch
            var bgSliderColour = HotWaterSwitch.LatestStatus.IsOn && HotWaterSwitch.LatestStatus.LoadWatts > 0 ? Color.Red : Color.Green;
            checkBoxHotWater.OnBackColor = bgSliderColour;

            // the text label for the load
            var onoff = HotWaterSwitch.LatestStatus.IsOn ? "On, " : "Off, ";
            var remainingTeext = "";
            if (HotWaterSwitch.LatestStatus.RemainingDelay != null)
            {
                remainingTeext = $", {Utils.FormatSeconds(HotWaterSwitch.LatestStatus.RemainingDelay)} left";
            }

            var loadText = $"{onoff}{Utils.FormatKw(HotWaterSwitch.LatestStatus.LoadWatts)}";
            labelHotWater.Text = $"{loadText}{remainingTeext}";
            checkBoxHotWater.Checked = HotWaterSwitch.LatestStatus.IsOn;

            // the container
            if (HotWaterSwitch.LatestStatus.Updated > DateTime.MinValue)
            {
                StringBuilder str = new StringBuilder(100);
                str.Append("Hot Water Switch, ");
                if (HotWaterSwitch.LatestStatus.AutoOn)
                {
                    str.Append($"Auto On after {Utils.FormatSeconds(HotWaterSwitch.LatestStatus.AutoOnDelay)}");
                }
                else
                {
                    str.Append($"Auto On disabled");
                }
                if (HotWaterSwitch.LatestStatus.AutoOff)
                {
                    str.Append($", Auto Off after {Utils.FormatSeconds(HotWaterSwitch.LatestStatus.AutoOffDelay)}");
                }
                else
                {
                    str.Append($", Auto Off disabled");
                }
                if (HotWaterSwitch.LatestStatus.EnabledScheduledJobs)
                {
                    str.Append($", There are enabled scheduled jobs");
                }
                else
                {
                    str.Append($", No sceduled jobs");
                }

                groupBoxHotWater.Text = str.ToString();
            }
        }

        private void ScaleLabelFont(Label lab)
        {
            //if (string.IsNullOrEmpty(lab.Text) || lab.Height == 0 || lab.Width == 0) return;
            SizeF extent = TextRenderer.MeasureText(lab.Text, lab.Font);

            float hRatio = lab.Height / extent.Height;
            float wRatio = lab.Width / extent.Width;
            float ratio = (hRatio < wRatio) ? hRatio : wRatio;

            float newSize = lab.Font.Size * ratio;

            if (newSize > 0)
            {
                lab.Font = new Font(lab.Font.FontFamily, newSize, lab.Font.Style);
            }
        }
    }
}
