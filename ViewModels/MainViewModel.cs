using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nophica.Annotations;
using SaintCoinach;
using SaintCoinach.Ex;

namespace Nophica.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //TODO: :knife: :pufferfish:
        private const string GameDirectory = @"C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\";

        public ARealmReversed Realm { get; private set; }
        public EquipmentSelectViewModel EquipmentSelect { get; private set; }
        public ExportViewModel Export { get; private set; }

        public MainViewModel()
        {
            // if (!App.IsValidGamePath(Properties.Settings.Default.GamePath))
            //     return;
            // var realm = new ARealmReversed(Properties.Settings.Default.GamePath, SaintCoinach.Ex.Language.English);
            var realm = new ARealmReversed(GameDirectory, Language.English);
            Initialize(realm);
        }

        void Initialize(ARealmReversed realm)
        {
            realm.Packs.GetPack(new SaintCoinach.IO.PackIdentifier("exd", SaintCoinach.IO.PackIdentifier.DefaultExpansion, 0)).KeepInMemory = true;
            
            Realm = realm;
            EquipmentSelect = new EquipmentSelectViewModel(this);
            Export = new ExportViewModel(this);
        }
    }
}
