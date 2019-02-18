using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDE.ThemeChanger.Data;
using Newtonsoft.Json;
using System.IO;
using WDE.Module.Attributes;

namespace WDE.ThemeChanger.Providers
{
    [AutoRegister, SingleInstance]
    public class ThemeSettingsProvider : IThemeSettingsProvider
    {
        private ThemeSettings settings { get; }

        public ThemeSettings GetSettings() => settings;

        public ThemeSettingsProvider()
        {
            if (File.Exists("theme.json"))
            {
                JsonSerializer ser = new JsonSerializer() { TypeNameHandling = TypeNameHandling.Auto };
                using (StreamReader re = new StreamReader("theme.json"))
                {
                    JsonTextReader reader = new JsonTextReader(re);
                    settings = ser.Deserialize<ThemeSettings>(reader);
                }
            }

            if (settings == null)
            {
                settings = new ThemeSettings();
                settings.themes.Add("BlueTheme");
                settings.themes.Add("DarkTheme");
                settings.Name = "BlueTheme";
            }
        }

        public void UpdateSettings(string name)
        {
            settings.Name = name;
            JsonSerializer ser = new JsonSerializer() { TypeNameHandling = TypeNameHandling.Auto };
            using (StreamWriter file = File.CreateText(@"theme.json"))
            {
                ser.Serialize(file, settings);
            }
        }
    }
}
