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
using ConUI.Helper;

namespace ConUI.Widgets
{
    public class WMenu : WWidget
    {
        public delegate Dictionary<string, Controller.InputEventDelegate> RegisterDelegate();

        public List<WWidget> Widgets { get; set; } = new List<WWidget>();
        public LFlowLayout.EFlowAlignment FlowAlignment { get; set; } = LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL;
        public LFlowLayout.EFlowSizeManagement FlowSizeManagement { get; set; } = LFlowLayout.EFlowSizeManagement.EFLOW_SIZE_MANAGEMENT_FIT;
        public int ElementSize { get; set; } = 3;

        private WPanel panel = new WPanel();

        private int selectionIndex = 0;

        public event RegisterDelegate OnAdditionalRegister;

        public WMenu AddOnAdditionalRegister(RegisterDelegate func)
        {
            OnAdditionalRegister += func;
            return this;
        }

        public WMenu SetFlowSizeManagement(LFlowLayout.EFlowSizeManagement m)
        {
            FlowSizeManagement = m;
            return this;
        }

        public WMenu SetElementSize(int i)
        {
            ElementSize = i;
            return this;
        }

        public WMenu AddWidget(WWidget w)
        {
            Widgets.Add(w);

            return this;
        }

        public WMenu DrawBorder(EBorderStyle border)
        {
            this.Border = border;
            return this;
        }

        public WMenu SetFlow(LFlowLayout.EFlowAlignment flow)
        {
            FlowAlignment = flow;

            return this;
        }

        public WMenu SetBackgroundColor(ConsoleColor cl)
        {
            panel.BackgroundColor = cl;
            return this;
        }

        public WWidget GetSelectedWidget()
        {
            return Widgets[selectionIndex];
        }

        public override void OnConstruction()
        {
            if (Widgets.Count == 0)
                return;

            Widgets[selectionIndex].OnUserFocus();

            float expPerc = 1.0f / Widgets.Count;

            SetLayoutManager<LLayoutManager>((new LAlignmentLayout())
                + new LAlignmentLayout.Slot()
                    .SetPivot(LLayoutManager.EPivot.EPIVOT_CENTER)
                    .SetHorizontalAlignment(LLayoutManager.EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_CENTER)
                    .SetVerticalAlignment(LLayoutManager.EVerticalAlignment.EVERTICAL_ALIGNMENT_CENTER)
                    .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_FILL)
                    [
                        panel
                    ]
                );

            LLayoutManager panelLayout = panel.SetLayoutManager<LFlowLayout>((new LFlowLayout())
                .SetFlowAlginment(FlowAlignment)
                .SetElementSize(ElementSize)
                .SetFlowSizeManagement(FlowSizeManagement));

            foreach(WWidget w in Widgets)
            {
                panelLayout = panelLayout
                    + new LFlowLayout.Slot()
                    .SetExpandPercentage(new System.Numerics.Vector2(FlowAlignment == LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL ? 1.0f : expPerc
                        , FlowAlignment == LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL ? expPerc : 1.0f))
                    //.SetMargin(new System.Numerics.IntVector2(1.0f, 1.0f))
                    [
                        w
                    ];
            }

            InputHandler.Instance.UIController.FocusedWidget = this;

            base.OnConstruction();
        }

        public override Dictionary<string, Controller.InputEventDelegate> OnRegister()
        {
            Dictionary<string, Controller.InputEventDelegate> lib = new Dictionary<string, Controller.InputEventDelegate>();
            Dictionary<string, Controller.InputEventDelegate> additional = new Dictionary<string, Controller.InputEventDelegate>();

            switch (FlowAlignment)
            {
                case LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL:
                    lib.Add("Up", OnUp);
                    lib.Add("Down", OnDown);

                    break;
                case LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_HORIZONTAL:
                    lib.Add("Left", OnUp);
                    lib.Add("Right", OnDown);

                    break;
            }

            lib.Add("Accept", OnEnter);

            if(OnAdditionalRegister != null)
            {
                additional = OnAdditionalRegister.Invoke();

                foreach (var elem in additional)
                {
                    if(!lib.ContainsKey(elem.Key))
                    {
                        lib.Add(elem.Key, elem.Value);
                    }
                }
            }

            return lib;
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

        private void OnUp()
        {
            if(Widgets.Count != 0)
            {
                Widgets[selectionIndex].OnUserDefocus();

                selectionIndex--;

                if (selectionIndex < 0)
                {
                    selectionIndex = Widgets.Count - 1;
                }

                Widgets[selectionIndex].OnUserFocus();
            }
        }

        private void OnDown()
        {
            if(Widgets.Count != 0)
            {
                Widgets[selectionIndex].OnUserDefocus();

                selectionIndex++;

                if(selectionIndex >= Widgets.Count)
                {
                    selectionIndex = 0;
                }

                Widgets[selectionIndex].OnUserFocus();
            }
        }

        private void OnEnter()
        {
            if(Widgets.Count != 0)
            {
                Widgets[selectionIndex].OnUserActivate();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            InputHandler.Instance.UIController.UnfocusWidget();
        }
    }
}
