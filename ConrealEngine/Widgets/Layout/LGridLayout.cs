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
using ConrealEngine;
using ConUI.Helper;

namespace ConUI.Widgets.Layout
{
    public class LGridLayout : LLayoutManager
    {
        private Slot[,] slots;

        public Slot[,] Slots { get { return slots; } }

        
        public LGridLayout(int width, int height)
        {
            slots = new Slot[width, height];
        }

        public override void AddSlot(Slot s)
        {
            base.AddSlot(s);

            if (s != null)
            {
                s.LayoutManager = this;
                slots[0, 0] = s;
            }     
        }

        public LGridLayout AddSlot(Slot s, int xPos, int yPos)
        {
            if(s != null && xPos >= 0 && xPos < Slots.GetLength(0) && yPos >= 0 && yPos < Slots.GetLength(1))
            {
                Slots[xPos, yPos] = s;

                if(s.Widget != null)
                {
                    s.Widget.OnConstruction();
                }
            }

            return this;
        }

        public override void OnLayoutChanged(IntVector2 parentTLPos, IntVector2 lastPossiblePosFromTL)
        {
            IntVector2 sizeOfLayout = lastPossiblePosFromTL - parentTLPos + new IntVector2(1, 1);
            IntVector2 sizeOfChild = sizeOfLayout / new IntVector2(slots.GetLength(0), slots.GetLength(1));

            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for(int y = 0; y < slots.GetLength(1); y++)
                {
                    if(slots[x,y] != null)
                    {
                        Slot s = slots[x, y];

                        s.CalculatedAbsolutePosition = parentTLPos + new IntVector2(x * sizeOfChild.X, y * sizeOfChild.Y);
                        s.CalculatedSize = sizeOfChild;

                        //s.Widget.OnConstruction();

                        if (s.Widget.LayoutManager != null)
                        {
                            s.Widget.LayoutManager.OnLayoutChanged(s.CalculatedAbsolutePosition + s.Margin, s.CalculatedAbsolutePosition + s.CalculatedSize - s.Margin - new IntVector2(1, 1));
                        }
                    }
                }
            }
        }

        public override void RedirectDrawcall(Renderer.RenderHandle r, Box drawingArea)
        {
            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    if (slots[x, y] != null)
                    {
                        Slot s = slots[x, y];

                        if(IsAllowedToDraw(s, drawingArea))
                            s.Widget.ExecDraw(r, drawingArea);
                    }
                }
            }
        }

        public override void RedirectConstruction()
        {
            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    if (slots[x, y] != null)
                    {
                        Slot s = slots[x, y];

                        s.Widget.OnConstruction();
                    }
                }
            }
        }

        public override void RemoveSlot(Slot s)
        {
            base.RemoveSlot(s);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    if (slots[x, y] != null)
                    {
                        Slot s = slots[x, y];

                        s.Widget.OnDestroy();

                        slots[x, y] = null;
                    }
                }
            }
        }
    }
}
