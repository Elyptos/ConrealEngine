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
using ConUI.Widgets.Layout;

namespace ConUI.Widgets
{
    public class WDialogBox : WPanel
    {
        private List<string> _dialog = new List<string>();
        private WText _textWidget = new WText();

        public delegate void DialogFinishedDelegate();

        public float TextSpeed { get; set; }

        public List<string> Dialog
        {
            get { return _dialog; }
            set
            {
                _dialog = value;
                currTime = 0.0f;
                strIndexPos = 0;
                txtIndex = 0;
                txtFinished = false;
                nextPageAnim = false;
            }
        }

        private float currTime = 0.0f;
        private int strIndexPos = 0;
        private int txtIndex = 0;

        private bool txtFinished = false;
        private bool nextPageAnim = false;

        public event DialogFinishedDelegate OnDialogFinished;

        public WDialogBox SetTextSpeed(float f)
        {
            TextSpeed = f;
            return this;
        }

        public new WDialogBox SetBackgroundColor(ConsoleColor c)
        {
            this.BackgroundColor = c;
            this._textWidget.BackgroundColor = c;
            return this;
        }

        public WDialogBox SetDialog(List<string> txt)
        {
            Dialog = txt;
            return this;
        }

        public WDialogBox AddOnDialogFinished(DialogFinishedDelegate func)
        {
            OnDialogFinished += func;
            return this;
        }

        public override void OnTick(float deltaSeconds)
        {
            base.OnTick(deltaSeconds);

            if (Dialog.Count == 0)
                return;

            if(txtFinished)
            {
                currTime += deltaSeconds;

                if(currTime >= 0.2f)
                {
                    currTime = 0;

                    nextPageAnim = !nextPageAnim;

                    if(nextPageAnim)
                    {
                        _textWidget.Text = this.Dialog[txtIndex] + " ▾";
                    }
                    else
                    {
                        _textWidget.Text = this.Dialog[txtIndex] + " ▼";
                    }
                }
            }
            else
            {
                currTime += deltaSeconds;

                if (currTime >= TextSpeed && strIndexPos < Dialog[txtIndex].Length)
                {
                    _textWidget.Text = Dialog[txtIndex].Substring(0, strIndexPos + 1);

                    currTime = 0;
                    strIndexPos++;

                    if (strIndexPos >= Dialog[txtIndex].Length)
                        txtFinished = true;
                }
            }
        }

        protected override bool ShouldTick()
        {
            return true;
        }

        public override void OnConstruction()
        {
            this.DrawBorder(EBorderStyle.EBORDER_STYLE_SINGLE_LIGHT);
            this.SetBackgroundColor(ConsoleColor.Gray);
            this._textWidget.ForegroundColor = ConsoleColor.Black;

            this.SetLayoutManager<LLayoutManager>(new LAlignmentLayout()
                    + new LAlignmentLayout.Slot()
                    .SetMargin(new Helper.IntVector2(1, 1))
                    [
                        _textWidget
                        .SetAutoLine(true)
                    ]
                );

            base.OnConstruction();
        }

        public override Dictionary<string, Controller.InputEventDelegate> OnRegister()
        {
            Dictionary<string, Controller.InputEventDelegate> res = new Dictionary<string, Controller.InputEventDelegate>();

            res.Add("Accept", OnEnter);

            return res;
        }

        private void OnEnter()
        {
            if(txtFinished && (txtIndex + 1) < Dialog.Count)
            {
                txtFinished = false;
                txtIndex++;
                strIndexPos = 0;
            }
            else if(!txtFinished && txtIndex < Dialog.Count)
            {
                txtFinished = true;
                _textWidget.Text = Dialog[txtIndex];
            }
            else if(txtFinished)
            {
                OnDialogFinished?.Invoke();
            }
        }
    }
}
