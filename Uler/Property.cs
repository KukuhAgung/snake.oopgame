using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Formats.Asn1.AsnWriter;

namespace Uler
{
    public class Property 
    {
        public readonly static ImageSource Kepala = LoadImage("Head.png");
        //public readonly static ImageSource Badan = LoadImage("Body.png");
        //public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource Badan = LoadImage("Body1.png");
        public readonly static ImageSource Empty = LoadImage("Empty1.png");
        public readonly static ImageSource Makanan = LoadImage("Food.png");
       public readonly static ImageSource Shield = LoadImage("Shield.png");
        public readonly static ImageSource KepalaMati = LoadImage("DeadHead.png");
        public readonly static ImageSource BadanMati = LoadImage("DeadBody.png");

        private static ImageSource LoadImage(string namafile)
        {
            return  new BitmapImage(new Uri($"Assets/{namafile}", UriKind.Relative));
        }

    }
}
