using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Resizator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string KEY_PERCENT = "PERCENT";
        public const string KEY_PATH = "PATH";
        public const string DEFAULT_PERCENT = "20";

        

        public static void AddOrUpdateSetting(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(localAppDataPath);
            KeyValueConfigurationElement setting = config.AppSettings.Settings[key];

            if (setting != null)
            {
                setting.Value = value;
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static string? GetSettingValue(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(localAppDataPath);
            return config.AppSettings.Settings[key]?.Value;
        }
    }
}
