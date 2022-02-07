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
using ConUI.Widgets.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConUI.Widgets
{
    class WPopupWidget : WPanel
    {
        private WWidget caller;

        public string InformationText { get; set; }
        public string ApplyButtonText { get; set; }
        public string CancelButtonText { get; set; }

        public event WButton.ButtonDelegate OnApply;

        public WPopupWidget(WWidget caller)
        {
            this.caller = caller;
        }

        public WPopupWidget SetInfoText(string txt)
        {
            InformationText = txt;
            return this;
        }

        public WPopupWidget SetCancelText(string txt)
        {
            CancelButtonText = txt;
            return this;
        }

        public WPopupWidget SetApplyText(string txt)
        {
            ApplyButtonText = txt;
            return this;
        }

        public WPopupWidget AddOnApply(WButton.ButtonDelegate func)
        {
            OnApply += func;
            return this;
        }

        public override void OnConstruction()
        {
            this.LayoutSlot.SetMargin(new Helper.IntVector2(1, 1));
            this.Border = EBorderStyle.EBORDER_STYLE_DOUBLE;

            this.SetLayoutManager(
                new LFlowLayout()
                    .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL)
                    + new LLayoutManager.Slot()
                    .SetExpandPercentage(new System.Numerics.Vector2(1.0f, 0.5f))
                    [
                        new WText()
                        .SetTextAlignment(WText.ETextAlignment.ETEXT_ALIGNMENT_CENTER)
                        .SetBackgroundColor(ConsoleColor.Gray)
                        .SetText(InformationText)
                    ]
                    + new LLayoutManager.Slot()
                    .SetExpandPercentage(new System.Numerics.Vector2(1.0f, 0.5f))
                    [
                        new WMenu()
                        .SetFlow(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_HORIZONTAL)
                        .AddWidget(new WButton().SetText(CancelButtonText).AddOnClick(OnCancelClick))
                        .AddWidget(new WButton().SetText(ApplyButtonText).AddOnClick(OnApplyClick))
                    ]
                );

            base.OnConstruction();
        }

        private void OnCancelClick(WButton btn)
        {
            caller.HidePopup();
        }

        private void OnApplyClick(WButton btn)
        {
            caller.HidePopup();

            OnApply?.Invoke(btn);
        }

        public override void OnDestroy()
        {
            //InputHandler.Instance.UIController.UnfocusWidget();
        }
    }
}
