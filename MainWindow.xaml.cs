using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KWEngine_SkyboxGenerator
{
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (sender != null && sender is System.Windows.Controls.Image)
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    //Bitmap
                    BitmapImage bmp = new BitmapImage(new Uri(files[0]));
                    if (bmp != null)
                    {
                        (sender as System.Windows.Controls.Image).Source = bmp;
                    }
                }
                else
                {
                    MessageBox.Show("Multiple file drops not allowed here.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource left = ImageLeft.Source as BitmapSource;
            BitmapSource right = ImageRight.Source as BitmapSource;
            BitmapSource front = ImageFront.Source as BitmapSource;
            BitmapSource back = ImageBack.Source as BitmapSource;
            BitmapSource top = ImageTop.Source as BitmapSource;
            BitmapSource bottom = ImageBottom.Source as BitmapSource;
            bool ok = false;
            int height, width;
            height = left.PixelHeight;
            width = left.PixelWidth;
            if(CheckEqualHeightAndWidth(width, height, right.PixelWidth, right.PixelHeight))
            {
                if (CheckEqualHeightAndWidth(width, height, front.PixelWidth, front.PixelHeight))
                {
                    if (CheckEqualHeightAndWidth(width, height, back.PixelWidth, back.PixelHeight))
                    {
                        if (CheckEqualHeightAndWidth(width, height, top.PixelWidth, top.PixelHeight))
                        {
                            if (CheckEqualHeightAndWidth(width, height, bottom.PixelWidth, bottom.PixelHeight))
                            {
                                ok = true;
                            }
                        }
                    }
                }
            }
            if(ok)
            {
                Bitmap bmp = GenerateUnifiedCubemapFromImages(left, right, front, back, top, bottom);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "png";
                sfd.Filter =
                    "PNG image (*.png)|*.png";
                sfd.InitialDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                bool? result = sfd.ShowDialog();

                if (result.HasValue && result.Value == true)
                {
                    string name = sfd.FileName;
                    bmp.Save(name, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("Cubemap generated successfully!", "Generator result:", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Cubemap could not be generated!", "Generator result:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private bool CheckEqualHeightAndWidth(int width, int height, int newWidth, int newHeight)
        {
            return width == newWidth && height == newHeight;
        }

        private Bitmap GenerateUnifiedCubemapFromImages(BitmapSource left, BitmapSource right, BitmapSource front, BitmapSource back, BitmapSource top, BitmapSource bottom)
        {
            Bitmap b = new Bitmap(left.PixelWidth * 4, left.PixelHeight * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(b);

            Bitmap leftb = ConvertBitmapSourceToBitmap(left);
            Bitmap rightb = ConvertBitmapSourceToBitmap(right);
            Bitmap frontb = ConvertBitmapSourceToBitmap(front);
            Bitmap backb = ConvertBitmapSourceToBitmap(back);
            Bitmap topb = ConvertBitmapSourceToBitmap(top);
            Bitmap bottomb = ConvertBitmapSourceToBitmap(bottom);

            g.DrawImage(leftb, new Rectangle(0,                    left.PixelHeight,     left.PixelWidth, left.PixelHeight));
            g.DrawImage(frontb, new Rectangle(left.PixelWidth,     left.PixelHeight,     left.PixelWidth, left.PixelHeight));
            g.DrawImage(rightb, new Rectangle(left.PixelWidth * 2, left.PixelHeight,     left.PixelWidth, left.PixelHeight));
            g.DrawImage(backb, new Rectangle(left.PixelWidth * 3,  left.PixelHeight,     left.PixelWidth, left.PixelHeight));
            g.DrawImage(topb, new Rectangle(left.PixelWidth,       0,                    left.PixelWidth, left.PixelHeight));
            g.DrawImage(bottomb, new Rectangle(left.PixelWidth,    left.PixelHeight * 2, left.PixelWidth, left.PixelHeight));

            g.Dispose();
            return b;
        }

        private Bitmap ConvertBitmapSourceToBitmap(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }
    }
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
}
