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

namespace Nophica.Views
{

    public class TextList : List<string> { }

    /// <summary>
    /// Interaction logic for EquipmentSelectorView.xaml
    /// </summary>
    public partial class EquipmentSelectorView : UserControl
    {
        public EquipmentSelectorView()
        {
            InitializeComponent();
        }
    }
}
