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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static ConrealEngine.Assets.UnicodeImage;

namespace ConrealEngine.Assets
{
    public class UnicodeFont
    {
        //private const int CHAR_SIZE_PIXEL = 16;

        private string _fontName = "";
        //private int charCountX = 0;
        //private int charCountY = 0;
        private GlyphTypeface fontFile = null;

        public string Font
        {
            get
            {
                return _fontName;
            }
            set
            {
                _fontName = value;

                Typeface face = new Typeface(_fontName);

                face.TryGetGlyphTypeface(out fontFile);

                //if (ResourceHandler.Instance.FontFiles.ContainsKey(value))
                //{
                //    fontFile = ResourceHandler.Instance.FontFiles[value];
                //    //charCountX = fontFile.Width / CHAR_SIZE_PIXEL;
                //    //charCountY = fontFile.Height / CHAR_SIZE_PIXEL;
                //}
            }
        }

        public IntVector2 CellSize { get { return fontFile != null ? new IntVector2(Size, (int)(Size * fontFile.Height)) : new IntVector2(); } }

        public int Size { get; set; } = 5;

        public UnicodeFont()
        {
            Font = "Consolas";
        }

        public char[,] Sample(char c)
        {
            return Sample((int)c);
        }

        //public char[,] Sample(int index)
        //{
        //    int charSize = CharSize;

        //    char[,] res = new char[charSize, charSize];

        //    Bitmap bitmap = GetBitmap(index);

        //    for (int x = 0; x < res.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < res.GetLength(1); y++)
        //        {
        //            Color pixel = bitmap.GetPixel(x, y);

        //            if (pixel.A != 0)
        //            {
        //                res[x, y] = '█';
        //            }
        //            else
        //            {
        //                res[x, y] = '\0';
        //            }
        //        }
        //    }

        //    return res;
        //}

        public char[,] Sample(int index)
        {
            //RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)geom.Bounds.Size.Width, (int)geom.Bounds.Size.Height, 72, 72, PixelFormats.Pbgra32);

            if (fontFile == null)
                return new char[,] { { '\0' } };

            ushort glyphIndex = 0;

            if (!fontFile.CharacterToGlyphMap.TryGetValue(index, out glyphIndex))
                return new char[,] { { '\0' } };

            Geometry geom = fontFile.GetGlyphOutline(glyphIndex, Size, 1);

            if(geom.IsEmpty())
                return new char[,] { { '\0' } };

            DrawingVisual viz = new DrawingVisual();
            using (DrawingContext dc = viz.RenderOpen())
            {
                double yOffset = Size * fontFile.Baseline;
                double xOffset = Size / 2d - geom.Bounds.Width / 2d;

                dc.PushTransform(new TranslateTransform(xOffset, yOffset));

                dc.DrawGeometry(System.Windows.Media.Brushes.Red, null, geom);
            }

            RenderTargetBitmap renderTarget = new RenderTargetBitmap(CellSize.X, CellSize.Y, 100, 100, PixelFormats.Pbgra32);

            renderTarget.Render(viz);

            MemoryStream stream = new MemoryStream();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));
            encoder.Save(stream);

            Bitmap bitmap = new Bitmap(stream);

            char[,] res = new char[bitmap.Width, bitmap.Height];

            for (int x = 0; x < res.GetLength(0); x++)
            {
                for (int y = 0; y < res.GetLength(1); y++)
                {
                    System.Drawing.Color pixel = bitmap.GetPixel(x, y);

                    if (pixel.A != 0)
                    {
                        res[x, y] = '█';
                    }
                    else
                    {
                        res[x, y] = '\0';
                    }
                }
            }

            return res;
        }

        //private Bitmap GetBitmap(int index)
        //{
        //    IntVector2 fontIndex = MathHelper.Get2DIndexFrom1D(index, charCountX, charCountY);
        //    int charSize = CharSize;

        //    Bitmap res = new Bitmap(charSize, charSize);

        //    if (fontIndex.X == -1 || fontFile == null)
        //        return res;

        //    Rectangle destRect = new Rectangle(0, 0, charSize, charSize);

        //    //res.SetResolution(fontFile.HorizontalResolution, fontFile.VerticalResolution);
        //    //res.SetResolution(200, 200);

        //    Graphics graphics = Graphics.FromImage(res);

        //    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        //    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        //    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
        //    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //    graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

        //    ImageAttributes wrapMode = new ImageAttributes();

        //    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.Clamp);
        //    graphics.DrawImage(fontFile, destRect, fontIndex.X * CHAR_SIZE_PIXEL, fontIndex.Y * CHAR_SIZE_PIXEL, CHAR_SIZE_PIXEL, CHAR_SIZE_PIXEL, GraphicsUnit.Pixel, wrapMode);

        //    return res;
        //}
    }
}
