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

using ConUI.Helper;
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
    public class WStatusbar : WWidget
    {
        //---------------------------------------------------------------PRIVATE MEMBER VARIABLES----------------------------------------------------------------------
        private WText txtHP = new WText();
        private WText txtMP = new WText();

        //---------------------------------------------------------------PUBLIC MEMBER VARIABLES----------------------------------------------------------------------
        public MonsterData MonsterStatus { get; set; }

        //---------------------------------------------------------------PUBLIC METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        public WStatusbar SetMonster(MonsterData m)
        {
            MonsterStatus = m;
            return this;
        }

        public override void OnConstruction()
        {
            this.Border = EBorderStyle.EBORDER_STYLE_SINGLE_ROUND_EDGE;
            this.LayoutSlot.SetSize(new IntVector2(25, 5));

            SetLayoutManager<LLayoutManager>((new LCanvasLayout())
                + new LCanvasLayout.Slot()
                .SetRelativePosition(new IntVector2(1, 1))
                .SetSize(new IntVector2(30, 1))
                .SetPivot(LLayoutManager.EPivot.EPIVOT_TOP_LEFT)
                [
                    new WText()
                    .SetText(MonsterStatus.Name)
                ]
                + new LCanvasLayout.Slot()
                .SetRelativePosition(new IntVector2(1, 2))
                .SetSize(new IntVector2(20, 1))
                .SetPivot(LLayoutManager.EPivot.EPIVOT_TOP_LEFT)
                [
                    new WPanel()
                    .SetLayoutManager<LFlowLayout>(new LFlowLayout())
                        .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_HORIZONTAL)

                        + new LFlowLayout.Slot()
                        .SetExpandPercentage(new Vector2(0.2f, 1.0f))
                        [
                            new WText()
                            .SetText("HP: ")
                        ]
                        + new LFlowLayout.Slot()
                        .SetExpandPercentage(new Vector2(0.8f, 1.0f))
                        [
                            txtHP
                            .SetText(MonsterStatus.CurrentHealth + " / " + MonsterStatus.MaxHealth)
                        ]
                ]
                + new LCanvasLayout.Slot()
                .SetRelativePosition(new IntVector2(1, 3))
                .SetSize(new IntVector2(20, 1))
                .SetPivot(LLayoutManager.EPivot.EPIVOT_TOP_LEFT)
                [
                    new WPanel()
                    .SetLayoutManager<LFlowLayout>(new LFlowLayout())
                        .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_HORIZONTAL)

                        + new LFlowLayout.Slot()
                        .SetExpandPercentage(new Vector2(0.2f, 1.0f))
                        [
                            new WText()
                            .SetText("MP: ")
                        ]
                        + new LFlowLayout.Slot()
                        .SetExpandPercentage(new Vector2(0.8f, 1.0f))
                        [
                            txtMP
                            .SetText(MonsterStatus.CurrentMana + " / " + MonsterStatus.MaxMana)
                        ]
                ]
             );

            base.OnConstruction();
        }

        //---------------------------------------------------------------PROTECTED METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        protected override void OnPreDraw()
        {
            base.OnPreDraw();

            txtHP.Text = MonsterStatus.CurrentHealth + " / " + MonsterStatus.MaxHealth;
            txtMP.Text = MonsterStatus.CurrentMana + " / " + MonsterStatus.MaxMana;
        }
    }
}
