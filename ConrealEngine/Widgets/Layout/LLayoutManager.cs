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
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConrealEngine;
using static ConrealEngine.Renderer;
using ConUI.Helper;

namespace ConUI.Widgets.Layout
{
    public class LLayoutManager
    {
        public enum EHorizontalAlignment
        {
            EHORIZONTAL_ALIGNMENT_LEFT = 0,
            EHORIZONTAL_ALIGNMENT_RIGHT = 1,
            EHORIZONTAL_ALIGNMENT_CENTER = 2,
            EHORIZONTAL_ALIGNMENT_ABSOLUTE = 3
        }

        public enum EVerticalAlignment
        {
            EVERTICAL_ALIGNMENT_TOP = 0,
            EVERTICAL_ALIGNMENT_BOTTOM = 1,
            EVERTICAL_ALIGNMENT_CENTER = 2,
            EVERTICAL_ALIGNMENT_ABSOLUTE = 3
        }

        public enum ESizeManagement
        {
            ESIZEMANAGEMENT_ABSOLUTE = 0,
            ESIZEMANAGEMENT_RELATIVE = 1,
            ESIZEMANAGEMENT_FILL = 2
        }

        public enum EPivot
        {
            EPIVOT_TOP_LEFT = 0,
            EPIVOT_TOP = 1,
            EPIVOT_TOP_RIGHT = 2,
            EPIVOT_LEFT = 3,
            EPIVOT_CENTER = 4,
            EPIVOT_RIGHT = 5,
            EPIVOT_BOTTOM_LEFT = 6,
            EPIVOT_BOTTOM = 7,
            EPIVOT_BOTTOM_RIGHT = 8
        }

        public class Slot
        {

            private EHorizontalAlignment _hAlignment = EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_CENTER;
            private EVerticalAlignment _vAlignment = EVerticalAlignment.EVERTICAL_ALIGNMENT_CENTER;
            private ESizeManagement _sizeManagement = ESizeManagement.ESIZEMANAGEMENT_FILL;
            private IntVector2 _size = new IntVector2(10, 10);
            private Vector2 _expPerc = new Vector2(1.0f, 1.0f);
            private IntVector2 _relPos = new IntVector2(0, 0);
            private IntVector2 _absPos = new IntVector2(0, 0);
            private IntVector2 _margin = new IntVector2(0, 0);
            private EPivot _pivot = EPivot.EPIVOT_CENTER;
            private WWidget _widget;
            private IntVector2 _calcSize = new IntVector2(0, 0);
            private IntVector2 _calcPos = new IntVector2(0, 0);
            private int _zOrder = 0;

            public WWidget Widget
            {
                get { return _widget; }
                set
                {
                    if (value != null)
                    {
                        _widget = value;
                        _widget.LayoutSlot = this;
                        _widget.OnConstruction();
                        SlotChanged();
                    }
                }
            }

            public LLayoutManager LayoutManager { get; set; } = null;
            public EHorizontalAlignment HorizontalAlignment { get { return _hAlignment; } set { _hAlignment = value; SlotChanged(); } }
            public EVerticalAlignment VerticalAlignment { get { return _vAlignment; } set { _vAlignment = value; SlotChanged(); } }
            public ESizeManagement SizeManagement { get { return _sizeManagement; } set { _sizeManagement = value; SlotChanged(); } }
            public IntVector2 Size { get { return _size; } set { _size = value; SlotChanged(); } }
            public Vector2 ExpandPercentage { get { return _expPerc; } set { _expPerc = value; SlotChanged(); } }
            public IntVector2 RelativePosition { get { return _relPos; } set { _relPos = value; SlotChanged(); } }
            public IntVector2 CalculatedAbsolutePosition
            {
                get { return _calcPos; }
                set
                {
                    _calcPos = value;
                    if(Widget != null)
                    {
                        Widget.OnLayoutSlotChanged();
                    }
                }
            }
            public IntVector2 CalculatedSize
            {
                get { return _calcSize; }
                set
                {
                    _calcSize = value;
                    if(Widget != null)
                    {
                        Widget.OnLayoutSlotChanged();
                    }
                }
            }
            public IntVector2 Margin { get { return _margin; } set { _margin = value; SlotChanged(); } }
            public EPivot Pivot { get { return _pivot; } set { _pivot = value; SlotChanged(); } }
            public int ZOrder { get { return _zOrder; } set { _zOrder = value; SlotChanged(); } }

            private void SlotChanged()
            {
                if(Widget != null && !Widget.PendingConstruction && !Widget.PendingDraw && LayoutManager != null)
                {
                    LayoutManager.OnLayoutChanged();
                    Widget.OnLayoutSlotChanged();
                }
            }

            public Slot SetZOrder(int i)
            {
                ZOrder = i;
                return this;
            }

            public Slot SetWidget(WWidget w)
            {
                Widget = w;
                return this;
            }

            public Slot SetHorizontalAlignment(EHorizontalAlignment alignment)
            {
                HorizontalAlignment = alignment;
                return this;
            }

            public Slot SetVerticalAlignment(EVerticalAlignment alignment)
            {
                VerticalAlignment = alignment;
                return this;
            }

            public Slot SetSizeManagement(ESizeManagement sizeManage)
            {
                SizeManagement = sizeManage;
                return this;
            }

            public Slot SetSize(IntVector2 size)
            {
                Size = size;
                return this;
            }

            public Slot SetRelativePosition(IntVector2 position)
            {
                RelativePosition = position;
                return this;
            }

            public Slot SetMargin(IntVector2 margin)
            {
                Margin = margin;
                return this;
            }

            public Slot SetExpandPercentage(Vector2 percentage)
            {
                ExpandPercentage = percentage;
                return this;
            }

            public Slot SetPivot(EPivot pivot)
            {
                Pivot = pivot;
                return this;
            }

            public IntVector2 GetRelativePivotPositionFromTLCorner()
            {
                IntVector2 res = new IntVector2(0, 0);
                IntVector2 center = CalculatedSize / 2;

                switch (Pivot)
                {
                    case EPivot.EPIVOT_BOTTOM:
                        res = new IntVector2(center.X, CalculatedSize.Y - 1);
                        break;
                    case EPivot.EPIVOT_BOTTOM_LEFT:
                        res = new IntVector2(0, CalculatedSize.Y - 1);
                        break;
                    case EPivot.EPIVOT_BOTTOM_RIGHT:
                        res = new IntVector2(CalculatedSize.X - 1, CalculatedSize.Y - 1);
                        break;
                    case EPivot.EPIVOT_CENTER:
                        res = center;
                        break;
                    case EPivot.EPIVOT_LEFT:
                        res = new IntVector2(0, center.Y);
                        break;
                    case EPivot.EPIVOT_RIGHT:
                        res = new IntVector2(CalculatedSize.X - 1, center.Y);
                        break;
                    case EPivot.EPIVOT_TOP:
                        res = new IntVector2(center.X, 0);
                        break;
                    case EPivot.EPIVOT_TOP_LEFT:
                        res = new IntVector2(0, 0);
                        break;
                    case EPivot.EPIVOT_TOP_RIGHT:
                        res = new IntVector2(CalculatedSize.X - 1, 0);
                        break;
                }

                return res;
            }

            public Slot this[WWidget w]
            {
                get
                {
                    this.Widget = w;
                    w.LayoutSlot = this;

                    return this;
                }
            }

            public Slot this[LLayoutManager m]
            {
                get
                {
                    this.Widget = m.Parent;
                    m.Parent.LayoutSlot = this;

                    return this;
                }
            }

            public static LLayoutManager operator +(LLayoutManager m, Slot s)
            {
                m.AddSlot(s);

                return m;
            }
        }

        public WWidget Parent { get; set; }

        public virtual void AddSlot(Slot s)
        {
            s.LayoutManager = this;
        }

        public virtual void RemoveSlot(Slot s)
        {
            
        }

        public void OnLayoutChanged()
        {
            if(Parent != null)
                OnLayoutChanged(Parent.LayoutSlot.CalculatedAbsolutePosition + Parent.LayoutSlot.Margin, Parent.LayoutSlot.CalculatedAbsolutePosition + Parent.LayoutSlot.CalculatedSize - Parent.LayoutSlot.Margin - new IntVector2(1, 1));
            else
                OnLayoutChanged(new IntVector2(0, 0), new IntVector2(Renderer.Instance.Handle.BufferWidth - 1, Renderer.Instance.Handle.BufferHeight - 1));
        }

        public virtual void OnLayoutChanged(IntVector2 parentTLPos, IntVector2 lastPossiblePosFromTL)
        {

        }

        public virtual void RedirectDrawcall(RenderHandle r, Box drawingArea)
        {

        }

        public virtual void RedirectConstruction()
        {

        }

        public virtual void OnDestroy()
        {

        }

        protected bool IsAllowedToDraw(Slot s, Box drawingArea)
        {
            if (s.CalculatedSize.X <= 0 || s.CalculatedSize.Y <= 0)
                return false;

            IntVector2 cornerTopLeft = s.CalculatedAbsolutePosition;
            IntVector2 cornerTopRight = new IntVector2(cornerTopLeft.X + s.CalculatedSize.X - 1, cornerTopLeft.Y);
            IntVector2 cornerBottomRight = new IntVector2(cornerTopLeft.X + s.CalculatedSize.X - 1, cornerTopLeft.Y + s.CalculatedSize.Y - 1);
            IntVector2 cornerBottomLeft = new IntVector2(cornerTopLeft.X, cornerTopLeft.Y + s.CalculatedSize.Y - 1);

            return drawingArea.IsInside(cornerTopLeft) || drawingArea.IsInside(cornerTopRight) || drawingArea.IsInside(cornerBottomRight) || drawingArea.IsInside(cornerBottomLeft);
        }
    }
}
