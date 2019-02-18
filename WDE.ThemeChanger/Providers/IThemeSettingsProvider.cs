using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDE.ThemeChanger.Data;

namespace WDE.ThemeChanger.Providers
{
    public interface IThemeSettingsProvider
    {
        ThemeSettings GetSettings();
        void UpdateSettings(string name);
        void UpdateTheme();
    }
}
