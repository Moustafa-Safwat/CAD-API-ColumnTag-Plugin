using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace WPFCADAPI.Ribbon
{

    public class Ribbon
    {

        static BitmapImage getBitmap(string fileName)

        {

            BitmapImage bmp = new BitmapImage();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block.             

            bmp.BeginInit();

            bmp.UriSource = new Uri(string.Format(

              "pack://application:,,,/{0};component/{1}",

              Assembly.GetExecutingAssembly().GetName().Name,

              fileName));

            bmp.EndInit();



            return bmp;

        }













        public const string RibbonTitle = "Safwat-Addin";
        public const string RibbonId = "10 10";

        [CommandMethod("safwat")]
        public void CreateRibbon()
        {
            RibbonControl ribbon = ComponentManager.Ribbon;
            if (ribbon != null)
            {
                RibbonTab rtab = ribbon.FindTab(RibbonId);
                if (rtab != null)
                {
                    ribbon.Tabs.Remove(rtab);
                }

                rtab = new RibbonTab();
                rtab.Title = RibbonTitle;
                rtab.Id = RibbonId;
                ribbon.Tabs.Add(rtab);
                AddContentToTab(rtab);
            }
        }

        private void AddContentToTab(RibbonTab rtab)
        {
            rtab.Panels.Add(AddPanelOne());
        }

        private static RibbonPanel AddPanelOne()
        {
            RibbonPanelSource rps = new RibbonPanelSource();
            rps.Title = "Text Annotation";
            RibbonPanel rp = new RibbonPanel();
            rp.Source = rps;
            RibbonButton rci = new RibbonButton();
            rci.Name = "Moustafa Sawfat Addin";
            rps.DialogLauncher = rci;
            //create button1
            var addinAssembly = typeof(Ribbon).Assembly;
            RibbonButton btnPythonShell = new RibbonButton
            //btnPythonShell.ShowImage = true;
            //btnPythonShell.Image = Resource1.icons8_autocad_48.ToBitmapImage();
            ////btnPythonShell.LargeImage = Resource1.icons8_autocad_48.ToBitmapImage();
            ////btnPythonShell.LargeImage= getBitmap(@"Images\coltagicon.png");
            //btnPythonShell.ShowText = false;
            //btnPythonShell.Text = "Column Tag";
            //btnPythonShell.CommandHandler = new RelayCommand(new WPFCADAPI.ViewModel.CadLayers().Execute);
            //btnPythonShell.Text=

            {
                Orientation = Orientation.Vertical,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Large,
                Name = "Safwat-Addin",
                ShowText = true,
                Text = "Column Tag",
                Description = "This Addin is made to Tag The Columns  (BY : Moustafa Safwat)",
                ShowImage = true,
                LargeImage = Resource1.fotor_2023_1_27_22_40_39.ToBitmapImage(),
                CommandHandler = new RelayCommand(new WPFCADAPI.ViewModel.CadLayers().Execute),


            };


            rps.Items.Add(btnPythonShell);


            return rp;
        }
        public static System.Windows.Media.ImageSource GetEmbeddedPng(System.Reflection.Assembly app, string imageName)
        {
            var file = app.GetManifestResourceStream(imageName);
            BitmapDecoder source = PngBitmapDecoder.Create(file, BitmapCreateOptions.None, BitmapCacheOption.None);
            return source.Frames[0];
        }


    }
    public static class BitmapExtensions
    {
        public static BitmapImage ToBitmapImage(this Bitmap image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }
    }
}
