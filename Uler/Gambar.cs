using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Uler
{
    public class Gambar
    {
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource Badan = LoadImage("Body.png");
        public readonly static ImageSource Kepala = LoadImage("Head.png");
        public readonly static ImageSource Makanan = LoadImage("Food.png");
        public readonly static ImageSource BadanMati = LoadImage("DeadBody.png");
        public readonly static ImageSource KepalaMati = LoadImage("DeadHead.png");
        public readonly static ImageSource Shield = LoadImage("shield.png");

        private static ImageSource LoadImage(string namafile)
        {
            return  new BitmapImage(new Uri($"Assets/{namafile}", UriKind.Relative));
        }
    }
}
