using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using System.Windows.Controls;
using System.Windows;

namespace Uler
{
    public class Mekanisme : Gamestate
    {
        private readonly LinkedList<Direction> ubaharah = new LinkedList<Direction>();

        public Mekanisme(int rows, int cols)
        {

            Rows = rows;
            Cols = cols;
            Grid = new Backgroundgrid[rows, cols];
            Dir = Direction.Right;

            Uler();
            TambahMakanan();

        }
        private Direction GetLastDirection()
        {
            if (ubaharah.Count == 0)
            {
                return Dir;
            }

            return ubaharah.Last.Value;
        }

        private bool Inginubaharah(Direction arahbaru)
        {
            if (ubaharah.Count == 2)
            {
                return false;
            }

            Direction lastDir = GetLastDirection();
            return arahbaru != lastDir && arahbaru != lastDir.Opposite();
        }

        public void UbahDirection(Direction dir)
        {
            if (Inginubaharah(dir))
            {
                ubaharah.AddLast(dir);
            }
        }

        private bool OutsideGrid(Posisi pos)
        {
            return pos.Row < 0 || pos.Row > Rows - 1 || pos.Col < 0 || pos.Col > Cols - 1;
        }

        private Backgroundgrid WillHit(Posisi poskepala)
        {
            if (OutsideGrid(poskepala))
            {
                return Backgroundgrid.Outside;
            }

            if (poskepala == PosisiEkor())
            {
                return Backgroundgrid.Empty;
            }
            return Grid[poskepala.Row, poskepala.Col];
        }


        public void Gerak()
        {
            if (ubaharah.Count > 0)
            {
                Dir = ubaharah.First.Value;
                ubaharah.RemoveFirst();
            }

            Posisi poskepala = PosisiKepala().Translate(Dir);
            Backgroundgrid hit = WillHit(poskepala);

            if (hit == Backgroundgrid.Outside || hit == Backgroundgrid.Uler)
            {
                gameover = true;
            }
            else if (hit == Backgroundgrid.Empty)
            {
                HapusEkor();
                TambahKepala(poskepala);
            }
            else if (hit == Backgroundgrid.Makanan)
            {
                TambahKepala(poskepala);
                score++;
                TambahMakanan();
            }
        }
    }
}
