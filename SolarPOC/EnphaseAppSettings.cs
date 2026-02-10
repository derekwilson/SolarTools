using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarPOC
{
    internal class EnphaseAppSettings
    {
        public EnphaseAppSettingsGateway? Gateway { get; set; }
        public EnphaseAppSettingsCredentials? Credentials { get; set; }
    }

    internal class EnphaseAppSettingsCredentials
    {
        public string? User { get; set; }
        public string? Password { get; set; }
    }

    internal class EnphaseAppSettingsGateway
    {
        public string? Address { get; set; }
    }

}
