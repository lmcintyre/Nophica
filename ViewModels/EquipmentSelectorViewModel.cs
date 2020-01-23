using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nophica.Annotations;
using SaintCoinach.Graphics.Viewer.RendererSources;
using SaintCoinach.Xiv.Items;

namespace Nophica.ViewModels
{
    class EquipmentSelectorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public EquipmentSelectViewModel Parent { get; private set; }

        private Equipment _SelectedEquipment;
        
        public Equipment SelectedEquipment {
            get => _SelectedEquipment;
            set {
                _SelectedEquipment = value;

                if (value == null) {
                    SelectedValid = false;
                    SetValues("", 0, 0, 0, false, 0);
                    return;
                }
                else {
                    SelectedValid = true;
                }

                // if you know a better way to do this dm me

                SetValues(SelectedEquipment.Name,
                            SelectedEquipment.ModelMain.Value1,
                            SelectedEquipment.ModelMain.Value2,
                            SelectedEquipment.ModelMain.Value3,
                            SelectedEquipment.IsDyeable,
                            SelectedEquipment.ModelMain.Value4);

                if (SelectedEquipment.IsMainhand() && this != Parent.OffhandSelect) {

                    EquipmentSelectorViewModel oh = Parent.OffhandSelect;

                    if (SelectedEquipment.HasSubModel()) {
                        oh.SelectedEquipment = SelectedEquipment;
                        oh.SetValues(SelectedEquipment.Name + " Offhand",
                            SelectedEquipment.ModelSub.Value1,
                            SelectedEquipment.ModelSub.Value2,
                            SelectedEquipment.ModelSub.Value3,
                            SelectedEquipment.IsDyeable,
                            SelectedEquipment.ModelSub.Value4);
                    }
                    else if (oh.PossibleEquipment.Count == 0) {
                        oh.SelectedEquipment = null;
                    }
                }

                OnPropertyChanged("SelectedEquipment");
            }
        }

        public string SelectedText { get; set; }
        public bool SelectedValid { get; set; }

        private short _Model, _Base, _Variant, _Stain;

        public short Model {
            get => _Model;
            set {
                Equipment eq = PossibleEquipment
                                .FirstOrDefault(e =>
                                    e.ModelMain.Value1 == value
                                    && e.ModelMain.Value2 == _Base
                                    && e.ModelMain.Value3 == _Variant);

                if (eq != null) {
                    _Model = value;
                    SelectedEquipment = eq;
                    OnPropertyChanged(nameof(SelectedEquipment));
                }
            }
        }
        public short Base {
            get => _Base;
            set
            {
                Equipment eq = PossibleEquipment
                    .FirstOrDefault(e =>
                        e.ModelMain.Value1 == _Model
                        && e.ModelMain.Value2 == value
                        && e.ModelMain.Value3 == _Variant);

                if (eq != null)
                {
                    _Base = value;
                    SelectedEquipment = eq;
                    OnPropertyChanged(nameof(SelectedEquipment));
                }
            }
        }

        public short Variant {
            get => _Variant;
            set
            {
                Equipment eq = PossibleEquipment
                    .FirstOrDefault(e =>
                        e.ModelMain.Value1 == _Model
                        && e.ModelMain.Value2 == _Base
                        && e.ModelMain.Value3 == value);

                if (eq != null)
                {
                    _Variant = value;
                    SelectedEquipment = eq;
                    OnPropertyChanged(nameof(SelectedEquipment));
                }
            }
        }

        public short Stain {
            get => _Stain;
            set => _Stain = value;
        }

        public ObservableCollection<Equipment> PossibleEquipment { get; set; }
        public bool ShouldEnableVariant { get; set; }
        public bool ShouldEnableStain { get; set; }
        public string Title { get; set; }

        public EquipmentSelectorViewModel(EquipmentSelectViewModel parent, string title) {
            Parent = parent;
            PossibleEquipment = new ObservableCollection<Equipment>();
            Title = title;
        }

        public void SetValues(string text, short m, short b, short v, bool stain, short s) {
            SelectedText = text;
            _Model = m;
            _Base = b;

            ShouldEnableVariant = v != 0;
            _Variant = v;

            ShouldEnableStain = stain;
            _Stain = s;

            OnPropertyChanged(nameof(Model));
            OnPropertyChanged(nameof(Base));
            OnPropertyChanged(nameof(Variant));
            OnPropertyChanged(nameof(Stain));
        }
    }
}
