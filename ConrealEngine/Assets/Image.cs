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

using ConUI.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConrealEngine.Assets
{
    public class UnicodeImage
    {
        public struct ImagePixel
        {
            public char Char { get; set; }
            public ConsoleColor Color { get; set; }
        }

        public Bitmap Image { get; set; }

        public UnicodeImage(Bitmap i)
        {
            Image = i;
        }

        public ImagePixel[,] Sample(IntVector2 charCounts)
        {
            if (Image == null)
                return null;

            ImagePixel[,] res = new ImagePixel[charCounts.X, charCounts.Y];

            Bitmap bitmap = GetBitmap(charCounts.X, charCounts.Y);

            if (bitmap == null)
                return null;

            for(int x = 0; x < res.GetLength(0); x++)
            {
                for(int y = 0; y < res .GetLength(1); y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);

                    if(pixel.A != 0)
                    {
                        res[x, y] = new ImagePixel() { Char = '█', Color = FromColor(pixel) };
                    }
                    else
                    {
                        res[x, y] = new ImagePixel() { Char = '\0', Color = FromColor(pixel) };
                    }
                }
            }

            return res;
        }

        private ConsoleColor FromColor(Color c)
        {
            int color = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0;

            color |= (c.R > 64) ? 4 : 0;
            color |= (c.G > 64) ? 2 : 0;
            color |= (c.B > 64) ? 1 : 0;

            return (ConsoleColor)color;
        }

        private Bitmap GetBitmap(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return null;

            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap res = new Bitmap(width, height);

            res.SetResolution(Image.HorizontalResolution, Image.VerticalResolution);

            Graphics graphics = Graphics.FromImage(res);

            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            ImageAttributes wrapMode = new ImageAttributes();

            wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
            graphics.DrawImage(Image, destRect, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, wrapMode);

            return res;
        }
    }
}
