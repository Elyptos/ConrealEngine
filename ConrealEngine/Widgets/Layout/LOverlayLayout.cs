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

using ConrealEngine;
using ConUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConUI.Widgets.Layout
{
    public class LOverlayLayout : LLayoutManager
    {
        public List<Slot> Children { get; set; } = new List<Slot>();

        public override void AddSlot(Slot s)
        {
            base.AddSlot(s);

            if (s != null)
            {
                Children.Add(s);
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

        public override void OnLayoutChanged(IntVector2 parentTLPos, IntVector2 lastPossiblePos)
        {
            IntVector2 childPos = new IntVector2(0, 0);
            IntVector2 sizeOfLayout = lastPossiblePos - parentTLPos + new IntVector2(1, 1);
            IntVector2 centerOfLayout = sizeOfLayout / 2;

            foreach (Slot s in Children)
            {
                switch (s.SizeManagement)
                {
                    case ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE:
                        s.CalculatedSize = new IntVector2(MathHelper.Clamp(s.Size.X, 0, sizeOfLayout.X), MathHelper.Clamp(s.Size.Y, 0, sizeOfLayout.Y));
                        break;
                    case ESizeManagement.ESIZEMANAGEMENT_RELATIVE:
                        s.CalculatedSize = sizeOfLayout * s.ExpandPercentage;
                        break;
                    case ESizeManagement.ESIZEMANAGEMENT_FILL:
                        s.CalculatedSize = sizeOfLayout;
                        break;
                }

                IntVector2 pivotPos = s.GetRelativePivotPositionFromTLCorner();

                switch (s.HorizontalAlignment)
                {
                    case EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_ABSOLUTE:
                        childPos.X = parentTLPos.X + s.RelativePosition.X;
                        break;
                    case EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_CENTER:
                        childPos.X = parentTLPos.X + centerOfLayout.X;
                        break;
                    case EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_LEFT:
                        childPos.X = parentTLPos.X;
                        break;
                    case EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_RIGHT:
                        childPos.X = lastPossiblePos.X - s.CalculatedSize.X + 1;
                        break;
                }

                switch (s.VerticalAlignment)
                {
                    case EVerticalAlignment.EVERTICAL_ALIGNMENT_ABSOLUTE:
                        childPos.Y = parentTLPos.Y + s.RelativePosition.Y;
                        break;
                    case EVerticalAlignment.EVERTICAL_ALIGNMENT_CENTER:
                        childPos.Y = parentTLPos.Y + centerOfLayout.Y;
                        break;
                    case EVerticalAlignment.EVERTICAL_ALIGNMENT_TOP:
                        childPos.Y = parentTLPos.Y;
                        break;
                    case EVerticalAlignment.EVERTICAL_ALIGNMENT_BOTTOM:
                        childPos.Y = lastPossiblePos.Y - s.CalculatedSize.Y + 1;
                        break;
                }

                s.CalculatedAbsolutePosition = childPos - pivotPos;

                if (s.Widget.LayoutManager != null)
                {
                    s.Widget.LayoutManager.OnLayoutChanged(s.CalculatedAbsolutePosition + s.Margin, s.CalculatedAbsolutePosition + s.CalculatedSize - s.Margin - new IntVector2(1, 1));
                }
            }
        }

        public override void RedirectDrawcall(Renderer.RenderHandle r, Box drawingArea)
        {
            foreach(Slot s in Children)
            {
                if (IsAllowedToDraw(s, drawingArea))
                {
                    s.Widget.ExecDraw(r, drawingArea);
                }
            }
        }

        public override void RedirectConstruction()
        {
            foreach (Slot s in Children)
            {
                s.Widget.OnConstruction();
            }
        }

        public override void RemoveSlot(Slot s)
        {
            Children.Remove(s);
        }
    }
}
