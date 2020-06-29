using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nophica.Properties;
using Nophica.ViewModels;
using SaintCoinach;
using SaintCoinach.Graphics;
using SaintCoinach.Graphics.Viewer;
using SaintCoinach.Graphics.Viewer.Interop;
using SaintCoinach.Imaging;
using SaintCoinach.Xiv;
using SaintCoinach.Xiv.Items;
using File = SaintCoinach.IO.File;
using Material = SaintCoinach.Graphics.Material;

namespace Nophica
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        //TODO: phyb ?
        private string WepSklbPathFormat = "chara/weapon/w{0:D4}/skeleton/base/b{1:D4}/skl_w{0:D4}b{1:D4}.sklb";
        private string WepPapPathFormat = "chara/weapon/w{0:D4}/animation/a0001/wp_common/resident/weapon.pap";

        private string EquipSklbPathFormat = "chara/human/c{0:D4}/skeleton/{1}/m{2:D4}/skl_c{0:D4}m{2:D4}.sklb";
        // unknown if used... going to assume animations for gear bones are inside chara paps and animate only when applicable
        private string EquipPapPathFormat = "";

        string[] DefaultAnimationNames = { "cbbw_id0" };

        private const string GameDirectory = @"C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\";
        private ARealmReversed realm;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            realm = new ARealmReversed(GameDirectory, SaintCoinach.Ex.Language.English);
            // DataContext = new MainViewModel();
            
            WindowPosition pos = WindowPosition.Load();
            if (pos.MainWindowWidth > 0)
                Width = pos.MainWindowWidth;
            if (pos.MainWindowHeight > 0)
                Height = pos.MainWindowHeight;
            if (pos.MainWindowLeft > 0)
                Left = pos.MainWindowLeft;
            if (pos.MainWindowTop > 0)
                Top = pos.MainWindowTop;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            new WindowPosition(Top, Left, Width, Height).Save();
        }

        private void loadButton_Click(object sender, RoutedEventArgs ea)
        {
            Equipment[] AllEquipment = realm.GameData.GetSheet<Item>().OfType<Equipment>().Where(e => !e.EquipSlotCategory.PossibleSlots.Any(s => s.Key == 5 || s.Key == 13)).OrderBy(e => e.Name).ToArray();
            listBox.ItemsSource = AllEquipment;
            listBox.DisplayMemberPath = "Name";
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            Equipment selected = (Equipment) listBox.SelectedItem;

            string papPath = "";
            string sklbPath = "";

            if (TryGetModel(out var model, out var variant)) {
                if (selected.EquipSlotCategory.Key == 1 || selected.EquipSlotCategory.Key == 2 || selected.EquipSlotCategory.Key == 13) {
                    papPath = string.Format(WepPapPathFormat, selected.ModelMain.Value1);
                    sklbPath = string.Format(WepSklbPathFormat, selected.ModelMain.Value1, selected.ModelMain.Value2);
                } else {
                    // sklbPath = string.Format()
                }
            }

            // System.Diagnostics.Debug.WriteLine(string.Format($"Attempting with sklb {sklbPath}"));
            // System.Diagnostics.Debug.WriteLine(string.Format($"Attempting with pap {papPath}"));

            SaintCoinach.IO.File pap, sklb;

            if (realm.Packs.TryGetFile(sklbPath, out sklb) &&
                realm.Packs.TryGetFile(papPath, out pap))
            {
                int b = selected.ModelMain.Value2;
                int m = selected.ModelMain.Value1;
                outputBox.Text += $"{model.File.Path}\n";
                outputBox.Text += $"\t{sklb.Path}\n";
                outputBox.Text += $"\t{pap.Path}\n";

                //Parent.EngineHelper.AddToLast(selected.Name, (e) =>
                //    CreateModel(e, new Skeleton(new SklbFile(sklb)), new PapFile(pap), model, variant.ImcVariant, m, b));
            }
            else {
                outputBox.Text += $"{model.File.Path}\n";
                //Parent.EngineHelper.AddToLast(selected.Name, (e) => new SaintCoinach.Graphics.Viewer.Content.ContentModel(e, variant, model, ModelQuality.High));
            }
        }

        private bool TryGetModel(out ModelDefinition model, out ModelVariantIdentifier variant) {
            Equipment selected = (Equipment) listBox.SelectedItem;

            model = null;
            variant = default(ModelVariantIdentifier);
            if (selected == null)
                return false;

            var charType = selected.GetModelCharacterType();
            if (charType == 0)
                return false;
            try
            {
                model = selected.GetModel(charType, out variant.ImcVariant);
                // if (selected.IsDyeable && SelectedStain != null)
                //     variant.StainKey = SelectedStain.Key;

                var result = (model != null);
                if (!result)
                    System.Windows.MessageBox.Show(string.Format("Unable to find model for {0} (c{1:D4}).", selected.Name, charType), "Model not found", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return result;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(string.Format("Failed to load model for {0} (c{1:D4}):{2}{3}", selected.Name, charType, Environment.NewLine, e), "Read failure", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return false;
            }
        }

        private void exportButton_Click(object sender, RoutedEventArgs ea) {
            testExport1();
        }

        private void testExport3() {
            Model model = null;

            string tmpSkel = "chara/monster/m0595/skeleton/base/b0001/skl_m0595b0001.sklb";
            string tmpPap = "chara/monster/m0595/animation/a0001/bt_common/resident/mount.pap";

            realm.Packs.TryGetFile(tmpSkel, out File sklb);
            realm.Packs.TryGetFile(tmpPap, out File pap);

            List<PapFile> papList = new List<PapFile>();
            List<SklbFile> skeleList = new List<SklbFile>();
            papList.Add(new PapFile(pap));
            skeleList.Add(new SklbFile(sklb));

            FbxExport.ExportFbx("mounttest.fbx", new List<Mesh>(), skeleList, papList);
        }

        private void testExport1() {
            //augmented ironworks bow
            string tmpMdl = "chara/weapon/w0612/obj/body/b0001/model/w0612b0001.mdl";
            string tmpSkel = "chara/weapon/w0612/skeleton/base/b0001/skl_w0612b0001.sklb";
            string tmpPap = "chara/weapon/w0612/animation/a0001/wp_common/resident/weapon.pap";

            realm.Packs.TryGetFile(tmpMdl, out File mdl);
            realm.Packs.TryGetFile(tmpSkel, out File sklb);
            realm.Packs.TryGetFile(tmpPap, out File pap);

            Model model = new Model(new ModelDefinition((ModelFile)mdl), ModelQuality.High);

            List<PapFile> papList = new List<PapFile>();
            List<SklbFile> skeleList = new List<SklbFile>();
            papList.Add(new PapFile(pap));
            skeleList.Add(new SklbFile(sklb));

            // try {
            //     
            // }
            // catch (Exception e) {
            //     outputBox.Text += e.Message + "\n";
            // }

            // FbxExport.ExportFbx("gaiuass.fbx", model.Meshes, skeleList, papList);
        }

        private void testExport2() {

            // string tmpPap = "chara/human/c0101/animation/a0001/bt_2ax_emp/resident/move_a.pap";
            string tmpPap2 = "chara/human/c0701/animation/f0002/nonresident/fear.pap";

            // realm.Packs.TryGetFile(tmpPap, out File pap);
            realm.Packs.TryGetFile(tmpPap2, out File pap2);

            List<PapFile> papList = new List<PapFile>();
            // papList.Add(new PapFile(pap));
            papList.Add(new PapFile(pap2));

            List<SklbFile> skeleList = getSkeles();
            Skeleton forDebugging = new Skeleton(skeleList[0]);

            Mesh[] meshes = getPlayer();
            // int result = FbxExport.ExportFbx("text.fbx", meshes, skeleList, papList);
            // System.Diagnostics.Debug.WriteLine($"Export finished with result {result}");
        }

        private void texButton_Click(object sender, RoutedEventArgs e)
        {
            long stamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string dirname = "tex_" + stamp;

            System.IO.Directory.CreateDirectory(dirname);

            Mesh[] meshes = getPlayer();

            string format = "chara/human/m{0}/obj/body/b{1}/material/v{2:D4}{3}";

            foreach (var mesh in meshes) {
                var thisDefinitionMaterials = mesh.Model.Definition.Materials;
                foreach (var material in thisDefinitionMaterials)
                {
                    string path = material.Name;
                    string m = path.Substring(path.IndexOf("_m") + 2, 4);
                    string b = path.Substring(path.IndexOf("_m") + 7, 4);

                    // Material mat = new Material(material, realm.Packs.GetFile(String.Format(format, m, b, variant.Variant, path)), variant);
                    // foreach (var tex in mat.TexturesFiles)
                    // {
                    //     string texFile = tex.Path.Substring(tex.Path.LastIndexOf('/')).Replace(".tex", ".png");
                    //     string output = folder + '\\' + texFile;
                    //     tex.GetImage().Save(output);
                    // }
                }






                foreach (var img in mesh.Material.Get().TexturesFiles) {
                    String imgName = img.Path;
                    int imgLastSep = imgName.LastIndexOf('/');
                    imgName = imgName.Substring(imgLastSep + 1);
                    imgName = imgName.Replace(".tex", ".png");

                    //Write the image out
                    if (!System.IO.File.Exists(dirname + "/" + imgName))
                        img.GetImage().Save(dirname + "/" + imgName);
                }
            }
        }

        private SklbFile initSkele(string path) {
            realm.Packs.TryGetFile(path, out File sklb);
            SklbFile skl = new SklbFile(sklb);
            return skl;
        }

        private List<SklbFile> getSkeles() {

            List<SklbFile> skeles = new List<SklbFile>();

            skeles.Add(initSkele("chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb"));
            skeles.Add(initSkele("chara/human/c0101/skeleton/face/f0002/skl_c0101f0002.sklb"));
            skeles.Add(initSkele("chara/human/c0101/skeleton/hair/h0106/skl_c0101h0106.sklb"));

            // string tmpSkel = "chara/human/c0201/skeleton/base/b0001/skl_c0201b0001.sklb";
            // 

            return skeles;
        }

        private Mesh[] getPlayer() {

            List<Model> models = new List<Model>();

            //hair
            models.Add(initModel("chara/human/c0701/obj/hair/h0106/model/c0701h0106_hir.mdl"));

            //face
            models.Add(initModel("chara/human/c0701/obj/face/f0003/model/c0701f0003_fac.mdl"));

            //body
            models.Add(initModel("chara/equipment/e0460/model/c0101e0460_top.mdl"));
            // models.Add(initModel("chara/human/c0101/obj/body/b0001/model/c0101b0001_top.mdl"));

            //hands
            models.Add(initModel("chara/human/c0101/obj/body/b0001/model/c0101b0001_glv.mdl"));

            //legs
            models.Add(initModel("chara/human/c0101/obj/body/b0001/model/c0101b0001_dwn.mdl"));

            //feet
            models.Add(initModel("chara/human/c0101/obj/body/b0001/model/c0101b0001_sho.mdl"));

            //tail
            // models.Add(initModel("chara/human/c0701/obj/tail/t0003/model/c0701t0003_til.mdl"));

            //ears (viera only)


            List<Mesh> meshes = new List<Mesh>();

            foreach (Model m in models) {
                foreach (Mesh mesh in m.Meshes) {
                    meshes.Add(mesh);
                    // outputBox.Text += $"{m.Definition.File.Path} Mesh {mesh.Index}:\n";
                    // for (int i = 0; i < 2; i++) {
                    //     
                    //     if (mesh.Vertices[i].BlendIndices.HasValue)
                    //         outputBox.Text += $"{mesh.Vertices[i].BlendIndices.Value}" + "\n";
                    // }
                }
            }

            return meshes.ToArray();
        }

        private Model initModel(string path) {
            //separate statements are better for exceptions :)
            realm.Packs.TryGetFile(path, out File mdl);
            ModelFile mdlFile = (ModelFile) mdl;
            ModelDefinition mdlDef = new ModelDefinition(mdlFile);
            Model model = new Model(mdlDef, ModelQuality.High);
            return model;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs ea) {

            // string tabTitle = ((TabItem) windowTabs.SelectedItem).Header.ToString();
            //
            // if (tabTitle != "Test UI")
            //     return;
            //
            // Equipment[] AllEquipment = realm.GameData.GetSheet<Item>().OfType<Equipment>()
            //             .Where(e => !e.EquipSlotCategory.PossibleSlots.Any(s => s.Key == 5 || s.Key == 13))
            //             .OrderBy(e => e.Name)
            //             .ToArray();
            //
            // ObservableCollection<Equipment> mainhands = new ObservableCollection<Equipment>();
            // // Equipment[] mainhands = new Equipment[0];
            //
            // var jobs = realm.GameData.GetSheet<ClassJob>().ToArray();
            // jobDropDown.ItemsSource = jobs;
            // jobDropDown.DisplayMemberPath = "Name";
            //
            // jobDropDown.SelectionChanged += (o, args) => {
            //     ClassJob cj = ((ClassJob) ((ComboBox) o).SelectedItem);
            //
            //     ImageFile imgFile = cj.Icon;
            //     var tmp = ImageConverter.Convert(imgFile.GetData(), ImageFormat.A8R8G8B8_1, imgFile.Width, imgFile.Height);
            //
            //     using (var ms = new MemoryStream()) {
            //         tmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //         ms.Position = 0;
            //
            //         var bi = new BitmapImage();
            //         bi.BeginInit();
            //         bi.CacheOption = BitmapCacheOption.OnLoad;
            //         bi.StreamSource = ms;
            //         bi.EndInit();
            //
            //         jobIcon.Source = bi;
            //     }
            //
            //     mainhands.Clear();
            //     AllEquipment
            //         .Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == 0) && e.ClassJobCategory.ClassJobs.Contains(cj))
            //         .OrderBy(e => e.Name)
            //         .ToList()
            //         .ForEach(e => mainhands.Add(e));
            //     // mainhandDropDown.ItemsSource = mainhands;
            //     mainhandDropDown.GetBindingExpression(ComboBox.ItemsSourceProperty)?.UpdateTarget();
            // };
            //
            //
            //
            // // Equipment[] mainhands = AllEquipment.Where(e => e.EquipSlotCategory.PossibleSlots.All(s => s.Key == 0)).OrderBy(e => e.Name).ToArray();
            // mainhandDropDown.ItemsSource = mainhands;
            // mainhandDropDown.DisplayMemberPath = "Name";
            //
            // mainhandDropDown.SelectionChanged += (o, args) => {
            //     Equipment eq = ((Equipment) ((ComboBox) o).SelectedItem);
            //     
            //     ImageFile imgFile = eq.Icon;
            //     var tmp = ImageConverter.Convert(imgFile.GetData(), ImageFormat.Dxt1, imgFile.Width, imgFile.Height);
            //
            //     using (var ms = new MemoryStream()) {
            //         tmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //         ms.Position = 0;
            //
            //         var bi = new BitmapImage();
            //         bi.BeginInit();
            //         bi.CacheOption = BitmapCacheOption.OnLoad;
            //         bi.StreamSource = ms;
            //         bi.EndInit();
            //
            //         mainhandIcon.Source = bi;
            //     }
            // };

            /*
             * System.Drawing.Image imgWinForms = System.Drawing.Image.FromFile("test.png");



// ImageSource ...

BitmapImage bi = new BitmapImage();

bi.BeginInit();

MemoryStream ms = new MemoryStream();

// Save to a memory stream...

imgWinForms.Save(ms, ImageFormat.Bmp);

// Rewind the stream...

ms.Seek(0, SeekOrigin.Begin);

// Tell the WPF image to use this stream...

bi.StreamSource = ms;

bi.EndInit();
             */
        }
    }
}
