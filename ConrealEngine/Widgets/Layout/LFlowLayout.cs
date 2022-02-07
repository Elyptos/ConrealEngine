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
    public class LFlowLayout : LLayoutManager
    {
        public enum EFlowAlignment
        {
            EFLOW_ALIGNMENT_HORIZONTAL = 0,
            EFLOW_ALIGNMENT_VERTICAL = 1
        }

        public enum EFlowSizeManagement
        {
            EFLOW_SIZE_MANAGEMENT_ABSOLUTE = 0,
            EFLOW_SIZE_MANAGEMENT_FIT = 1
        }

        public List<Slot> Children { get; set; } = new List<Slot>();
        public EFlowAlignment FlowAlignment { get; set; } = EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL;
        public EFlowSizeManagement FlowSizeManagement { get; set; } = EFlowSizeManagement.EFLOW_SIZE_MANAGEMENT_FIT;
        public int ElementSize { get; set; } = 3;

        public LFlowLayout SetElementSize(int size)
        {
            this.ElementSize = size;
            return this;
        }

        public LFlowLayout SetFlowAlginment(EFlowAlignment al)
        {
            this.FlowAlignment = al;
            return this;
        }

        public LFlowLayout SetFlowSizeManagement(EFlowSizeManagement sm)
        {
            this.FlowSizeManagement = sm;
            return this;
        }

        public override void AddSlot(Slot s)
        {
            base.AddSlot(s);

            if (s != null)
            {
                Children.Add(s);
            }
        }

        public override void OnLayoutChanged(IntVector2 parentTLPos, IntVector2 lastPossiblePosFromTL)
        {
            List<Slot> sizePriority = Children;
            IntVector2 sizeOfLayout = lastPossiblePosFromTL - parentTLPos + new IntVector2(1, 1);
            IntVector2 centerOfLayout = sizeOfLayout / 2;
            int remainingSize = 0;
            int occupiedSize = 0;
            int sizeToThisPoint = 0;

            if (Children.Count == 0)
                return;

            switch (FlowAlignment)
            {
                case EFlowAlignment.EFLOW_ALIGNMENT_HORIZONTAL:
                    sizePriority = sizePriority.OrderByDescending(x => x.ExpandPercentage.X).ToList();

                    remainingSize = sizeOfLayout.X;

                    switch(FlowSizeManagement)
                    {
                        case EFlowSizeManagement.EFLOW_SIZE_MANAGEMENT_ABSOLUTE:
                            for (int i = 0; i < sizePriority.Count; i++)
                            {
                                sizePriority[i].CalculatedSize = new IntVector2(ElementSize, sizePriority[i].CalculatedSize.Y);
                            }
                            break;
                        case EFlowSizeManagement.EFLOW_SIZE_MANAGEMENT_FIT:
                            for (int i = 0; i < sizePriority.Count; i++)
                            {
                                sizePriority[i].CalculatedSize = new IntVector2((int)(remainingSize * sizePriority[i].ExpandPercentage.X), sizePriority[i].CalculatedSize.Y);
                            }
                            break;
                    }

                    occupiedSize = Children.Sum(x => x.CalculatedSize.X);

                    Children.Last().CalculatedSize = new IntVector2(Children.Last().CalculatedSize.X + (sizeOfLayout.X - occupiedSize), Children.Last().CalculatedSize.X);


                    foreach (Slot s in Children)
                    {
                        if (s.CalculatedSize.X > 0)
                        {
                            IntVector2 childPos = new IntVector2(0, 0);

                            switch (s.SizeManagement)
                            {
                                case ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE:
                                    s.CalculatedSize = new IntVector2(s.CalculatedSize.X, MathHelper.Clamp(s.Size.Y, 0, sizeOfLayout.Y));
                                    break;
                                case ESizeManagement.ESIZEMANAGEMENT_RELATIVE:
                                    s.CalculatedSize = new IntVector2(s.CalculatedSize.X, (int)(sizeOfLayout.Y * s.ExpandPercentage.Y));
                                    break;
                                case ESizeManagement.ESIZEMANAGEMENT_FILL:
                                    s.CalculatedSize = new IntVector2(s.CalculatedSize.X, sizeOfLayout.Y);
                                    break;
                            }

                            IntVector2 pivotPos = s.GetRelativePivotPositionFromTLCorner();

                            childPos.X = parentTLPos.X + pivotPos.X + sizeToThisPoint;

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
                                    childPos.Y = lastPossiblePosFromTL.Y - s.CalculatedSize.Y + 1;
                                    break;
                            }

                            //s.AbsolutePosition = childPos;
                            //s.CalculatedAbsolutePosition = s.AbsolutePosition - pivotPos;

                            s.CalculatedAbsolutePosition = childPos - pivotPos;

                            sizeToThisPoint += s.CalculatedSize.X;

                            //s.Widget.OnConstruction();

                            if (s.Widget.LayoutManager != null)
                            {
                                s.Widget.LayoutManager.OnLayoutChanged(s.CalculatedAbsolutePosition + s.Margin, s.CalculatedAbsolutePosition + s.CalculatedSize - s.Margin - new IntVector2(1, 1));
                            }
                        }
                    }

                    break;
                case EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL:
                    sizePriority = sizePriority.OrderByDescending(x => x.ExpandPercentage.Y).ToList();

                    remainingSize = sizeOfLayout.Y;

                    switch(FlowSizeManagement)
                    {
                        case EFlowSizeManagement.EFLOW_SIZE_MANAGEMENT_ABSOLUTE:
                            for (int i = 0; i < sizePriority.Count; i++)
                            {
                                sizePriority[i].CalculatedSize = new IntVector2(sizePriority[i].CalculatedSize.X, ElementSize);
                            }
                            break;
                        case EFlowSizeManagement.EFLOW_SIZE_MANAGEMENT_FIT:
                            for (int i = 0; i < sizePriority.Count; i++)
                            {
                                sizePriority[i].CalculatedSize = new IntVector2(sizePriority[i].CalculatedSize.X, (int)(remainingSize * sizePriority[i].ExpandPercentage.Y));
                            }
                            break;
                    }

                    occupiedSize = Children.Sum(x => x.CalculatedSize.Y);

                    Children.Last().CalculatedSize = new IntVector2(Children.Last().CalculatedSize.X, Children.Last().CalculatedSize.Y + (sizeOfLayout.Y - occupiedSize));

                    foreach (Slot s in Children)
                    {
                        if (s.CalculatedSize.Y > 0)
                        {
                            IntVector2 childPos = new IntVector2(0, 0);

                            switch (s.SizeManagement)
                            {
                                case ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE:
                                    s.CalculatedSize = new IntVector2(MathHelper.Clamp(s.Size.X, 0, sizeOfLayout.X), s.CalculatedSize.Y);
                                    break;
                                case ESizeManagement.ESIZEMANAGEMENT_RELATIVE:
                                    s.CalculatedSize = new IntVector2((int)(sizeOfLayout.X * s.ExpandPercentage.X), s.CalculatedSize.Y);
                                    break;
                                case ESizeManagement.ESIZEMANAGEMENT_FILL:
                                    s.CalculatedSize = new IntVector2(sizeOfLayout.X, s.CalculatedSize.Y);
                                    break;
                            }

                            IntVector2 pivotPos = s.GetRelativePivotPositionFromTLCorner();

                            childPos.Y = parentTLPos.Y + pivotPos.Y + sizeToThisPoint;

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
                                    childPos.X = lastPossiblePosFromTL.X - s.CalculatedSize.X + 1;
                                    break;
                            }

                            //s.AbsolutePosition = childPos;
                            //s.CalculatedAbsolutePosition = s.AbsolutePosition - pivotPos;

                            s.CalculatedAbsolutePosition = childPos - pivotPos;

                            sizeToThisPoint += s.CalculatedSize.Y;

                            //s.Widget.OnConstruction();

                            if (s.Widget.LayoutManager != null)
                            {
                                s.Widget.LayoutManager.OnLayoutChanged(s.CalculatedAbsolutePosition + s.Margin, s.CalculatedAbsolutePosition + s.CalculatedSize - s.Margin - new IntVector2(1, 1));
                            }
                        }
                    }



                    break;
            }
        }

        public override void RedirectDrawcall(Renderer.RenderHandle r, Box drawingArea)
        {
            foreach (Slot w in Children)
            {
                if(IsAllowedToDraw(w, drawingArea))
                    w.Widget.ExecDraw(r, drawingArea);
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
            if(s != null)
            {
                Children.Remove(s);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            foreach (Slot s in Children)
            {
                s.Widget.OnDestroy();
            }

            Children.Clear();
        }
    }
}
