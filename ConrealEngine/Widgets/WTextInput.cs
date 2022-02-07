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
using System.Text.RegularExpressions;
using ConUI.Widgets.Layout;
using ConUI.Helper;

namespace ConUI.Widgets
{
    public class WTextInput : WWidget
    {
        public delegate void TextChangedDelegate(string str);
        public delegate bool TextValidationDelegate(string str);

        public event TextChangedDelegate OnTextChanged;
        public event TextValidationDelegate OnValidateText;

        public WButton Button { get; set; } = new WButton();

        public String Text { get { return Button.Text.Text; }set { Button.Text.Text = value; } }

        public bool NumericOnly
        {
            get { return numericOnly; }
            set
            {
                if(value)
                    Text = "0";

                numericOnly = value;
            }
        }

        private String textBackup = "";
        private bool duringEdit = false;
        private bool numericOnly = false;

        public WTextInput AddOnTextChanged(TextChangedDelegate del)
        {
            OnTextChanged += del;
            return this;
        }

        public WTextInput AddOnTextValidation(TextValidationDelegate del)
        {
            OnValidateText += del;
            return this;
        }

        //This method should not be called while under construction
        public void SetText(string txt)
        {
            if (PendingConstruction)
                return;

            if (!ValidateText(txt))
                return;

            if (OnValidateText != null && !OnValidateText.Invoke(Text))
                return;

            Text = txt;
        }

        public WTextInput SetTextWithoutExternalCheck(bool numericOnly, string txt)
        {
            NumericOnly = numericOnly;

            if (ValidateText(txt))
                Text = txt;

            return this;
        }

        public WTextInput SetNumericOnly(bool b)
        {
            NumericOnly = b;
            return this;
        }

        public void OnInputRecieved(ConsoleKeyInfo info)
        {
            if(duringEdit == false)
            {
                textBackup = Text;
                duringEdit = true;
            }

            switch(info.Key)
            {
                case ConsoleKey.Backspace:
                    if(Text.Length > 0)
                    {
                        Text = Text.Remove(Text.Length - 1);
                    }
                    break;
                case ConsoleKey.Escape:
                    duringEdit = false;
                    Text = textBackup;
                    break;
                case ConsoleKey.Enter:
                    if (NumericOnly && String.IsNullOrEmpty(Text))
                        Text = "0";

                    if(OnValidateText != null)
                    {
                        if(OnValidateText.Invoke(Text))
                        {
                            duringEdit = false;
                            OnTextChanged?.Invoke(Text);
                        }
                        else
                        {
                            duringEdit = false;
                            Text = textBackup;
                        }
                    }
                    else
                    {
                        duringEdit = false;
                        OnTextChanged?.Invoke(Text);
                    }
                   
                    break;
                default:
                    Regex reg = new Regex("^-?\\d+\\.?\\d*$");

                    string nString = Text + info.KeyChar;

                    if(ValidateText(nString))
                    {
                        Text = nString;
                    }

                    break;
            }

            //ExecDraw();
        }

        public override void OnConstruction()
        {
            Button.BorderFocused = EBorderStyle.EBORDER_STYLE_SINGLE_HEAVY;
            Button.BorderDefocused = EBorderStyle.EBORDER_STYLE_SINGLE_LIGHT;
            Button.BGColorFocused = ConsoleColor.White;
            Button.BGColorDefocused = ConsoleColor.White;

            SetLayoutManager<LAlignmentLayout>(new LAlignmentLayout());

            LayoutManager.AddSlot(
                 new LAlignmentLayout.Slot()
                    .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_FILL)
                    .SetPivot(LLayoutManager.EPivot.EPIVOT_CENTER)
                    .SetHorizontalAlignment(LLayoutManager.EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_CENTER)
                    .SetVerticalAlignment(LLayoutManager.EVerticalAlignment.EVERTICAL_ALIGNMENT_CENTER)
                    //.SetMargin(new System.Numerics.IntVector2(1.0f, 1.0f))
                    [
                        Button
                    ]
                );

            base.OnConstruction();
        }

        public override void OnUserActivate()
        {
            InputHandler.Instance.UIController.FocusTextInput(this);
        }

        public override void OnUserDefocus()
        {
            Button.OnUserDefocus();
        }

        public override void OnUserFocus()
        {
            Button.OnUserFocus();
        }

        protected override void OnDraw(Renderer.RenderHandle r, Box drawingArea)
        {
            base.OnDraw(r, drawingArea);
        }

        protected override void OnPostDraw()
        {
            base.OnPostDraw();
        }

        protected override void OnPreDraw()
        {
            base.OnPreDraw();
        }

        private bool ValidateText(string toCheck)
        {
            Regex reg = new Regex("^-?\\d+\\.?\\d*$");

            if (String.IsNullOrWhiteSpace(toCheck))
                return false;

            if (NumericOnly)
            {
                 return reg.IsMatch(toCheck);
            }
            else
            {
                return true;
            }
        }
    }
}
