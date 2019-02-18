using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDE.ThemeChanger.Providers;
using Prism.Mvvm;
using System.Windows;

namespace WDE.ThemeChanger.ViewModels
{
    public class ThemeConfigViewModel : BindableBase
    {
        public Action SaveAction { get; set; }
        private readonly IThemeSettingsProvider settings;

        private string _name;
        private List<string> _themes;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public List<string> Themes
        {
            get { return _themes; }
            set { SetProperty(ref _themes, value); }
        }

        public ThemeConfigViewModel(IThemeSettingsProvider s)
        {
            SaveAction = Save;

            _name = s.GetSettings().Name;
            _themes = s.GetSettings().themes;

            settings = s;
        }

        private void Save()
        {
            settings.UpdateSettings(Name);
        }

        public void UpdateTheme()
        {
            string curentTheme = Application.Current.Resources.MergedDictionaries[1].Source.ToString();
            string compareStr = "/" + _name + ".xaml";

            if (!curentTheme.Contains(compareStr))
            {
                Application.Current.Resources.MergedDictionaries.Clear();
                Uri uriOne = new Uri("pack://application:,,,/Xceed.Wpf.AvalonDock.Themes.VS2013;component/" + _name + ".xaml");
                Uri uriTwo = new Uri("Themes/" + _name + ".xaml", UriKind.RelativeOrAbsolute);
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uriOne });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uriTwo });
            }
        }
    }
}
