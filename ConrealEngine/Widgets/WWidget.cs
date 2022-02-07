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
using System.Numerics;
using ConUI.Widgets.Layout;
using static ConUI.Widgets.Layout.LLayoutManager;
using static ConrealEngine.Controller;
using ConUI.Helper;

namespace ConUI.Widgets
{
    public class WWidget : Drawable
    {
        public enum EBorderStyle
        {
            EBORDER_STYLE_NONE = -1,
            EBORDER_STYLE_DOUBLE = 0,
            EBORDER_STYLE_SINGLE_HEAVY = 1,
            EBORDER_STYLE_SINGLE_LIGHT = 2,
            EBORDER_STYLE_SINGLE_DASH = 3,
            EBORDER_STYLE_SINGLE_ROUND_EDGE = 4
        }

        protected WWidget parent;
        private LLayoutManager layoutManager;
        private WPopupWidget popupWidget = null;
        private bool pendingConstruction = true;
        private bool pendingDraw = false;
        private bool isTickable = false;

        readonly char[,] BORDERLIB = new char[,]
        {
            {(char)0x2554, (char)0x2557, (char)0x255D, (char)0x255A, (char)0x2550, (char)0x2551},
            {(char)0x250f, (char)0x2513, (char)0x251b, (char)0x2517, (char)0x2501, (char)0x2503},
            {(char)0x250c, (char)0x2510, (char)0x2518, (char)0x2514, (char)0x2500, (char)0x2502},
            {(char)0x250c, (char)0x2510, (char)0x2518, (char)0x2514, (char)0x2504, (char)0x2506},
            {(char)0x256d, (char)0x256e, (char)0x256f, (char)0x2570, (char)0x2500, (char)0x2502},
        };

        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.White;
        public WWidget Parent { get { return parent; } }
        public LLayoutManager LayoutManager
        {
            get{ return layoutManager; }
            set
            {
                if (value != null)
                {
                    layoutManager = value;
                    LayoutManager.Parent = this;

                    if(!pendingConstruction)
                        layoutManager.OnLayoutChanged();
                }
            }
        }
        public Slot LayoutSlot{ get; set; }
        public EBorderStyle Border { get; set; } = EBorderStyle.EBORDER_STYLE_NONE;
        public bool PendingConstruction { get { return pendingConstruction; } }
        public bool PendingDraw { get { return pendingDraw; } }
        public bool IsVisible { get; set; } = true;

        public WWidget() { }

        public T SetLayoutManager<T>(T l) where T : LLayoutManager
        {
            LayoutManager = l;

            return l;
        }

        /*public void ExecDraw()
        {
            Box dArea = GetParentDrawingArea();

            ExecDraw(Renderer.Instance.Handle, dArea);
        }*/


        public void ExecDraw(Renderer.RenderHandle r, Box drawingArea)
        {
            if (!IsVisible)
                return;

            this.OnPreDraw();
            this.OnDraw(r, drawingArea);
            this.OnPostDraw();
        }

        public virtual void OnUserFocus()
        {

        }

        public virtual void OnUserDefocus()
        {

        }

        public virtual void OnUserActivate()
        {

        }

        public void SetPosition(IntVector2 pos)
        {
            LayoutSlot.RelativePosition = pos;
            LayoutSlot.LayoutManager.OnLayoutChanged();
            LayoutSlot.LayoutManager.RedirectDrawcall(Renderer.Instance.Handle, GetParentDrawingArea());
        }

        public IntVector2 GetPosition()
        {
            return LayoutSlot.RelativePosition;
        }

        public virtual Dictionary<string, InputEventDelegate> OnRegister()
        {
            return new Dictionary<string, InputEventDelegate>();
        }

        public virtual void OnDestroy()
        {
            if(isTickable)
            {
                Engine.Instance.RemoveTickableActor(this);
            }

            if(LayoutManager != null)
            {
                LayoutManager.OnDestroy();
            }
        }

        public void ShowPopup(string infoText, string applyText, string cancelText, WButton.ButtonDelegate applyCallback)
        {
            if (popupWidget != null)
                return;

            popupWidget = new WPopupWidget(this);

            popupWidget.InformationText = infoText;
            popupWidget.ApplyButtonText = applyText;
            popupWidget.CancelButtonText = cancelText;
            popupWidget.AddOnApply(applyCallback);

            LAlignmentLayout popupLayout = new LAlignmentLayout();

            popupLayout.AddSlot(
                    new Slot()
                        .SetSize(new IntVector2(30, 10))
                        .SetHorizontalAlignment(EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_CENTER)
                        .SetVerticalAlignment(EVerticalAlignment.EVERTICAL_ALIGNMENT_CENTER)
                        .SetSizeManagement(ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE)
                        [
                            popupWidget
                        ]
                    );

            popupLayout.Parent = this;
            popupLayout.OnLayoutChanged(LayoutSlot.CalculatedAbsolutePosition, LayoutSlot.CalculatedAbsolutePosition + LayoutSlot.CalculatedSize - new IntVector2(1, 1));
            //popupWidget.ExecDraw();
        }

        public void HidePopup()
        {
            if(popupWidget != null)
            {
                popupWidget.LayoutManager.OnDestroy();

                popupWidget = null;

                //ExecDraw();
            }
        }

        public override void OnConstruction()
        {
            if(!isTickable)
                EnableTick(ShouldTick());

            if(LayoutManager != null)
            {
                LayoutManager.RedirectConstruction();
            }

            pendingConstruction = false;
        }

        public virtual void OnLayoutSlotChanged()
        {

        }

        protected override bool ShouldTick()
        {
            return false;
        }

        public override void EnableTick(bool b)
        {
            if (b)
            {
                Engine.Instance.AddTickableActor(this);
            }
            else
            {
                Engine.Instance.RemoveTickableActor(this);
            }

            isTickable = b;
        }

        public override void OnTick(float deltaSeconds)
        {
            
        }

        protected override void OnPreDraw()
        {
            pendingDraw = true;
        }

        protected override void OnDraw(Renderer.RenderHandle r, Box drawingArea)
        {
            Box nDrawingArea = GetDrawingArea(drawingArea);
            Box borderDrawingArea = GetDrawingArea(drawingArea, false);

            if(borderDrawingArea.IsValid())
            {
                if (LayoutManager != null)
                {
                    if (nDrawingArea.IsValid())
                        LayoutManager.RedirectDrawcall(r, nDrawingArea);
                }

                if (Border != EBorderStyle.EBORDER_STYLE_NONE)
                {
                    MakeBorder(r, borderDrawingArea);
                }
            }

            if(popupWidget != null)
            {
                popupWidget.LayoutSlot.LayoutManager.OnLayoutChanged(LayoutSlot.CalculatedAbsolutePosition, LayoutSlot.CalculatedAbsolutePosition + LayoutSlot.CalculatedSize - new IntVector2(1, 1));
                popupWidget.ExecDraw(r, nDrawingArea);
                //popupWidget.ExecDraw();
            }
        }

        //protected bool IsInsideClip(IntVector2 posFromTLAbsolute)
        //{
        //    IntVector2 clip = this.LayoutSlot.CalculatedAbsolutePosition + LayoutSlot.CalculatedClip;

        //    return posFromTLAbsolute.X <= clip.X && posFromTLAbsolute.Y <= clip.Y;
        //}

        protected override void OnPostDraw()
        {
            pendingDraw = false;
        }

        protected Box GetParentDrawingArea()
        {
            Box res = new Box();

            if (this.LayoutSlot.LayoutManager.Parent == null)
            {
                res.CornerTL = new IntVector2(0, 0);
                res.CornerTR = new IntVector2(Renderer.Instance.Handle.BufferWidth - 1, 0);
                res.CornerBR = new IntVector2(Renderer.Instance.Handle.BufferWidth - 1, Renderer.Instance.Handle.BufferHeight - 1);
                res.CornerBL = new IntVector2(0, Renderer.Instance.Handle.BufferHeight - 1);
            }
            else
            {
                res = this.LayoutSlot.LayoutManager.Parent.GetDrawingArea();
            }

            return res;
        }

        protected Box GetDrawingArea()
        {
            Box parentDrArea = new Box();
            Box res = new Box();

            if(this.LayoutSlot.LayoutManager.Parent == null)
            {
                parentDrArea.CornerTL = new IntVector2(0, 0);
                parentDrArea.CornerTR = new IntVector2(Renderer.Instance.Handle.BufferWidth - 1, 0);
                parentDrArea.CornerBR = new IntVector2(Renderer.Instance.Handle.BufferWidth - 1, Renderer.Instance.Handle.BufferHeight - 1);
                parentDrArea.CornerBL = new IntVector2(0, Renderer.Instance.Handle.BufferHeight - 1);
            }
            else
            {
                parentDrArea = this.LayoutSlot.LayoutManager.Parent.GetDrawingArea();
            }

            res = GetDrawingArea(parentDrArea);

            return res;
        }

        protected Box GetDrawingArea(Box drawingAreaParent, bool withMargin = true)
        {
            IntVector2 cornerTopLeft = this.LayoutSlot.CalculatedAbsolutePosition;
            IntVector2 cornerTopRight = new IntVector2(cornerTopLeft.X + this.LayoutSlot.CalculatedSize.X - 1, cornerTopLeft.Y);
            IntVector2 cornerBottomRight = new IntVector2(cornerTopLeft.X + this.LayoutSlot.CalculatedSize.X - 1, cornerTopLeft.Y + this.LayoutSlot.CalculatedSize.Y - 1);
            IntVector2 cornerBottomLeft = new IntVector2(cornerTopLeft.X, cornerTopLeft.Y + this.LayoutSlot.CalculatedSize.Y - 1);

            if(withMargin)
            {
                cornerTopLeft += this.LayoutSlot.Margin;

                cornerBottomRight -= this.LayoutSlot.Margin;

                cornerTopRight.X -= this.LayoutSlot.Margin.X;
                cornerTopRight.Y += this.LayoutSlot.Margin.Y;

                cornerBottomLeft.X += this.LayoutSlot.Margin.X;
                cornerBottomLeft.Y -= this.LayoutSlot.Margin.Y;
            }

            Box res = drawingAreaParent;

            if(!drawingAreaParent.IsInside(cornerTopLeft) && !drawingAreaParent.IsInside(cornerTopRight) && !drawingAreaParent.IsInside(cornerBottomRight) && !drawingAreaParent.IsInside(cornerBottomLeft))
            {
                res.CornerTL = new IntVector2(2, 0);
                res.CornerBR = new IntVector2(0, 0);
            }
            else
            {
                if (drawingAreaParent.IsInside(cornerTopLeft))
                {
                    res.CornerTL = cornerTopLeft;
                }
                else
                {
                    if(drawingAreaParent.IsInside(new IntVector2(cornerTopLeft.X, res.CornerTL.Y)))
                    {
                        res.CornerTL = new IntVector2(cornerTopLeft.X, res.CornerTL.Y);
                    }
                    else if(drawingAreaParent.IsInside(new IntVector2(res.CornerTL.X, cornerTopLeft.Y)))
                    {
                        res.CornerTL = new IntVector2(res.CornerTL.X, cornerTopLeft.Y);
                    }
                }

                if (drawingAreaParent.IsInside(cornerTopRight))
                {
                    res.CornerTR = cornerTopRight;
                }
                else
                {
                    if (drawingAreaParent.IsInside(new IntVector2(cornerTopRight.X, res.CornerTR.Y)))
                    {
                        res.CornerTR = new IntVector2(cornerTopRight.X, res.CornerTR.Y);
                    }
                    else if (drawingAreaParent.IsInside(new IntVector2(res.CornerTL.X, cornerTopRight.Y)))
                    {
                        res.CornerTR = new IntVector2(res.CornerTR.X, cornerTopRight.Y);
                    }
                }

                if (drawingAreaParent.IsInside(cornerBottomRight))
                {
                    res.CornerBR = cornerBottomRight;
                }
                else
                {
                    if (drawingAreaParent.IsInside(new IntVector2(cornerBottomRight.X, res.CornerBR.Y)))
                    {
                        res.CornerBR = new IntVector2(cornerBottomRight.X, res.CornerBR.Y);
                    }
                    else if (drawingAreaParent.IsInside(new IntVector2(res.CornerBR.X, cornerBottomRight.Y)))
                    {
                        res.CornerBR = new IntVector2(res.CornerBR.X, cornerBottomRight.Y);
                    }
                }

                if (drawingAreaParent.IsInside(cornerBottomLeft))
                {
                    res.CornerBL = cornerBottomLeft;
                }
                else
                {
                    if (drawingAreaParent.IsInside(new IntVector2(cornerBottomLeft.X, res.CornerBL.Y)))
                    {
                        res.CornerBL = new IntVector2(cornerBottomLeft.X, res.CornerBL.Y);
                    }
                    else if (drawingAreaParent.IsInside(new IntVector2(res.CornerBL.X, cornerBottomLeft.Y)))
                    {
                        res.CornerBL = new IntVector2(res.CornerBL.X, cornerBottomLeft.Y);
                    }
                }
            }

            return res;
        }

        protected void MakeBorder(Renderer.RenderHandle r, Box drawingArea)
        {
            if (Border == EBorderStyle.EBORDER_STYLE_NONE)
                return;

            //Make Corners
            IntVector2 cornerTopLeft = this.LayoutSlot.CalculatedAbsolutePosition;
            IntVector2 cornerTopRight = new IntVector2(cornerTopLeft.X + this.LayoutSlot.CalculatedSize.X - 1, cornerTopLeft.Y);
            IntVector2 cornerBottomRight = new IntVector2(cornerTopLeft.X + this.LayoutSlot.CalculatedSize.X - 1, cornerTopLeft.Y + this.LayoutSlot.CalculatedSize.Y - 1);
            IntVector2 cornerBottomLeft = new IntVector2(cornerTopLeft.X, cornerTopLeft.Y + this.LayoutSlot.CalculatedSize.Y - 1);

            if(cornerTopLeft == drawingArea.CornerTL && cornerTopRight == drawingArea.CornerTR && cornerBottomRight == drawingArea.CornerBR && cornerBottomLeft == drawingArea.CornerBL)
            {
                r.Draw(BORDERLIB[(int)Border, 0], cornerTopLeft, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 1], cornerTopRight, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 2], cornerBottomRight, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 3], cornerBottomLeft, ForegroundColor, BackgroundColor);

                r.DrawMany(BORDERLIB[(int)Border, 4], cornerTopLeft + new IntVector2(1, 0), cornerTopRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopLeft + new IntVector2(0, 1), cornerBottomLeft - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 4], cornerBottomLeft + new IntVector2(1, 0), cornerBottomRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopRight + new IntVector2(0, 1), cornerBottomRight - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
            }
            else if (cornerTopLeft == drawingArea.CornerTL && cornerTopRight == drawingArea.CornerTR && cornerBottomRight != drawingArea.CornerBR && cornerBottomLeft != drawingArea.CornerBL)
            {
                r.Draw(BORDERLIB[(int)Border, 0], cornerTopLeft, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 1], cornerTopRight, ForegroundColor, BackgroundColor);

                r.DrawMany(BORDERLIB[(int)Border, 4], cornerTopLeft + new IntVector2(1, 0), cornerTopRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopLeft + new IntVector2(0, 1), drawingArea.CornerBL, ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopRight + new IntVector2(0, 1), drawingArea.CornerBR, ForegroundColor, BackgroundColor);
            }
            else if (cornerTopLeft != drawingArea.CornerTL && cornerTopRight == drawingArea.CornerTR && cornerBottomRight == drawingArea.CornerBR && cornerBottomLeft != drawingArea.CornerBL)
            {
                r.Draw(BORDERLIB[(int)Border, 1], cornerTopRight, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 2], cornerBottomRight, ForegroundColor, BackgroundColor);

                r.DrawMany(BORDERLIB[(int)Border, 4], drawingArea.CornerTL, cornerTopRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 4], drawingArea.CornerBL, cornerBottomRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopRight + new IntVector2(0, 1), cornerBottomRight - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
            }
            else if (cornerTopLeft != drawingArea.CornerTL && cornerTopRight != drawingArea.CornerTR && cornerBottomRight == drawingArea.CornerBR && cornerBottomLeft == drawingArea.CornerBL)
            {
                r.Draw(BORDERLIB[(int)Border, 2], cornerBottomRight, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 3], cornerBottomLeft, ForegroundColor, BackgroundColor);

                r.DrawMany(BORDERLIB[(int)Border, 5], drawingArea.CornerTL, cornerBottomLeft - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 4], cornerBottomLeft + new IntVector2(1, 0), cornerBottomRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], drawingArea.CornerTR, cornerBottomRight - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
            }
            else if (cornerTopLeft == drawingArea.CornerTL && cornerTopRight == drawingArea.CornerTR && cornerBottomRight == drawingArea.CornerBR && cornerBottomLeft != drawingArea.CornerBL)
            {
                r.Draw(BORDERLIB[(int)Border, 0], cornerTopLeft, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 1], cornerTopRight, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 2], cornerBottomRight, ForegroundColor, BackgroundColor);

                r.DrawMany(BORDERLIB[(int)Border, 4], cornerTopLeft + new IntVector2(1, 0), cornerTopRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopLeft + new IntVector2(0, 1), drawingArea.CornerBL, ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 4], drawingArea.CornerBL, cornerBottomRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopRight + new IntVector2(0, 1), cornerBottomRight - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
            }
            else if (cornerTopLeft != drawingArea.CornerTL && cornerTopRight == drawingArea.CornerTR && cornerBottomRight == drawingArea.CornerBR && cornerBottomLeft == drawingArea.CornerBL)
            {
                r.Draw(BORDERLIB[(int)Border, 1], cornerTopRight, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 2], cornerBottomRight, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 3], cornerBottomLeft, ForegroundColor, BackgroundColor);

                r.DrawMany(BORDERLIB[(int)Border, 4], drawingArea.CornerTL, cornerTopRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], drawingArea.CornerTL, cornerBottomLeft - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 4], cornerBottomLeft + new IntVector2(1, 0), cornerBottomRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopRight + new IntVector2(0, 1), cornerBottomRight - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
            }
            else if (cornerTopLeft == drawingArea.CornerTL && cornerTopRight != drawingArea.CornerTR && cornerBottomRight == drawingArea.CornerBR && cornerBottomLeft == drawingArea.CornerBL)
            {
                r.Draw(BORDERLIB[(int)Border, 0], cornerTopLeft, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 2], cornerBottomRight, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 3], cornerBottomLeft, ForegroundColor, BackgroundColor);

                r.DrawMany(BORDERLIB[(int)Border, 4], cornerTopLeft + new IntVector2(1, 0), drawingArea.CornerTR, ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopLeft + new IntVector2(0, 1), cornerBottomLeft - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 4], cornerBottomLeft + new IntVector2(1, 0), cornerBottomRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], drawingArea.CornerTR, cornerBottomRight - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
            }
            else if (cornerTopLeft == drawingArea.CornerTL && cornerTopRight == drawingArea.CornerTR && cornerBottomRight != drawingArea.CornerBR && cornerBottomLeft == drawingArea.CornerBL)
            {
                r.Draw(BORDERLIB[(int)Border, 0], cornerTopLeft, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 1], cornerTopRight, ForegroundColor, BackgroundColor);
                r.Draw(BORDERLIB[(int)Border, 3], cornerBottomLeft, ForegroundColor, BackgroundColor);

                r.DrawMany(BORDERLIB[(int)Border, 4], cornerTopLeft + new IntVector2(1, 0), cornerTopRight - new IntVector2(1, 0), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopLeft + new IntVector2(0, 1), cornerBottomLeft - new IntVector2(0, 1), ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 4], cornerBottomLeft + new IntVector2(1, 0), drawingArea.CornerBR, ForegroundColor, BackgroundColor);
                r.DrawMany(BORDERLIB[(int)Border, 5], cornerTopRight + new IntVector2(0, 1), drawingArea.CornerBR, ForegroundColor, BackgroundColor);
            }
        }
    }
}
