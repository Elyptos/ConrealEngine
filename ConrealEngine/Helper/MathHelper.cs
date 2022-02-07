/*
	Copyright (c) 2017 Thomas Schöngrundner

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConUI.Helper
{
    public static class MathHelper
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
                return min;
            else if (val.CompareTo(max) > 0)
                return max;
            else
                return val;
        }

        public static int Get1DIndexFrom2D(IntVector2 index, int width, int height)
        {
            if (index.X >= width || index.Y >= height)
                return -1;

            return index.X + width * index.Y;
        }

        public static IntVector2 Get2DIndexFrom1D(int index, int width, int height)
        {
            if(index < 0 || index >= (width * height))
            {
                return new IntVector2(-1, -1);
            }
            else
            {
                IntVector2 res = new IntVector2();

                res.X = index % width;
                res.Y = index / width;

                return res;
            }
        }

        public static int Lerp(int start, int end, float alpha)
        {
            return (int)((end - start) * alpha) + start;
        }

        public static float Lerp(float start, float end, float alpha)
        {
            return ((end - start) * alpha) + start;
        }

        public static IntVector2 Lerp(IntVector2 start, IntVector2 end, float alpha)
        {
            return new IntVector2(Lerp(start.X, end.X, alpha), Lerp(start.Y, end.Y, alpha));
        }
    }

    public struct Box
    {
        public IntVector2 CornerTL { get; set; }
        public IntVector2 CornerTR { get; set; }
        public IntVector2 CornerBR { get; set; }
        public IntVector2 CornerBL { get; set; }

        public int Width { get { return CornerBR.X - CornerTL.X + 1; } }
        public int Height { get { return CornerBR.Y - CornerTL.Y + 1; } }

        public Box(IntVector2 tlPos, int width, int height)
        {
            CornerTL = tlPos;
            CornerTR = new IntVector2(tlPos.X + width - 1, tlPos.Y);
            CornerBR = new IntVector2(tlPos.X + width - 1, tlPos.Y + height - 1);
            CornerBL = new IntVector2(tlPos.X, tlPos.Y + height - 1);
        }

        public bool IsInside(IntVector2 pos)
        {
            return IsValid() && pos.X >= CornerTL.X && pos.X <= CornerBR.X && pos.Y >= CornerTL.Y && pos.Y <= CornerBR.Y;
        }

        public bool IsInside(Box b)
        {
            return IsInside(b.CornerTL) && IsInside(b.CornerTR) && IsInside(b.CornerBL) && IsInside(b.CornerBR);
        }

        public bool IsValid()
        {
            IntVector2 diff = CornerBR - CornerTL;

            return diff.X >= 0 && diff.Y >= 0;
        }
    }

    public struct IntVector2
    {
        public IntVector2(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public static IntVector2 operator +(IntVector2 v1, IntVector2 v2)
        {
            return new IntVector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static IntVector2 operator +(IntVector2 v1, int scalar)
        {
            return new IntVector2(v1.X + scalar, v1.Y + scalar);
        }

        public static IntVector2 operator -(IntVector2 v1, IntVector2 v2)
        {
            return new IntVector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static IntVector2 operator -(IntVector2 v1, int scalar)
        {
            return new IntVector2(v1.X - scalar, v1.Y - scalar);
        }

        public static IntVector2 operator *(IntVector2 v1, IntVector2 v2)
        {
            return new IntVector2(v1.X * v2.X, v1.Y * v2.Y);
        }

        public static IntVector2 operator *(IntVector2 v1, Vector2 v2)
        {
            return new IntVector2((int)(v1.X * v2.X), (int)(v1.Y * v2.Y));
        }

        public static IntVector2 operator *(IntVector2 v1, int scalar)
        {
            return new IntVector2(v1.X * scalar, v1.Y * scalar);
        }

        public static IntVector2 operator *(IntVector2 v1, float scalar)
        {
            return new IntVector2((int)(v1.X * scalar), (int)(v1.Y * scalar));
        }

        public static IntVector2 operator /(IntVector2 v1, IntVector2 v2)
        {
            return new IntVector2(v1.X / v2.X, v1.Y / v2.Y);
        }

        public static IntVector2 operator /(IntVector2 v1, int scalar)
        {
            return new IntVector2(v1.X / scalar, v1.Y / scalar);
        }

        public static bool operator ==(IntVector2 v1, IntVector2 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(IntVector2 v1, IntVector2 v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }

        public Vector2 ToFloatVector()
        {
            return new Vector2((int)X, (int)Y);
        }

        public override bool Equals(Object other)
        {
            if (other == null)
                return false;

            if(other is IntVector2)
            {
                IntVector2 otherVec = (IntVector2)other;

                return this == otherVec;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }
    }
}
