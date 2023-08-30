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
        private static readonly string localAppDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Resizator", "settings.txt");

        public string? Percent { get; private set; } = null;
        public string? Path { get; private set; } = null;

        private Settings() { }
        
        public static Settings INSTANCE { get; private set; } = new();

        public static void Load()
        {
            INSTANCE = JsonSerializer.Deserialize<Settings>(File.ReadAllText(localAppDataPath));
        }

        public static void Save()
        {
            File.WriteAllText(localAppDataPath, JsonSerializer.Serialize(INSTANCE));
        }
    }
}
