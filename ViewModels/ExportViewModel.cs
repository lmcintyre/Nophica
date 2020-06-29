using Nophica.Annotations;
using Nophica.Util;
using SaintCoinach.Graphics;
using SaintCoinach.Graphics.Viewer.Interop;
using SaintCoinach.IO;
using SaintCoinach.Xiv;
using SaintCoinach.Xiv.Items;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using static Nophica.Data;

namespace Nophica.ViewModels
{
    struct Exportable {

        public Race Race;
        public Tribe Tribe;
        public Sex Sex;

        public byte Customize; // [26]

        public Equipment Mainhand;
        public Equipment Head;
        public Equipment Body;
        public Equipment Hands;
        public Equipment Legs;
        public Equipment Feet;

        public Equipment Offhand;
        public Equipment Ears;
        public Equipment Neck;
        public Equipment Wrist;
        public Equipment RingLeft;
        public Equipment RingRight;

        
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

            // System.Diagnostics.Debug.WriteLine(Parent.EquipmentSelect.MainhandSelect.SelectedEquipment);

            // not needed for now
            //CharaMakeTypeStruct[] sheet = Data.LoadCharaMakeType(Parent.Realm);

            Quad bbase = new Quad(0);
            Exportable ex = GetExportableData();
            var meshes = new List<Mesh>();
            var skeletons = new List<SklbFile>();
            var paps = new List<PapFile>();

            //ignore mainhand/offhand

            //face
            // var faceMdl = PathFormatter.Instance.GetFaceModelPath(ex.Tribe, ex.Sex, 1);

            // string faceSkele = "";
            int what = 1;
            // while (faceSkele == "")
            //     faceSkele = PathFormatter.Instance.GetFaceSkeletonPath(ex.Tribe, ex.Sex, what++);

            //body
            // string bodyMdl = "";
            // what = 1;
            // while (bodyMdl == "")
            //     bodyMdl = PathFormatter.Instance.GetBodyModelPath(ex.Tribe, ex.Sex, what++);

            var bodySkele = PathFormatter.Instance.GetBodySkeletonPath(ex.Tribe, ex.Sex);

            //hair
            // var hairMdl = PathFormatter.Instance.GetHairModelPath(ex.Tribe, ex.Sex, 1);
            // var hairSkele = PathFormatter.Instance.GetHairSkeletonPath(ex.Tribe, ex.Sex, 1);

            //equipment

            //no mainhand, met
            // Quad shisTest = new Quad { Value1 = 36, Value2 = 1 };

            // var topMesh = InitEquipMesh(ex.Tribe, ex.Sex, shisTest, EquipSlotKey.Body);
            // var glvMesh = InitEquipMesh(ex.Tribe, ex.Sex, shisTest, EquipSlotKey.Hands);
            // var dwnMesh = InitEquipMesh(ex.Tribe, ex.Sex, shisTest, EquipSlotKey.Legs);
            // var shoMesh = InitEquipMesh(ex.Tribe, ex.Sex, shisTest, EquipSlotKey.Feet);

            // var topMesh = InitEquipMesh(ex.Tribe, ex.Sex, Quad.Zero, EquipSlotKey.Body);
            // var glvMesh = InitEquipMesh(ex.Tribe, ex.Sex, Quad.Zero, EquipSlotKey.Hands);
            // var dwnMesh = InitEquipMesh(ex.Tribe, ex.Sex, Quad.Zero, EquipSlotKey.Legs);
            // var shoMesh = InitEquipMesh(ex.Tribe, ex.Sex, Quad.Zero, EquipSlotKey.Feet);

            var topMesh = InitEquipMesh(ex.Tribe, ex.Sex, ex.Body);
            var dwnMesh = InitEquipMesh(ex.Tribe, ex.Sex, ex.Legs);

            // var topMesh = InitEquipMesh(ex.Tribe, ex.Sex, ex.Body);
            // Quad gaiuass = new Quad {Value1 = 9063, Value2 = 1};
            // var topMesh = InitEquipMesh(ex.Tribe, ex.Sex, gaiuass, EquipSlotKey.BodyNoHeadHandsLegsFeet);
            // var topMesh = InitNonVariantMesh("chara/equipment/e9063/model/c0501e9063_top.mdl");
            // var topMesh = InitEquipMesh(ex.Tribe, ex.Sex, ex.Body);
            // var glvMesh = InitEquipMesh(ex.Tribe, ex.Sex, ex.Hands);
            // var dwnMesh = InitEquipMesh(ex.Tribe, ex.Sex, ex.Legs);
            // var shoMesh = InitEquipMesh(ex.Tribe, ex.Sex, ex.Feet);

            // no pap
            // paps.Add(InitPap("chara/human/c0201/animation/a0001/bt_common/resident/move_a.pap"));
            // paps.Add(InitPap("chara/human/c0201/animation/a0001/bt_common/resident/idle.pap"));
            // paps.Add(InitPap("chara/human/c0501/animation/a0001/bt_common/resident/move_a.pap"));

            // meshes.Add(InitNonVariantMesh(faceMdl));
            // meshes.Add(InitNonVariantMesh(bodyMdl));
            // meshes.Add(InitNonVariantMesh(hairMdl));

            // meshes.Add(metMesh);
            meshes.Add(topMesh);
            // meshes.Add(glvMesh);
            meshes.Add(dwnMesh);
            // meshes.Add(shoMesh);

            skeletons.Add(InitSklb(bodySkele));
            // skeletons.Add(InitSklb(faceSkele));
            // skeletons.Add(InitSklb(hairSkele));

            int result = FbxExport.ExportFbx("testnew.fbx", meshes, skeletons, paps);
            MessageBox.Show($"Export ended with result {result}", "Export", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private Mesh InitNonVariantMesh(string path) {
            ModelFile mf = (ModelFile) Parent.Realm.Packs.GetFile(path);
            Model model = mf.GetModelDefinition().GetModel(ModelQuality.High);
            Mesh ret = LoadMesh(model);
            return ret;
        }

        private Mesh InitEquipMesh(Tribe t, Sex s, Equipment e) {
            return InitEquipMesh(t, s, e.ModelMain, (EquipSlotKey) e.EquipSlotCategory.Key);
        }

        private Mesh InitEquipMesh(Tribe t, Sex s, Quad q, EquipSlotKey slotKey) {
            var path = PathFormatter.Instance.GetEquipmentModelPath(t, s, q, slotKey);

            ModelFile mf = (ModelFile)Parent.Realm.Packs.GetFile(path);
            Model model = mf.GetModelDefinition().GetModel(ModelQuality.High);

            ImcVariant variant = PathFormatter.Instance.GetVariant(q, slotKey);

            Mesh ret = LoadMeshWithVariant(model, variant);
            return ret;
        }

        private SklbFile InitSklb(string path) {
            File file = Parent.Realm.Packs.GetFile(path);
            return new SklbFile(file);
        }

        private PapFile InitPap(string path) {
            File file = Parent.Realm.Packs.GetFile(path);
            return new PapFile(file);
        }

        private Exportable GetExportableData() {
            Exportable ex = new Exportable();

            CharaMakeViewModel cm = Parent.CharaMake;
            EquipmentSelectViewModel es = Parent.EquipmentSelect;

            ex.Race = cm.SelectedRace;
            ex.Tribe = cm.SelectedTribe;
            ex.Sex = cm.SelectedSex;

            ex.Mainhand = es.MainhandSelect.SelectedEquipment;
            ex.Head = es.HeadSelect.SelectedEquipment;
            ex.Body = es.BodySelect.SelectedEquipment;
            ex.Hands = es.HandsSelect.SelectedEquipment;
            ex.Legs = es.LegsSelect.SelectedEquipment;
            ex.Feet = es.FeetSelect.SelectedEquipment;
            
            ex.Offhand = es.OffhandSelect.SelectedEquipment;
            ex.Ears = es.EarsSelect.SelectedEquipment;
            ex.Neck = es.NeckSelect.SelectedEquipment;
            ex.Wrist = es.WristSelect.SelectedEquipment;
            ex.RingLeft = es.RingLSelect.SelectedEquipment;
            ex.RingRight = es.RingRSelect.SelectedEquipment;

            return ex;
        }
    }
}
