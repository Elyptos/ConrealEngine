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
    public class LAlignmentLayout : LLayoutManager
    {
        public Slot Child { get; set; }

        public override void AddSlot(Slot s)
        {
            base.AddSlot(s);

            if(s != null)
            {
                Child = s;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if(Child != null)
            {
                Child.Widget.OnDestroy();
            }

            Child = null;
        }

        public override void OnLayoutChanged(IntVector2 parentTLPos, IntVector2 lastPossiblePos)
        {
            if (Child == null)
                return;

            IntVector2 childPos = new IntVector2(0, 0);
            IntVector2 sizeOfLayout = lastPossiblePos - parentTLPos + new IntVector2(1, 1);
            IntVector2 centerOfLayout = sizeOfLayout / 2;

            switch(Child.SizeManagement)
            {
                case ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE:
                    Child.CalculatedSize = new IntVector2(MathHelper.Clamp(Child.Size.X, 0, sizeOfLayout.X), MathHelper.Clamp(Child.Size.Y, 0, sizeOfLayout.Y));
                    break;
                case ESizeManagement.ESIZEMANAGEMENT_RELATIVE:
                    Child.CalculatedSize = sizeOfLayout * Child.ExpandPercentage;
                    break;
                case ESizeManagement.ESIZEMANAGEMENT_FILL:
                    Child.CalculatedSize = sizeOfLayout;
                    break;
            }

            IntVector2 pivotPos = Child.GetRelativePivotPositionFromTLCorner();

            switch (Child.HorizontalAlignment)
            {
                case EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_ABSOLUTE:
                    childPos.X = parentTLPos.X + Child.RelativePosition.X;
                    break;
                case EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_CENTER:
                    childPos.X = parentTLPos.X + centerOfLayout.X;
                    break;
                case EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_LEFT:
                    childPos.X = parentTLPos.X;
                    break;
                case EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_RIGHT:
                    childPos.X = lastPossiblePos.X - Child.CalculatedSize.X + 1;
                    break;
            }

            switch (Child.VerticalAlignment)
            {
                case EVerticalAlignment.EVERTICAL_ALIGNMENT_ABSOLUTE:
                    childPos.Y = parentTLPos.Y + Child.RelativePosition.Y;
                    break;
                case EVerticalAlignment.EVERTICAL_ALIGNMENT_CENTER:
                    childPos.Y = parentTLPos.Y + centerOfLayout.Y;
                    break;
                case EVerticalAlignment.EVERTICAL_ALIGNMENT_TOP:
                    childPos.Y = parentTLPos.Y;
                    break;
                case EVerticalAlignment.EVERTICAL_ALIGNMENT_BOTTOM:
                    childPos.Y = lastPossiblePos.Y - Child.CalculatedSize.Y + 1;
                    break;
            }

            //Child.AbsolutePosition = childPos;
            //Child.CalculatedAbsolutePosition = Child.AbsolutePosition - pivotPos;

            Child.CalculatedAbsolutePosition = childPos - pivotPos;

            //Child.Widget.OnConstruction();

            if (Child.Widget.LayoutManager != null)
            {
                Child.Widget.LayoutManager.OnLayoutChanged(Child.CalculatedAbsolutePosition + Child.Margin, Child.CalculatedAbsolutePosition + Child.CalculatedSize - Child.Margin - new IntVector2(1, 1));
            }
        }

        public override void RedirectDrawcall(Renderer.RenderHandle r, Box drawingArea)
        {
            if(Child != null && IsAllowedToDraw(Child, drawingArea))
            {
                Child.Widget.ExecDraw(r, drawingArea);
            }
        }

        public override void RedirectConstruction()
        {
            if(Child != null)
            {
                Child.Widget.OnConstruction();
            }
        }

        public override void RemoveSlot(Slot s)
        {
            if(s != null && Child == s)
            {
                Child = null;
            }
        }
    }
}
