using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using FFMpegCore.Arguments;
using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace test_ffmpeg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        private BitmapImage? myFrame;
        private Bitmap? myImage;
        

        public BitmapImage? MyFrameProperty
        {
            get { return myFrame; }
            set
            {
                myFrame = value;

            }

        }

        public MainWindow()
        {
            InitializeComponent();
            Unosquare.FFME.Library.FFmpegDirectory = @"O:\WindowsProjects\test_ffmpeg\ffmpeg";
            
           
        }

        private Bitmap? Captured_bitmap { get; set; }

        private async void MediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            await MediaElement.Open(new Uri(@"rtsp://admin:idt12345@192.168.0.160:554/cam/realmonitor?channel=1&subtype=0"));
        }

        private async void btn1_Click(object sender, RoutedEventArgs e)
        {
            Captured_bitmap = await MediaElement.CaptureBitmapAsync();
            MyFrameProperty = BitmaptoImageSource(Captured_bitmap);
            this.btn_image.Source = MyFrameProperty;
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            var imageMat = BitmapConverter.ToMat(Captured_bitmap);
            var imageNew = new Mat();
            imageNew = applyCannyEffect(imageMat);
            myImage = BitmapConverter.ToBitmap(imageNew);
            this.btn_capture.Source = BitmaptoImageSource(myImage);
        }

        Mat applyCannyEffect(Mat image)
        {
            var newImage = new Mat();
            Cv2.Canny(image, newImage, 50, 200);
            return newImage;
        }

        /// to convert system.drawing.bitmap to Imagesource
        /// https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        private BitmapImage BitmaptoImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
