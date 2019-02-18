using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDE.ThemeChanger.Data
{
    public class ThemeSettings
    {
        public string Name { get; set; }
        public List<string> themes { get; set; }

        public ThemeSettings()
        {
            themes = new List<string>();
        }
    }
}
