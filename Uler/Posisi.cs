using System;
using System.Collections.Generic;

namespace Uler
{
    public class Posisi
    {
        public int Row { get;}
        public int Col { get;}
        public Posisi(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public Posisi Translate(Direction dir)
        {
            return new Posisi(Row + dir.RowOffset, Col + dir.ColOffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Posisi posisi &&
                   Row == posisi.Row &&
                   Col == posisi.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Posisi left, Posisi right)
        {
            return EqualityComparer<Posisi>.Default.Equals(left, right);
        }

        public static bool operator !=(Posisi left, Posisi right)
        {
            return !(left == right);
        }
    }
}
