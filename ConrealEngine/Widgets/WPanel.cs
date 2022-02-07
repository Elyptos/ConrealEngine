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
using System.IO;
using ConrealEngine;
using System.Numerics;
using ConUI.Helper;
using ConrealEngine.Assets;

namespace ConUI.Widgets
{
    public class WPanel : WWidget
    {
        public class SpriteAnimation
        {
            public class AnimData
            {
                public int AnimIndex { get; set; }
                public UnicodeImage Image { get; set; }
                public UnicodeImage.ImagePixel[,] SampledImage { get; set; }
            }

            private string animationName = "";
            private float currAnimIndex = 0;

            public string AnimName { get { return animationName; } }

            public float AnimSpeed { get; set; } = 1.0f;

            public int AnimIndex { get { return (int)currAnimIndex; } }

            public List<AnimData> Images { get; set; } = new List<AnimData>();

            public SpriteAnimation(string animPath, string animName)
            {
                LoadAnimation(animPath, animName);
            }

            public SpriteAnimation(string animPath, string animName, float animSpeed)
            {
                LoadAnimation(animPath, animName);

                AnimSpeed = animSpeed;
            }

            public void LoadAnimation(string animPath, string animName)
            {
                if (!Directory.Exists(animPath))
                    return;

                Images.Clear();

                try
                {
                    string[] filePaths = Directory.GetFiles(animPath);

                    foreach(string path in filePaths)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(path);

                        if(fileName.Contains(animName))
                        {
                            int animIndex = 0;

                            if(int.TryParse(fileName.Substring(fileName.LastIndexOf('_') + 1), out animIndex))
                            {
                                try
                                {
                                    Images.Add(new AnimData() {AnimIndex = animIndex, Image = ResourceHandler.Instance.LoadImage(path) });
                                    

                                    animationName = animName;
                                }
                                catch (ArgumentException) { }
                            }
                        }
                    }

                    Images.OrderBy(x => x.AnimIndex);
                    ResetAnimation();
                }
                catch (IOException) { }
            }

            public void ResampleAnimationData(IntVector2 size)
            {
                foreach(AnimData data in Images)
                {
                    data.SampledImage = data.Image.Sample(size);
                }
            }

            public void ResetAnimation()
            {
                currAnimIndex = 0;
            }

            public UnicodeImage.ImagePixel[,] GetCurrImage(float deltaSeconds)
            {
                if (Images.Count == 0)
                    return null;

                currAnimIndex = currAnimIndex + deltaSeconds * AnimSpeed;

                if(currAnimIndex >= Images.Count)
                {
                    currAnimIndex = 0.0f;
                }
                else if(currAnimIndex < 0)
                {
                    currAnimIndex = (float)Images.Count - 1;
                }

                return Images[AnimIndex].SampledImage;
            }
        }

        private IntVector2 sizeBefore = new IntVector2();
        private UnicodeImage.ImagePixel[,] sampledImage;
        private SpriteAnimation _currAnimation;
        private UnicodeImage _image;

        public bool Transparent { get; set; } = false;

        public char Background { get; set; } = ' ';

        public UnicodeImage Image
        {
            get { return _image; }
            set
            {
                _image = value;

                if (_image != null && LayoutSlot != null)
                {
                    sampledImage = _image.Sample(LayoutSlot.CalculatedSize);
                }
                else
                {
                    sampledImage = null;
                }
            }
        }

        public Dictionary<string, SpriteAnimation> SpriteAnimations { get; set; } = new Dictionary<string, SpriteAnimation>();

        public SpriteAnimation CurrentAnimation
        {
            get
            {
                return _currAnimation;
            }
            set
            {
                _currAnimation = value;

                if(_currAnimation != null && LayoutSlot != null)
                {
                    _currAnimation.ResampleAnimationData(LayoutSlot.CalculatedSize);
                }
            }
        }

        public WPanel SetCurrentAnimation(SpriteAnimation anim)
        {
            CurrentAnimation = anim;

            return this;
        }

        public WPanel SetSpriteAnimations(Dictionary<string, SpriteAnimation> animations)
        {
            SpriteAnimations = animations;

            return this;
        }

        public WPanel DrawImage(string relImagePath)
        {
            Image = ResourceHandler.Instance.LoadImage(relImagePath);

            return this;
        }

        public WPanel SetTransparency(bool b)
        {
            Transparent = b;
            return this;
        }

        public WPanel SetBackgroundColor(ConsoleColor c)
        {
            BackgroundColor = c;
            return this;
        }

        public WPanel SetForegroundColor(ConsoleColor c)
        {
            ForegroundColor = c;
            return this;
        }

        public WPanel SetBackground(char c)
        {
            Background = c;
            return this;
        }

        public WPanel DrawBorder(EBorderStyle border)
        {
            this.Border = border;
            return this;
        }

        public override void OnConstruction()
        {
            base.OnConstruction();
        }

        protected override void OnDraw(Renderer.RenderHandle r, Box drawingArea)
        {
            Box widgetArea = GetDrawingArea(drawingArea);

            if(widgetArea.IsValid())
            {
                if (sampledImage != null)
                {
                    for (int x = 0; x < sampledImage.GetLength(0); x++)
                    {
                        for (int y = 0; y < sampledImage.GetLength(1); y++)
                        {
                            IntVector2 charPos = LayoutSlot.CalculatedAbsolutePosition + new IntVector2(x, y);
                            
                            if(widgetArea.IsInside(charPos))
                            {
                                if(sampledImage[x, y].Char == '\0')
                                {
                                    if (!Transparent)
                                    {
                                        r.Draw(Background, charPos, ForegroundColor, BackgroundColor);
                                    }
                                }
                                else
                                {
                                    r.Draw(sampledImage[x, y].Char, charPos, sampledImage[x, y].Color, BackgroundColor);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if(!Transparent)
                        r.DrawMany(Background, widgetArea.CornerTL, widgetArea.CornerBR, ForegroundColor, BackgroundColor);
                }
            }

            base.OnDraw(r, drawingArea);
        }

        public override void OnTick(float deltaSeconds)
        {
            if(CurrentAnimation != null)
                sampledImage = CurrentAnimation.GetCurrImage(deltaSeconds);
        }

        public override void OnLayoutSlotChanged()
        {
            if (LayoutSlot.CalculatedSize != sizeBefore)
            {
                if (CurrentAnimation != null)
                {
                    CurrentAnimation.ResampleAnimationData(LayoutSlot.CalculatedSize);
                }
                else if (Image != null)
                {
                    sampledImage = Image.Sample(LayoutSlot.CalculatedSize);
                }

                sizeBefore = LayoutSlot.CalculatedSize;
            }
        }
    }
}
