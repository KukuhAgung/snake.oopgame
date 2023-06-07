using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Uler
{
    public class Gamestate
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public Backgroundgrid[,] Grid { get; set; }
        public Direction Dir { get; set; }
        public int score { get; set; }
        public bool gameover { get; set; }


        private readonly LinkedList<Posisi> Posisiuler = new LinkedList<Posisi>();
        private readonly Random random = new Random();

        public void Uler()
        {
            int r = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = Backgroundgrid.Uler;
                Posisiuler.AddFirst(new Posisi(r, c));
            }
        }

        private IEnumerable<Posisi> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == Backgroundgrid.Empty)
                    {
                        yield return new Posisi(r, c);
                    }
                }
            }
        }

        public void TambahMakanan()
        {
            List<Posisi> empty = new List<Posisi>(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Posisi pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = Backgroundgrid.Makanan;
        }


        public Posisi PosisiKepala()
        {
            return Posisiuler.First.Value;
        }

        public Posisi PosisiEkor()
        {
            return Posisiuler.Last.Value;
        }

        public IEnumerable<Posisi> posisiuler()
        {
            return Posisiuler;
        }

        public void TambahKepala(Posisi pos)
        {
            Posisiuler.AddFirst(pos);
            Grid[pos.Row, pos.Col] = Backgroundgrid.Uler;
        }
        public void HapusEkor()
        {
            Posisi ekor = Posisiuler.Last.Value;
            Grid[ekor.Row, ekor.Col] = Backgroundgrid.Empty;
            Posisiuler.RemoveLast();
        }

    }
}
