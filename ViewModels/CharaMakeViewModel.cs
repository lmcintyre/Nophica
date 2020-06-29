using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Nophica.Annotations;
using static Nophica.Data;
using SaintCoinach.Xiv;

namespace Nophica.ViewModels
{
    class CharaMakeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MainViewModel Parent { get; set; }

        private Tribe[] AllTribes;

        // Note: this does not reflect my irl opinion
        private Sex[] AllSexes = {
            Sex.Male,
            Sex.Female
        };

        public ObservableCollection<Race> Races { get; private set; }
        public ObservableCollection<Tribe> Tribes { get; private set; }
        public ObservableCollection<Sex> Sexes { get; private set; }

        private Race _SelectedRace;

        public Race SelectedRace {
            get => _SelectedRace;
            set {
                _SelectedRace = value;
                UpdateTribes();
                UpdateSexes();
                OnPropertyChanged("SelectedTribe");
                OnPropertyChanged("SelectedSex");
            }
        }

        public Tribe SelectedTribe { get; set; }
        public Sex SelectedSex { get; set; }

        public CharaMakeViewModel(MainViewModel parent)
        {
            Parent = parent;
            Races = new ObservableCollection<Race>();
            Tribes = new ObservableCollection<Tribe>();
            Sexes = new ObservableCollection<Sex>();
            
            Parent.Realm.GameData.GetSheet<Race>().ToList().ForEach(r => {
                if (!r.Masculine.IsEmpty)
                    Races.Add(r);
            });
            AllTribes = Parent.Realm.GameData.GetSheet<Tribe>().ToArray();

            SelectedRace = Races[0];
            OnPropertyChanged("SelectedRace");
        }

        private void UpdateTribes() {
            Tribes.Clear();

            AllTribes
                .Where(t => TribeValidForRace(SelectedRace, t))
                .Select(t => t)
                .ToList()
                .ForEach(Tribes.Add);

            SelectedTribe = Tribes[0];
            OnPropertyChanged("SelectedTribe");
        }

        private void UpdateSexes() {
            Sexes.Clear();

            if (SelectedRace.Masculine == "Hrothgar")
                Sexes.Add(AllSexes[0]);
            else if (SelectedRace.Masculine == "Viera")
                Sexes.Add(AllSexes[1]);
            else {
                Sexes.Add(AllSexes[0]);
                Sexes.Add(AllSexes[1]);
            }

            if (Sexes.Contains(SelectedSex))
                return;

            SelectedSex = Sexes[0];
            OnPropertyChanged("SelectedSex");
        }

        private bool TribeValidForRace(Race r, Tribe t) {
            int rKey = r.Key;
            int tKey = t.Key;

            // Race 4 is Miqo
            // Tribe 7, 8 are Miqo tribes
            // Valid tribes are:
            //    2 * Race
            //   (2 * Race) - 1

            return tKey == rKey * 2 || tKey == (rKey * 2) - 1;
        }
    }
}
