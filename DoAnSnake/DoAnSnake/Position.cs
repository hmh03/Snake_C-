
using System;
//đi
namespace DoAnSnake
{
    public class Position
    {
        public int Row { get; }
        public int Colum { get; }
        public Position(int row, int colum)
        {
            Row = row;
            Colum = colum;
        }
        public Position Translate(Direction dir)
        {
            return new Position(Row+ dir.rowoffset, Colum+ dir.coloffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Colum == position.Colum;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Colum);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }
}
