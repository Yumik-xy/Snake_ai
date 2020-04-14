using System;
using System.Drawing;

namespace Snake
{
    public enum Type { Food, Body, Head, Tail }
    class Block
    {
        private int size;
        private Point point;
        private Direction old_direction;
        private Direction new_direction;
        private Type type;
        public Block(Point point)
        {
            this.point = point;
        }

        public override bool Equals(object obj)
        {
            return ((Block)obj).Point.X == Point.X && ((Block)obj).Point.Y == Point.Y;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Block(int size, Point point, Type type, Direction old_direction = Direction.Left, Direction new_direction = Direction.Left)
        {
            this.size = size;
            this.point = point;
            this.type = type;
            this.old_direction = old_direction;
            this.new_direction = new_direction;
        }

        public Point Point { get { return point; } set { point = value; } }

        public Type Type { set { type = value; } }


        public Direction Old_direction { get { return old_direction; } }
        public Direction New_direction { set { new_direction = value; } }

        private Image GetImage()
        {
            switch (type)
            {
                case Type.Food:
                    Random random = new Random();
                    switch (random.Next(13))
                    {
                        case 0: return Properties.Resources.Food0;
                        case 1: return Properties.Resources.Food1;
                        case 2: return Properties.Resources.Food2;
                        case 3: return Properties.Resources.Food3;
                        case 4: return Properties.Resources.Food4;
                        case 5: return Properties.Resources.Food5;
                        case 6: return Properties.Resources.Food6;
                        case 7: return Properties.Resources.Food7;
                        case 8: return Properties.Resources.Food8;
                        case 9: return Properties.Resources.Food9;
                        case 10: return Properties.Resources.Food10;
                        case 11: return Properties.Resources.Food11;
                        case 12: return Properties.Resources.Food12;
                        case 13: return Properties.Resources.Food13;
                        default: return null;
                    }
                case Type.Body:
                    if (old_direction == Direction.Left && new_direction == Direction.Left) return Properties.Resources.Body00;
                    else if (old_direction == Direction.Left && new_direction == Direction.Up) return Properties.Resources.Body01;
                    else if (old_direction == Direction.Left && new_direction == Direction.Down) return Properties.Resources.Body03;
                    else if (old_direction == Direction.Up && new_direction == Direction.Left) return Properties.Resources.Body10;
                    else if (old_direction == Direction.Up && new_direction == Direction.Up) return Properties.Resources.Body11;
                    else if (old_direction == Direction.Up && new_direction == Direction.Right) return Properties.Resources.Body12;
                    else if (old_direction == Direction.Right && new_direction == Direction.Up) return Properties.Resources.Body21;
                    else if (old_direction == Direction.Right && new_direction == Direction.Right) return Properties.Resources.Body22;
                    else if (old_direction == Direction.Right && new_direction == Direction.Down) return Properties.Resources.Body23;
                    else if (old_direction == Direction.Down && new_direction == Direction.Left) return Properties.Resources.Body30;
                    else if (old_direction == Direction.Down && new_direction == Direction.Right) return Properties.Resources.Body32;
                    else if (old_direction == Direction.Down && new_direction == Direction.Down) return Properties.Resources.Body33;
                    else return null;
                case Type.Head:
                    switch (old_direction)
                    {
                        case Direction.Left: return Properties.Resources.Head0;
                        case Direction.Up: return Properties.Resources.Head1;
                        case Direction.Right: return Properties.Resources.Head2;
                        case Direction.Down: return Properties.Resources.Head3;
                        default: return null;
                    }
                case Type.Tail:
                    switch (new_direction)
                    {
                        case Direction.Left: return Properties.Resources.Tail0;
                        case Direction.Up: return Properties.Resources.Tail1;
                        case Direction.Right: return Properties.Resources.Tail2;
                        case Direction.Down: return Properties.Resources.Tail3;
                        default: return null;
                    }
                default:
                    return null;
            }

        }

        public virtual void ScanPoint(Graphics graphics)
        {
            lock (graphics)
            {
                try
                {
                    graphics.DrawImage(GetImage(), point.X * size, point.Y * size, size, size);
                }
                catch
                { }
            }
        }
        public virtual void Clear(Graphics graphics)
        {
            SolidBrush solidBrush = new SolidBrush(Color.White);
            lock (graphics)
            {
                try
                {
                    graphics.FillRectangle(solidBrush, point.X * size, point.Y * size, size, size);
                }
                catch
                { }
            }
        }
    }
}
