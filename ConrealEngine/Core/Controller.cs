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

using ConUI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConrealEngine
{
    public class Controller
    {
        public delegate void InputEventDelegate();

        public Dictionary<string, InputEventDelegate> Controls { get; set; } = new Dictionary<string, InputEventDelegate>();

        public virtual void OnInputRecieved(string eventName, ConsoleKeyInfo pressedKey)
        {
            
        }
    }

    public class PlayerController : Controller
    {
        public override void OnInputRecieved(string eventName, ConsoleKeyInfo pressedKey)
        {
            if (Controls.ContainsKey(eventName) && Controls[eventName] != null)
            {
                Controls[eventName]();
            }
        }

        public void AddEvent(string eventName, InputEventDelegate func)
        {
            if(!Controls.ContainsKey(eventName))
            {
                Controls.Add(eventName, func);
            }
        }

        public void RemoveEvent(string eventName)
        {
            Controls.Remove(eventName);
        }
    }

    public class AIController : Controller
    {

    }

    public class UIController : Controller
    {
        private WWidget focusedWidget;
        private WWidget focusedWidgetBefore;
        private WTextInput focusedTextField = null;

        public WWidget FocusedWidget
        {
            get { return focusedWidget; }
            set
            {
                if (value == focusedWidget)
                    return;

                focusedWidgetBefore = focusedWidget;

                if (focusedWidget != null)
                {
                    UnfocusWidget();
                }

                focusedWidget = value;
                Controls = focusedWidget.OnRegister();
            }
        }

        public override void OnInputRecieved(string eventName, ConsoleKeyInfo pressedKey)
        {
            if(focusedTextField != null)
            {
                focusedTextField.OnInputRecieved(pressedKey);

                if(pressedKey.Key == ConsoleKey.Escape ||pressedKey.Key == ConsoleKey.Enter)
                {
                    StopTextInput();
                }
            }
            else
            {
                if (Controls.ContainsKey(eventName) && Controls[eventName] != null)
                {
                    Controls[eventName]();
                }
            }
        }

        public void UnfocusWidget()
        {
            Controls.Clear();
            focusedWidget = focusedWidgetBefore;
            
            if(focusedWidget != null)
            {
                Controls = focusedWidget.OnRegister();
            }
        }

        public void FocusTextInput(WTextInput txtField)
        {
            if(txtField != null)
                focusedTextField = txtField;
        }

        public void StopTextInput()
        {
            focusedTextField = null;
        }
    }
}
