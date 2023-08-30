using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Resizator
{
    public class Settings
    {
        private static readonly string localAppDataDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Resizator");
        private static readonly string localAppDataFile = System.IO.Path.Combine(localAppDataDir, "settings.txt");

        public string? Percent { get; set; } = "20";
        public string? Path { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static Settings Load()
        {
            if(File.Exists(localAppDataFile))
            {
                return JsonSerializer.Deserialize<Settings>(File.ReadAllText(localAppDataFile)) ?? new Settings();
            }
            return new Settings();
        }

        public void Save()
        {
            Directory.CreateDirectory(localAppDataDir);
            File.WriteAllText(localAppDataFile, JsonSerializer.Serialize(this));
        }
    }
}
