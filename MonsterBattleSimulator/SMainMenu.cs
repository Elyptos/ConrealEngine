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
using ConUI.Widgets.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ConUI.Scenes;
using ConUI.Helper;
using ConrealEngine;

namespace MonsterBattleSimulator
{
    public class SMainMenu : SScene
    {
        //---------------------------------------------------------------PROTECTED METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        protected override void OnSceneCleanup()
        {
            
        }

        protected override void OnSceneConstruction()
        {
            this.SetRoot(
                new LAlignmentLayout()
                    + new LLayoutManager.Slot()
                    .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE)
                    .SetSize(new IntVector2(30, 18))
                    .SetMargin(new IntVector2(1, 1))
                    [
                        new WPanel()
                        .SetBackgroundColor(ConsoleColor.White)
                        .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_DOUBLE)
                        .SetLayoutManager<LFlowLayout>(new LFlowLayout())
                            .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL)

                            + new LLayoutManager.Slot()
                            .SetExpandPercentage(new Vector2(0.5f, 0.25f))
                            .SetMargin(new IntVector2(1, 1))
                            [
                                new WText()
                                .SetText("Monster Fighter")
                                .SetBackgroundColor(ConsoleColor.White)
                                .SetForegroundColor(ConsoleColor.DarkMagenta)
                                .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_SINGLE_ROUND_EDGE)
                            ]

                            + new LLayoutManager.Slot()
                            .SetExpandPercentage(new Vector2(0.5f, 0.75f))
                            .SetMargin(new IntVector2(1, 1))
                            [
                               new WMenu()
                                .SetFlow(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL)
                                .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_SINGLE_DASH)
                                .AddWidget((new WButton())
                                    .SetText("Start Game")
                                    .AddOnClick(OnClickNewGame)
                                )
                                .AddWidget((new WButton())
                                    .SetText("Options")
                                    .AddOnClick(OnClickOptions)
                                )
                                .AddWidget((new WButton())
                                    .SetText("Exit Game")
                                    .AddOnClick(OnClickExitGame)
                                )
                            ]
                    ]
                );
        }

        //---------------------------------------------------------------PRIVATE METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        private void OnClickNewGame(WButton btn)
        {
            Renderer.Instance.SetScene(new SChooseCharacterScreen());
        }

        private void OnClickLoadGame(WButton btn)
        {

        }

        private void OnClickOptions(WButton btn)
        {

        }

        private void OnClickExitGame(WButton btn)
        {
            Engine.Instance.Stop();
        }
    }
}
