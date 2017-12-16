using API.DARP.Data.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP
{
    [Serializable]
    public class Settings
    {
        public const int SETTINGS_VERSION_NUMBER = 8;

        public Settings()
        {
            SettingsVersion = SETTINGS_VERSION_NUMBER;
            GeneralSettings = GeneralSettings.CreateDefaults();
            ILSConfigurationSettings = ILSConfigurationSettings.CreateDefaults();
        }

        public int SettingsVersion { get; set; }


        public GeneralSettings GeneralSettings { get; set; }

        public ILSConfigurationSettings ILSConfigurationSettings { get; set; }


        public static Settings CheckDeserializeSettings(Settings deserializeSettings)
        {

            if (deserializeSettings.SettingsVersion != SETTINGS_VERSION_NUMBER)
                return new Settings();
            else
                return deserializeSettings;
        }

        internal static Settings CheckDeserializeSettings(object loadExecutionSettings)
        {
            throw new NotImplementedException();
        }
    }
}
