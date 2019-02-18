using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDE.ThemeChanger.Data;
using Newtonsoft.Json;
using System.IO;
using WDE.Module.Attributes;
using System.Windows;

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

        public void UpdateTheme()
        {
            string curentTheme = Application.Current.Resources.MergedDictionaries[1].Source.ToString();
            string compareStr = "/" + settings.Name + ".xaml";

            if (!curentTheme.Contains(compareStr))
            {
                Application.Current.Resources.MergedDictionaries.Clear();
                Uri uriOne = new Uri("pack://application:,,,/Xceed.Wpf.AvalonDock.Themes.VS2013;component/" + settings.Name + ".xaml");
                Uri uriTwo = new Uri("Themes/" + settings.Name + ".xaml", UriKind.RelativeOrAbsolute);
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uriOne });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uriTwo });
            }
        }
    }
}
