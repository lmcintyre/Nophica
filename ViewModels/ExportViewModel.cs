using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Nophica.Annotations;
using SaintCoinach.Xiv;
using SaintCoinach.Xiv.Items;

namespace Nophica.ViewModels
{
    struct ExportableEquipment {

        public Tribe Tribe;

        public short Model;
        public short Base;
        public short Variant;
        public short Stain;

    }

    class ExportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MainViewModel Parent { get; set; }

        public ExportViewModel(MainViewModel parent) {
            Parent = parent;
        }

        private ICommand _ExportCommand;
        public ICommand ExportCommand { get { return _ExportCommand ?? (_ExportCommand = new DelegateCommand(Export)); } }

        public void Export() {

            System.Diagnostics.Debug.WriteLine(Parent.EquipmentSelect.MainhandSelect.SelectedEquipment);

            


        }

    }
}
