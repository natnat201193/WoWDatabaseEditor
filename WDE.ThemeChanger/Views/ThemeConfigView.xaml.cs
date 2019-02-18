using System;
using System.Windows;
using System.Windows.Controls;
using WDE.ThemeChanger.ViewModels;

namespace WDE.ThemeChanger.Views
{
    /// <summary>
    /// Interaction logic for ThemeConfigView.xaml
    /// </summary>
    public partial class ThemeConfigView : UserControl
    {
        public ThemeConfigView()
        {
            InitializeComponent();
        }

        private void ComboBox_OnItemSelect(object sender, SelectionChangedEventArgs e)
        {
            ThemeConfigViewModel model = DataContext as ThemeConfigViewModel;
            model.UpdateTheme();
        }
    }
}
