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
    public class SMonsterBattleScreen : SScene
    {
        //---------------------------------------------------------------PRIVATE MEMBER VARIABLES----------------------------------------------------------------------
        private BattleManager _battleManager;

        private WStatusbar _statBarOpponent = new WStatusbar();
        private WStatusbar _statBarPlayer = new WStatusbar();
        private WMenu _battleMenu = new WMenu();
        private WMenu _actionMenu = new WMenu();
        private WDialogBox _dialogBox = new WDialogBox();
        private LLayoutManager.Slot _menuSlot = new LLayoutManager.Slot();

        private WMonster _player;
        private WMonster _opponent;

        //Panel animation variables
        private float _panelSpeed = 1.5f;
        private float _panelOpenPerc = 0.0f;
        private WPanel[] _panels = new WPanel[3];

        //Monster animation variables
        private IntVector2 _startOpponentPanel = new IntVector2(-40, 1);
        private IntVector2 _startPlayerPanel = new IntVector2(110, 25);
        private IntVector2 _destOpponentPanel = new IntVector2(56, 1);
        private IntVector2 _destPlayerPanel = new IntVector2(15, 25);
        private float _monsterPanelsSpeed = 0.6f;
        private float _monsterPanelsAlpha = 0.0f;

        //---------------------------------------------------------------PUBLIC MEMBER VARIABLES----------------------------------------------------------------------
        public WDialogBox DialogBox { get { return _dialogBox; } }
        public WMonster Player { get { return _player; } }
        public WMonster Opponent { get { return _opponent; } }

        //---------------------------------------------------------------PUBLIC METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        public SMonsterBattleScreen(BattleManager battleManager, WMonster player, WMonster opponent)
        {
            _battleManager = battleManager;
            _player = player;
            _opponent = opponent;
        }

        /// <summary>
        /// Shows the player menu
        /// </summary>
        public void ShowPlayerMenu()
        {
            _menuSlot.Widget = new WPanel()
                                .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_DOUBLE)
                                .SetBackgroundColor(ConsoleColor.Gray);

            _menuSlot.Widget.LayoutManager = new LFlowLayout()
                .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_HORIZONTAL)

                                    + new LLayoutManager.Slot()
                                    .SetExpandPercentage(new Vector2(0.5f, 1.0f))
                                    .SetMargin(new IntVector2(1, 1))
                                    [
                                        _battleMenu
                                    ]

                                    + new LLayoutManager.Slot()
                                    .SetExpandPercentage(new Vector2(0.5f, 1.0f))
                                    .SetMargin(new IntVector2(1, 1))
                                    [
                                       _actionMenu
                                    ];


            InputHandler.Instance.UIController.FocusedWidget = _battleMenu;
        }

        /// <summary>
        /// Shows the log dialog box
        /// </summary>
        public void ShowDialogBox()
        {
            _menuSlot.Widget = _dialogBox;
            InputHandler.Instance.UIController.FocusedWidget = _dialogBox;
        }

        public override void OnTick(float deltaSeconds)
        {
            if (_panelOpenPerc != 1.0f)
            {
                _panelOpenPerc = MathHelper.Clamp(_panelOpenPerc + _panelSpeed * deltaSeconds, 0.0f, 1.0f);

                _panels[0].LayoutSlot.ExpandPercentage = new Vector2(1.0f, (1.0f - _panelOpenPerc) / 2.0f);
                _panels[1].LayoutSlot.ExpandPercentage = new Vector2(1.0f, _panelOpenPerc);
                _panels[2].LayoutSlot.ExpandPercentage = new Vector2(1.0f, (1.0f - _panelOpenPerc) / 2.0f);
            }

            if (_monsterPanelsAlpha != 1.0f)
            {
                _monsterPanelsAlpha = MathHelper.Clamp(_monsterPanelsAlpha + _monsterPanelsSpeed * deltaSeconds, 0.0f, 1.0f);

                _player.LayoutSlot.RelativePosition = MathHelper.Lerp(_startPlayerPanel, _destPlayerPanel, _monsterPanelsAlpha);
                _opponent.LayoutSlot.LayoutManager.Parent.LayoutSlot.RelativePosition = MathHelper.Lerp(_startOpponentPanel, _destOpponentPanel, _monsterPanelsAlpha);
            }
            else
            {
                _statBarPlayer.IsVisible = true;
                _statBarOpponent.IsVisible = true;
            }
        }

        //---------------------------------------------------------------PROTECTED METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        protected override void OnSceneConstruction()
        {
            _menuSlot.ExpandPercentage = new Vector2(1.0f, 0.2f);

            _battleMenu
                .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_SINGLE_LIGHT)
                .AddWidget(new WButton().SetText("Attack").AddOnClick(btnAttackClick))
                .AddWidget(new WButton().SetText("Magic"));

            _actionMenu
                .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_SINGLE_LIGHT);

            _panels[0] = new WPanel();
            _panels[1] = new WPanel();
            _panels[2] = new WPanel();

            //Construct widget layout
            this.SetRoot(
                    new LAlignmentLayout()
                    + new LLayoutManager.Slot()
                    .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE)
                    .SetSize(new IntVector2(110, 55))
                    .SetMargin(new IntVector2(1, 1))
                    [
                        new WPanel()
                        .SetTransparency(true)
                        .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_DOUBLE)
                        .SetLayoutManager<LOverlayLayout>(new LOverlayLayout())

                        + new LLayoutManager.Slot()
                        [
                            new WPanel()
                            .SetTransparency(true)
                            .SetLayoutManager<LFlowLayout>(new LFlowLayout())
                            .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL)

                            + new LLayoutManager.Slot()
                            .SetExpandPercentage(new Vector2(1.0f, 0.8f))
                            [
                                new WPanel()
                                .SetBackgroundColor(ConsoleColor.White)
                                .SetLayoutManager<LCanvasLayout>(new LCanvasLayout())


                                    + new LLayoutManager.Slot()
                                    .SetPivot(LLayoutManager.EPivot.EPIVOT_TOP_LEFT)
                                    .SetRelativePosition(_startOpponentPanel)
                                    .SetSize(new IntVector2(40, 25))
                                    [
                                        new WPanel()
                                        .SetTransparency(true)
                                        .SetLayoutManager<LCanvasLayout>(new LCanvasLayout())

                                             + new LLayoutManager.Slot()
                                            .SetPivot(LLayoutManager.EPivot.EPIVOT_TOP_LEFT)
                                            .SetRelativePosition(new IntVector2(9, 0))
                                            .SetSize(new IntVector2(20, 20))
                                            .SetZOrder(1)
                                            [
                                                _opponent.SetShowFront(true)
                                            ]

                                            + new LLayoutManager.Slot()
                                            .SetPivot(LLayoutManager.EPivot.EPIVOT_TOP_LEFT)
                                            .SetRelativePosition(new IntVector2(0, 15))
                                            .SetSize(new IntVector2(40, 8))
                                            [
                                                new WPanel()
                                                .DrawImage("Images/Ground.png")
                                            ]
                                    ]

                                    + new LLayoutManager.Slot()
                                    .SetRelativePosition(new IntVector2(10, 1))
                                    .SetPivot(LLayoutManager.EPivot.EPIVOT_TOP_LEFT)
                                    [
                                        _statBarOpponent
                                        
                                        .SetMonster(_opponent.Status)
                                    ]

                                    + new LLayoutManager.Slot()
                                    .SetPivot(LLayoutManager.EPivot.EPIVOT_TOP_LEFT)
                                    .SetRelativePosition(_startPlayerPanel)
                                    .SetSize(new IntVector2(20, 20))
                                    [
                                        _player.SetShowFront(false)
                                    ]

                                    + new LLayoutManager.Slot()
                                    .SetRelativePosition(new IntVector2(70, 30))
                                    .SetPivot(LLayoutManager.EPivot.EPIVOT_TOP_LEFT)
                                    [
                                        _statBarPlayer
                                        .SetMonster(_player.Status)
                                    ]

                                    + new LLayoutManager.Slot()
                                    .SetRelativePosition(new IntVector2(0, 0))
                                    .SetPivot(LLayoutManager.EPivot.EPIVOT_TOP_LEFT)
                                    [
                                        new WFPSCounter()
                                    ]
                            ]
                            + _menuSlot
                            [
                                _dialogBox
                            ]
                        ]

                        + new LOverlayLayout.Slot()
                          [
                                new WPanel()
                                .SetTransparency(true)
                                .SetLayoutManager<LFlowLayout>(new LFlowLayout())
                                    .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL)

                                    + new LFlowLayout.Slot()
                                    .SetExpandPercentage(new Vector2(1.0f, 0.5f))
                                    [
                                        _panels[0]
                                        .SetBackgroundColor(ConsoleColor.Black)
                                    ]
                                    + new LFlowLayout.Slot()
                                    .SetExpandPercentage(new Vector2(1.0f, 0.0f))
                                    [
                                        _panels[1]
                                        .SetTransparency(true)
                                    ]
                                    + new LFlowLayout.Slot()
                                    .SetExpandPercentage(new Vector2(1.0f, 0.5f))
                                    [
                                        _panels[2]
                                        .SetBackgroundColor(ConsoleColor.Black)
                                    ]
                          ]
                    ]
                );

            _statBarOpponent.IsVisible = false;
            _statBarPlayer.IsVisible = false;

            _battleManager.StartBattle();
        }

        //---------------------------------------------------------------PRIVATE METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        private void btnAttackClick(WButton caller)
        {
            _battleManager.Next(new Actions.Action());
        }
    }
}
