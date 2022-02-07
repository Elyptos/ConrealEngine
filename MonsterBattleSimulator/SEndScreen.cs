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
using ConUI.Helper;
using ConUI.Scenes;
using ConUI.Widgets;
using ConUI.Widgets.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonsterBattleSimulator
{
    public class SEndScreen : SScene
    {
        //---------------------------------------------------------------PRIVATE MEMBER VARIABLES----------------------------------------------------------------------
        private WText titel = new WText();
        private bool playerWon = false;

        //---------------------------------------------------------------PUBLIC METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        public SEndScreen(bool playerWon)
        {
            this.playerWon = playerWon;
        }

        protected override void OnSceneConstruction()
        {
            this.SetRoot(
                new LAlignmentLayout()
                    + new LLayoutManager.Slot()
                    .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE)
                    .SetSize(new IntVector2(180, 40))
                    .SetMargin(new IntVector2(1, 1))
                    [
                        new WPanel()
                        .SetBackgroundColor(ConsoleColor.White)
                        .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_DOUBLE)
                        .SetLayoutManager<LFlowLayout>(new LFlowLayout())
                            .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL)

                            + new LLayoutManager.Slot()
                            .SetExpandPercentage(new Vector2(0.5f, 0.65f))
                            .SetMargin(new IntVector2(1, 1))
                            [
                                new WText()
                                .SetText(playerWon ? "You win" : "You loose")
                                .SetTextAlignment(WText.ETextAlignment.ETEXT_ALIGNMENT_CENTER)
                                .SetForegroundColor(ConsoleColor.Black)
                                .SetFontSampler(WText.EFontSampler.FONT_SAMPLER_CONREAL)
                                .SetFontSize(16)
                                .SetFont(playerWon ? "Consolas" : "Comic Sans MS")
                            ]

                            + new LLayoutManager.Slot()
                            .SetExpandPercentage(new Vector2(0.5f, 0.35f))
                            .SetMargin(new IntVector2(1, 1))
                            [
                                new WPanel()
                                .SetTransparency(true)
                                .SetLayoutManager<LAlignmentLayout>(new LAlignmentLayout())
                                    + new LAlignmentLayout.Slot()
                                    .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE)
                                    .SetMargin(new IntVector2(1, 1))
                                    .SetSize(new IntVector2(20, 10))
                                    [
                                        new WMenu()
                                        .SetFlow(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL)
                                        .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_SINGLE_DASH)
                                        .AddWidget
                                        (
                                            new WButton()
                                            .SetText("New Battle")
                                            .AddOnClick(OnClickNewBattle)
                                        )
                                        .AddWidget
                                        (
                                            new WButton()
                                            .SetText("Quit")
                                            .AddOnClick(OnClickQuit)
                                        )
                                    ]
                            ]
                    ]
                );
        }

        private void OnClickQuit(WButton caller)
        {
            Engine.Instance.Stop();
        }

        private void OnClickNewBattle(WButton caller)
        {
            Renderer.Instance.SetScene(new SChooseCharacterScreen());
        }
    }
}
