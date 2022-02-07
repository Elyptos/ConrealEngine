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
using ConUI.Widgets.Layout;
using System.Numerics;
using ConUI.Helper;

namespace ConUI.Widgets
{
    public class WButton : WWidget
    {
        public delegate void ButtonDelegate(WButton caller);

        public WText Text { get; set; } = new WText();
        public EBorderStyle BorderFocused { get; set; } = EBorderStyle.EBORDER_STYLE_SINGLE_ROUND_EDGE;
        public EBorderStyle BorderDefocused { get; set; } = EBorderStyle.EBORDER_STYLE_SINGLE_ROUND_EDGE;
        public ConsoleColor BGColorFocused { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor BGColorDefocused { get; set; } = ConsoleColor.Gray;
        public ConsoleColor FGColorFocused { get; set; } = ConsoleColor.Black;
        public ConsoleColor FGColorDefocused { get; set; } = ConsoleColor.Black;

        public event ButtonDelegate OnClick;
        public event ButtonDelegate OnFocus;
        public event ButtonDelegate OnDefocus;

        private bool isFocused = false;

        public WButton SetText(string txt)
        {
            Text.Text = txt;
            return this;
        }

        public WButton SetStyleFocused(EBorderStyle border, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            BorderFocused = border;
            BGColorFocused = backgroundColor;
            FGColorFocused = foregroundColor;

            return this;
        }

        public WButton SetStyleDefocused(EBorderStyle border, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            BorderDefocused = border;
            BGColorDefocused = backgroundColor;
            FGColorDefocused = foregroundColor;

            return this;
        }

        public WButton AddOnClick(ButtonDelegate func)
        {
            OnClick += func;

            return this;
        }

        public WButton AddOnFocus(ButtonDelegate func)
        {
            OnFocus += func;

            return this;
        }

        public WButton AddOnDefocus(ButtonDelegate func)
        {
            OnDefocus += func;

            return this;
        }

        public override void OnConstruction()
        {
            this.LayoutSlot.Margin = new IntVector2(1, 1);

            SetLayoutManager<LAlignmentLayout>(new LAlignmentLayout());

            LayoutManager.AddSlot(
                new LAlignmentLayout.Slot()
                .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_FILL)
                .SetPivot(LLayoutManager.EPivot.EPIVOT_CENTER)
                .SetHorizontalAlignment(LLayoutManager.EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_CENTER)
                .SetVerticalAlignment(LLayoutManager.EVerticalAlignment.EVERTICAL_ALIGNMENT_CENTER)
                [
                    Text
                ]
            );

            if(!isFocused)
            {
                Text.ForegroundColor = FGColorDefocused;
                Text.BackgroundColor = BGColorDefocused;
                this.Border = BorderDefocused;
                this.BackgroundColor = BGColorDefocused;
            }

            base.OnConstruction();
        }

        protected override void OnDraw(RenderHandle r, Box drawingArea)
        {
            Box widgetArea = GetDrawingArea(drawingArea);

            if(widgetArea.IsValid())
            {
                r.DrawMany(' ', widgetArea.CornerTL, widgetArea.CornerBR, ForegroundColor, BackgroundColor);
            }

            base.OnDraw(r, drawingArea);
        }

        public override void OnUserFocus()
        {
            isFocused = true;
            Text.ForegroundColor = FGColorFocused;
            this.Border = BorderFocused;
            this.BackgroundColor = BGColorFocused;
            Text.BackgroundColor = BGColorFocused;

            //ExecDraw();

            OnFocus?.Invoke(this);
        }

        public override void OnUserDefocus()
        {
            isFocused = false;
            Text.ForegroundColor = FGColorDefocused;
            this.Border = BorderDefocused;
            this.BackgroundColor = BGColorDefocused;
            Text.BackgroundColor = BGColorDefocused;

            //ExecDraw();

            OnDefocus?.Invoke(this);
        }

        public override void OnUserActivate()
        {
            OnClick?.Invoke(this);
        }
    }
}
