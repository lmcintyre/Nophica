using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nophica.Properties;

namespace Nophica
{
    class WindowPosition {
        public double MainWindowTop;
        public double MainWindowLeft;
        public double MainWindowWidth;
        public double MainWindowHeight;

        public static WindowPosition Load() {
            string json = Settings.Default.WindowPosition;
            WindowPosition result;
            if (!string.IsNullOrEmpty(json))
                result = JsonConvert.DeserializeObject<WindowPosition>(json);
            else
                result = new WindowPosition();
            return result;
        }

        public void Save() {
            Settings.Default.WindowPosition = JsonConvert.SerializeObject(this);
        }

        public WindowPosition() { }

        public WindowPosition(double top, double left, double width, double height) {
            MainWindowTop = top;
            MainWindowLeft = left;
            MainWindowWidth = width;
            MainWindowHeight = height;
        }
    }
}
