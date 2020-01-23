using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nophica.Annotations;
using SaintCoinach.Xiv;
using SaintCoinach.Xiv.Items;

namespace Nophica.ViewModels
{
    enum Slot {
        Mainhand = 0,
        Offhand,
        Head,
        Body,
        Hands,
        Legs = 6,
        Feet,
        Ears,
        Neck,
        Wrists,
        FingerL,
        FingerR
    }

    sealed class EquipmentSelectViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Text { get; set; }

        private readonly Equipment[] AllEquipment;
        public ObservableCollection<ClassJob> ClassJobs { get; set; }

        public EquipmentSelectorViewModel MainhandSelect { get; set; }
        public EquipmentSelectorViewModel OffhandSelect { get; set; }
        public EquipmentSelectorViewModel HeadSelect { get; set; }
        public EquipmentSelectorViewModel BodySelect { get; set; }
        public EquipmentSelectorViewModel HandsSelect { get; set; }
        public EquipmentSelectorViewModel LegsSelect { get; set; }
        public EquipmentSelectorViewModel FeetSelect { get; set; }
        public EquipmentSelectorViewModel EarsSelect { get; set; }
        public EquipmentSelectorViewModel NeckSelect { get; set; }
        public EquipmentSelectorViewModel WristSelect { get; set; }
        public EquipmentSelectorViewModel RingLSelect { get; set; }
        public EquipmentSelectorViewModel RingRSelect { get; set; }

        private ClassJob _SelectedClassJob;
        public ClassJob SelectedClassJob {
            get => _SelectedClassJob;
            set {
                _SelectedClassJob = value;
                
                UpdateEquipmentLists();
                // try
                // {
                //     UpdateEquipmentLists();
                // }
                // catch (Exception ex)
                // {
                //     System.Diagnostics.Debug.WriteLine(ex.Message);
                //     System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                // }
            }
        }

        private MainViewModel Parent { get; set; }

        public EquipmentSelectViewModel(MainViewModel parent) {
            Parent = parent;

            AllEquipment = Parent.Realm.GameData
                .GetSheet<Item>()
                .OfType<Equipment>()
                .Where(e => !e.EquipSlotCategory.PossibleSlots.Any(s => s.Key == 5 || s.Key == 13))
                .OrderBy(e => e.Name)
                .ToArray();

            ClassJobs = new ObservableCollection<ClassJob>();
            Parent.Realm.GameData
                .GetSheet<ClassJob>()
                .Where(x => x.Name != "adventurer")
                .ToList()
                .ForEach(x => ClassJobs.Add(x));
            
            MainhandSelect = new EquipmentSelectorViewModel(this, "Mainhand");
            OffhandSelect = new EquipmentSelectorViewModel(this, "Offhand");

            HeadSelect = new EquipmentSelectorViewModel(this, "Head");
            BodySelect = new EquipmentSelectorViewModel(this, "Body");
            HandsSelect = new EquipmentSelectorViewModel(this, "Hands");
            LegsSelect = new EquipmentSelectorViewModel(this, "Legs");
            FeetSelect = new EquipmentSelectorViewModel(this, "Feet");

            EarsSelect = new EquipmentSelectorViewModel(this, "Ears");
            NeckSelect = new EquipmentSelectorViewModel(this, "Neck");
            WristSelect = new EquipmentSelectorViewModel(this, "Wrist");
            RingLSelect = new EquipmentSelectorViewModel(this, "Left Ring");
            RingRSelect = new EquipmentSelectorViewModel(this, "Right Ring");

            SelectedClassJob = ClassJobs[0];
        }

        private void UpdateEquipmentLists() {

            /*
             * 0 mainhand
             * 1 offhand
             * 2 head
             * 3 body
             * 4 gloves
             * 6 legs
             * 7 feet
             * 8 ears
             * 9 neck
             * 10 wrist
             * 11 fingerl
             * 12 fingerr
             */


            OffhandSelect.PossibleEquipment.Clear();
            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int)Slot.Offhand) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(OffhandSelect.PossibleEquipment.Add);

            if (OffhandSelect.PossibleEquipment.Count > 0)
                OffhandSelect.SelectedEquipment = OffhandSelect.PossibleEquipment[0];

            MainhandSelect.PossibleEquipment.Clear();

            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int) Slot.Mainhand) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(MainhandSelect.PossibleEquipment.Add);
            
            if (MainhandSelect.PossibleEquipment.Count > 0)
                MainhandSelect.SelectedEquipment = MainhandSelect.PossibleEquipment[0];

            

            HeadSelect.PossibleEquipment.Clear();
            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int)Slot.Head) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(HeadSelect.PossibleEquipment.Add);

            if (HeadSelect.PossibleEquipment.Count > 0)
                HeadSelect.SelectedEquipment = HeadSelect.PossibleEquipment[0];

            BodySelect.PossibleEquipment.Clear();
            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int)Slot.Body) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(BodySelect.PossibleEquipment.Add);

            if (BodySelect.PossibleEquipment.Count > 0)
                BodySelect.SelectedEquipment = BodySelect.PossibleEquipment[0];

            HandsSelect.PossibleEquipment.Clear();
            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int)Slot.Hands) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(HandsSelect.PossibleEquipment.Add);

            if (HandsSelect.PossibleEquipment.Count > 0)
                HandsSelect.SelectedEquipment = HandsSelect.PossibleEquipment[0];

            LegsSelect.PossibleEquipment.Clear();
            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int)Slot.Legs) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(LegsSelect.PossibleEquipment.Add);

            if (LegsSelect.PossibleEquipment.Count > 0)
                LegsSelect.SelectedEquipment = LegsSelect.PossibleEquipment[0];

            FeetSelect.PossibleEquipment.Clear();
            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int)Slot.Feet) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(FeetSelect.PossibleEquipment.Add);

            if (FeetSelect.PossibleEquipment.Count > 0)
                FeetSelect.SelectedEquipment = FeetSelect.PossibleEquipment[0];

            EarsSelect.PossibleEquipment.Clear();
            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int)Slot.Ears) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(EarsSelect.PossibleEquipment.Add);

            if (EarsSelect.PossibleEquipment.Count > 0)
                EarsSelect.SelectedEquipment = EarsSelect.PossibleEquipment[0];

            NeckSelect.PossibleEquipment.Clear();
            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int)Slot.Neck) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(NeckSelect.PossibleEquipment.Add);

            if (NeckSelect.PossibleEquipment.Count > 0)
                NeckSelect.SelectedEquipment = NeckSelect.PossibleEquipment[0];

            WristSelect.PossibleEquipment.Clear();
            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int)Slot.Wrists) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(WristSelect.PossibleEquipment.Add);

            if (WristSelect.PossibleEquipment.Count > 0)
                WristSelect.SelectedEquipment = WristSelect.PossibleEquipment[0];

            RingLSelect.PossibleEquipment.Clear();
            RingRSelect.PossibleEquipment.Clear();
            AllEquipment
                .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int)Slot.FingerL || s.Key == (int)Slot.FingerR) && e.ClassJobCategory.ClassJobs.Contains(_SelectedClassJob))
                .OrderBy(e => e.Name)
                .ToList()
                .ForEach(e => {
                    RingLSelect.PossibleEquipment.Add(e);
                    RingRSelect.PossibleEquipment.Add(e);
                });

            if (RingLSelect.PossibleEquipment.Count > 0) {
                RingLSelect.SelectedEquipment = RingLSelect.PossibleEquipment[0];
                RingRSelect.SelectedEquipment = RingLSelect.PossibleEquipment[0];
            }
        }
    }
}
