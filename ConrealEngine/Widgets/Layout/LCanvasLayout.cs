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
using static ConrealEngine.Renderer;

namespace ConUI.Widgets.Layout
{
    public class LCanvasLayout : LLayoutManager
    {
        public List<Slot> Children { get; set; } = new List<Slot>();

        public override void AddSlot(Slot s)
        {
            base.AddSlot(s);

            if (s != null)
                Children.Add(s);
        }

        public override void RemoveSlot(Slot s)
        {
            Children.Remove(s);
        }



        public override void RedirectDrawcall(RenderHandle r, Box drawingArea)
        {
            List<Slot> sortedChildren = Children.OrderBy(x => x.ZOrder).ToList();

            foreach (Slot w in sortedChildren)
            {
                if(IsAllowedToDraw(w, drawingArea))
                    w.Widget.ExecDraw(r, drawingArea);
            }
        }

        public override void OnLayoutChanged(IntVector2 parentTLPos, IntVector2 lastPossiblePos)
        {
            foreach(Slot s in Children)
            {
                IntVector2 sizeOfLayout = lastPossiblePos - parentTLPos + new IntVector2(1, 1);

                s.CalculatedSize = s.Size;

                IntVector2 pivotPos = s.GetRelativePivotPositionFromTLCorner();

                //s.AbsolutePosition = parentTLPos + s.RelativePosition;
                //s.CalculatedAbsolutePosition = s.AbsolutePosition - pivotPos;

                s.CalculatedAbsolutePosition = (parentTLPos + s.RelativePosition) - pivotPos;

                //s.Widget.OnConstruction();

                if(s.Widget.LayoutManager != null)
                {
                    s.Widget.LayoutManager.OnLayoutChanged(s.CalculatedAbsolutePosition + s.Margin, s.CalculatedAbsolutePosition + s.CalculatedSize - s.Margin - new IntVector2(1, 1));
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            foreach(Slot s in Children)
            {
                s.Widget.OnDestroy();
            }

            Children.Clear();
        }

        public override void RedirectConstruction()
        {
            foreach (Slot s in Children)
            {
                s.Widget.OnConstruction();
            }
        }
    }
}
