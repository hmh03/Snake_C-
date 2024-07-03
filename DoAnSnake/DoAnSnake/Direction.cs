
// hướng đường đi
using System;
namespace DoAnSnake
{
    public class Direction
    {
        public readonly static Direction Up = new Direction(-1, 0);
        public readonly static Direction Down = new Direction(1, 0);
        public readonly static Direction Left = new Direction(0, -1);
        public readonly static Direction Right = new Direction(0, 1);

        public int rowoffset { get; }
        public int coloffset { get; }
        private Direction(int rowoffset, int coloffset)
        {
            this.rowoffset = rowoffset;
            this.coloffset = coloffset;
        }
        public Direction OppoSite()
        {
            return new Direction(-rowoffset, -coloffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Direction direction &&
                   rowoffset == direction.rowoffset &&
                   coloffset == direction.coloffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(rowoffset, coloffset);
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }
    }
}
