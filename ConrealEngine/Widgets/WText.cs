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
using System.Text;
using System.Threading.Tasks;
using ConrealEngine;
using static ConrealEngine.Renderer;
using System.Numerics;
using ConUI.Helper;
using ConrealEngine.Assets;

namespace ConUI.Widgets
{
    public class WText : WWidget
    {
        public enum ETextAlignment
        {
            ETEXT_ALIGNMENT_LEFT = 0,
            ETEXT_ALIGNMENT_CENTER = 1,
        }

        public enum EFontSampler
        {
            FONT_SAMPLER_CONSOLE = 0,
            FONT_SAMPLER_CONREAL = 1
        }

        private List<char[,]> sampledText = new List<char[,]>();
        private string _text = "";
        private EFontSampler _fontSampler = EFontSampler.FONT_SAMPLER_CONSOLE;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;

                if(String.IsNullOrWhiteSpace(_text) || FontSampler == EFontSampler.FONT_SAMPLER_CONSOLE)
                {
                    sampledText.Clear();
                }
                else
                {
                    SampleText();
                }
            }
        }
        public UnicodeFont Font { get; set; } = new UnicodeFont();
        public ETextAlignment TextAlignment { get; set; } = ETextAlignment.ETEXT_ALIGNMENT_LEFT;
        public EFontSampler FontSampler
        {
            get { return _fontSampler; }
            set
            {
                _fontSampler = value;

                if(_fontSampler == EFontSampler.FONT_SAMPLER_CONREAL)
                {
                    SampleText();
                }
                else
                {
                    sampledText.Clear();
                }
            }
        }

        public bool AutoLine { get; set; } = false;

        public WText SetFontSampler(EFontSampler sampler)
        {
            this.FontSampler = sampler;
            return this;
        }

        public WText SetFontSize(int size)
        {
            Font.Size = size;
            SampleText();

            return this;
        }

        public WText SetFont(string name)
        {
            Font.Font = name;
            SampleText();

            return this;
        }

        public WText SetAutoLine(bool b)
        {
            AutoLine = b;
            return this;
        }

        public WText SetTextAlignment(ETextAlignment alignment)
        {
            this.TextAlignment = alignment;
            return this;
        }

        public WText SetText(string txt)
        {
            this.Text = txt;
            return this;
        }

        public WText SetForegroundColor(ConsoleColor c)
        {
            this.ForegroundColor = c;
            return this;
        }

        public WText SetBackgroundColor(ConsoleColor c)
        {
            this.BackgroundColor = c;
            return this;
        }

        public WText DrawBorder(EBorderStyle border)
        {
            this.Border = border;
            return this;
        }

        public override void OnConstruction()
        {
            base.OnConstruction();
        }

        protected override void OnDraw(RenderHandle r, Box drawingArea)
        {
            Box widgetArea = GetDrawingArea(drawingArea);

            IntVector2 centerPos = LayoutSlot.CalculatedAbsolutePosition + ((LayoutSlot.CalculatedAbsolutePosition + LayoutSlot.CalculatedSize - new IntVector2(1, 1)) - LayoutSlot.CalculatedAbsolutePosition) / 2;

            List<string> lines = AutoLine ? ConstructLines(widgetArea) : new List<string>() { Text };

            IntVector2 cellSize = Font.CellSize;

            //Normal text formating
            switch(TextAlignment)
            {
                case ETextAlignment.ETEXT_ALIGNMENT_LEFT:

                    if(FontSampler == EFontSampler.FONT_SAMPLER_CONREAL)
                    {
                        for(int i = 0; i < sampledText.Count; i++)
                        {
                            if(sampledText[i].GetLength(0) > 1 || sampledText[i].GetLength(1) > 1 || sampledText[i][0,0] != '\0')
                            {
                                IntVector2 nPos = new IntVector2((LayoutSlot.CalculatedAbsolutePosition.X + LayoutSlot.Margin.X) + i * cellSize.X, LayoutSlot.CalculatedAbsolutePosition.Y + LayoutSlot.Margin.Y);

                                Box charBounds = new Box(nPos, Font.CellSize.X, Font.CellSize.Y);

                                if (widgetArea.IsInside(charBounds))
                                    r.DrawMany(sampledText[i], nPos, ForegroundColor, BackgroundColor);
                            }
                        }
                    }
                    else
                    {
                        for (int y = 0; y < lines.Count; y++)
                        {
                            for (int i = 0; i < lines[y].Length; i++)
                            {
                                IntVector2 nPos = new IntVector2((LayoutSlot.CalculatedAbsolutePosition.X + LayoutSlot.Margin.X) + i, LayoutSlot.CalculatedAbsolutePosition.Y + y + LayoutSlot.Margin.Y);

                                if (widgetArea.IsInside(nPos))
                                    r.Draw(lines[y][i], nPos, ForegroundColor, BackgroundColor);
                            }
                        }
                    }

                    break;
                case ETextAlignment.ETEXT_ALIGNMENT_CENTER:
                    if(FontSampler == EFontSampler.FONT_SAMPLER_CONREAL)
                    {
                        int centerChar = (sampledText.Count / 2) * Font.CellSize.X;
                        IntVector2 centerCharPos = LayoutSlot.CalculatedAbsolutePosition + centerChar;
                        IntVector2 diff = centerPos - centerCharPos;

                        for (int i = 0; i < sampledText.Count; i++)
                        { 
                            if (sampledText[i].GetLength(0) > 1 || sampledText[i].GetLength(1) > 1 || sampledText[i][0, 0] != '\0')
                            {
                                IntVector2 nPos = new IntVector2((LayoutSlot.CalculatedAbsolutePosition.X + LayoutSlot.Margin.X) + i * cellSize.X + diff.X, LayoutSlot.CalculatedAbsolutePosition.Y + LayoutSlot.Margin.Y);

                                Box charBounds = new Box(nPos, Font.CellSize.X, Font.CellSize.Y);

                                if (widgetArea.IsInside(charBounds))
                                    r.DrawMany(sampledText[i], nPos, ForegroundColor, BackgroundColor);
                            }
                        }
                    }
                    else
                    {
                        for (int y = 0; y < lines.Count; y++)
                        {
                            int centerChar = lines[y].Length / 2;
                            IntVector2 centerCharPos = LayoutSlot.CalculatedAbsolutePosition + centerChar;
                            IntVector2 diff = centerPos - centerCharPos;

                            for (int i = 0; i < lines[y].Length; i++)
                            {
                                IntVector2 nPos = new IntVector2((LayoutSlot.CalculatedAbsolutePosition.X + LayoutSlot.Margin.X) + i + diff.X, LayoutSlot.CalculatedAbsolutePosition.Y + y + LayoutSlot.Margin.Y);

                                if (widgetArea.IsInside(nPos))
                                    r.Draw(lines[y][i], nPos, ForegroundColor, BackgroundColor);
                            }
                        }
                    }

                    break;
            }

            base.OnDraw(r, drawingArea);
        }

        private void SampleText()
        {
            sampledText.Clear();

            foreach(char c in Text)
            {
                sampledText.Add(Font.Sample(c));
            }
        }

        private List<string> ConstructLines(Box drawingArea)
        {
            int maxCharsPerLine = drawingArea.CornerBR.X - drawingArea.CornerTL.X + 1;
            int maxLines = drawingArea.CornerBR.Y - drawingArea.CornerTL.Y + 1;

            string[] words = Text.Split(' ');

            List<string> res = new List<string>();
            string line = "";

            int lineIndex = 0;

            foreach(string word in words)
            {
                if (lineIndex >= maxLines)
                    break;

                if(word.Length > maxCharsPerLine)
                {
                    int lineBreakCount = word.Length / maxCharsPerLine;

                    //string[] wordBreak = word.Sub

                    for (int i = 0; i < lineBreakCount; i++)
                    {
                        if (lineIndex >= maxLines)
                            break;

                        res.Add(word.Substring(i * (maxCharsPerLine - 1), maxCharsPerLine));

                        lineIndex++;
                    }

                    if (lineIndex < maxLines)
                    {
                        res.Add(word.Substring(lineBreakCount * (maxCharsPerLine - 1)));

                        line = res.Last();

                        lineIndex++;
                    }
                }
                else if((line + word).Length > maxCharsPerLine)
                {
                    res.Add(line);

                    lineIndex++;

                    line = word + " ";
                }
                else
                {
                    line += String.Format("{0} " , word);
                }
            }

            if(line.Length > 0)
            {
                res.Add(line);
            }

            return res;
        }
    }
}
