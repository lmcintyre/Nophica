using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nophica.ViewModels;

namespace Nophica.Views
{
    /// <summary>
    /// Interaction logic for EquipmentSelect.xaml
    /// </summary>
    public partial class EquipmentSelectView : UserControl
    {
        public EquipmentSelectView()
        {
            try {
                InitializeComponent();
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }
    }
}
