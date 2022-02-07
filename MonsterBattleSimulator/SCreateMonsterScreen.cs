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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonsterBattleSimulator
{
    public class SCreateMonsterScreen : SScene
    {
        //---------------------------------------------------------------PUBLIC MEMBER VARIABLES----------------------------------------------------------------------
        public MonsterData Monster { get; set; }

        //---------------------------------------------------------------PUBLIC METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        public SCreateMonsterScreen()
        {
            Monster = null;
        }

        public SCreateMonsterScreen(MonsterData m)
        {
            Monster = m;
        }

        //---------------------------------------------------------------PROTECTED METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        /// <summary>
        /// Construction method of this scene
        /// </summary>
        protected override void OnSceneConstruction()
        {
            //Construct widget layout
            this.SetRoot(
                    new LAlignmentLayout()
                    + new LLayoutManager.Slot()
                    .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE)
                    .SetSize(new IntVector2(50, 33))
                    .SetMargin(new IntVector2(1, 1))
                    [
                        new WPanel()
                        .SetBackgroundColor(ConsoleColor.White)
                        .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_DOUBLE)
                        .SetLayoutManager<LFlowLayout>(new LFlowLayout())
                            .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL)

                            + new LLayoutManager.Slot()
                            .SetExpandPercentage(new Vector2(0.5f, 0.15f))
                            .SetMargin(new IntVector2(1, 1))
                            [
                                new WText()
                                .SetText(Monster != null ? "EDIT YOUR MONSTER" : "CREATE YOUR MONSTER")
                                .SetBackgroundColor(ConsoleColor.White)
                                .SetForegroundColor(ConsoleColor.DarkMagenta)
                                .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_SINGLE_ROUND_EDGE)
                            ]

                            + new LLayoutManager.Slot()
                            .SetExpandPercentage(new Vector2(0.5f, 0.85f))
                            //.SetMargin(new IntVector2(1, 1))
                            [
                               new WPanel()
                                .SetLayoutManager<LFlowLayout>(new LFlowLayout())
                                    .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_HORIZONTAL)

                                    + new LFlowLayout.Slot()
                                    .SetExpandPercentage(new Vector2(0.5f, 1.0f))
                                    [
                                        new WPanel()
                                            .SetLayoutManager<LFlowLayout>(new LFlowLayout())
                                                .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL)
                                                .SetFlowSizeManagement(LFlowLayout.EFlowSizeManagement.EFLOW_SIZE_MANAGEMENT_ABSOLUTE)
                                                .SetElementSize(3)

                                                + new LFlowLayout.Slot()
                                                [
                                                    new WText()
                                                    .SetText("Name:")
                                                ]
                                                + new LFlowLayout.Slot()
                                                [
                                                    new WText()
                                                    .SetText("Typ:")
                                                ]
                                                + new LFlowLayout.Slot()
                                                [
                                                    new WText()
                                                    .SetText("Health")
                                                ]
                                                + new LFlowLayout.Slot()
                                                [
                                                    new WText()
                                                    .SetText("Mana")
                                                ]
                                                + new LFlowLayout.Slot()
                                                [
                                                    new WText()
                                                    .SetText("Strength")
                                                ]
                                                + new LFlowLayout.Slot()
                                                [
                                                    new WText()
                                                    .SetText("Defense")
                                                ]
                                                + new LFlowLayout.Slot()
                                                [
                                                    new WText()
                                                    .SetText("Agility")
                                                ]
                                    ]
                                    + new LFlowLayout.Slot()
                                    .SetExpandPercentage(new Vector2(0.5f, 1.0f))
                                    [
                                         new WMenu()
                                         .SetFlow(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_VERTICAL)
                                         .SetFlowSizeManagement(LFlowLayout.EFlowSizeManagement.EFLOW_SIZE_MANAGEMENT_ABSOLUTE)
                                         .SetElementSize(3)
                                         
                                         .AddWidget(new WTextInput().AddOnTextChanged(txtNameChanged).AddOnTextValidation(txtNameCheck).SetTextWithoutExternalCheck(false, Monster != null ? Monster.Name : ""))
                                         .AddWidget(new WButton().SetText(Monster != null ? Monster.TypeToString() : "Ork").AddOnClick(btnTypClick))
                                         .AddWidget(new WTextInput().AddOnTextChanged(txtHealthChanged).AddOnTextValidation(txtHealthCheck)
                                                .SetTextWithoutExternalCheck(true, Monster != null ? Monster.MaxHealth.ToString() : "0")
                                            )
                                         .AddWidget(new WTextInput().AddOnTextChanged(txtManaChanged).AddOnTextValidation(txtManaCheck)
                                                .SetTextWithoutExternalCheck(true, Monster != null ? Monster.MaxMana.ToString() : "0")
                                            )
                                         .AddWidget(new WTextInput().AddOnTextChanged(txtAttackChanged).AddOnTextValidation(txtAttackCheck)
                                                .SetTextWithoutExternalCheck(true, Monster != null ? Monster.Strength.ToString() : "0")
                                            )
                                         .AddWidget(new WTextInput().AddOnTextChanged(txtDefenseChanged).AddOnTextValidation(txtDefenseCheck).SetNumericOnly(true)
                                                .SetTextWithoutExternalCheck(true, Monster != null ? Monster.Defense.ToString() : "0")
                                            )
                                         .AddWidget(new WTextInput().AddOnTextChanged(txtAgilityChanged).AddOnTextValidation(txtAgilityCheck).SetNumericOnly(true)
                                                .SetTextWithoutExternalCheck(true, Monster != null ? Monster.Agility.ToString() : "0")
                                            )
                                         .AddWidget(new WButton().SetText(Monster != null ? "Apply" : "Create").AddOnClick(btnCreateClick))
                                         .AddWidget(new WButton().SetText("Cancel").AddOnClick(btnCancelClick))
                                    ]
                            ]
                    ]
                );

            if(Monster == null)
            {
                Monster = new MonsterData();
            }
        }

        //---------------------------------------------------------------PRIVATE METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        private bool txtManaCheck(string str)
        {
            int i = (int)float.Parse(str);

            return i > 0;
        }

        private void txtManaChanged(string str)
        {
            int i = (int)float.Parse(str);

            Monster.MaxMana = i;
        }

        private void btnCancelClick(WButton btn)
        {
            Renderer.Instance.SetScene(new SChooseCharacterScreen());
        }

        private void btnCreateClick(WButton btn)
        {
            if(!MonsterLib.MonsterList.Contains(Monster))
            {
                MonsterLib.MonsterList.Add(Monster);
            }

            MonsterLib.SaveLib();

            Renderer.Instance.SetScene(new SChooseCharacterScreen());
        }

        private void btnTypClick(WButton btn)
        {
            switch(btn.Text.Text)
            {
                case "Ork":
                    btn.Text.Text = "Troll";
                    Monster.Type = MonsterData.EMonsterType.EMONSTER_TYPE_TROLL;
                    break;
                case "Troll":
                    btn.Text.Text = "Goblin";
                    Monster.Type = MonsterData.EMonsterType.EMONSTER_TYPE_GOBLIN;
                    break;
                case "Goblin":
                    btn.Text.Text = "Ork";
                    Monster.Type = MonsterData.EMonsterType.EMONSTER_TYPE_ORK;
                    break;
            }

            //btn.ExecDraw();
        }

        private void txtNameChanged(string txt)
        {
            Monster.Name = txt;
        }

        private void txtHealthChanged(string txt)
        {
            int i = (int)float.Parse(txt);

            Monster.MaxHealth = i;
        }

        private void txtAttackChanged(string txt)
        {
            int i = (int)float.Parse(txt);

            Monster.Strength = i;
        }

        private void txtDefenseChanged(string txt)
        {
            int i = (int)float.Parse(txt);

            Monster.Defense = i;
        }

        private void txtAgilityChanged(string txt)
        {
            int i = (int)float.Parse(txt);

            Monster.Agility = i;
        }

        private bool txtNameCheck(string txt)
        {
            return true;
        }

        private bool txtHealthCheck(string txt)
        {
            int i = (int)float.Parse(txt);

            return i > 0;
        }

        private bool txtAttackCheck(string txt)
        {
            int i = (int)float.Parse(txt);

            return i > 0;
        }

        private bool txtDefenseCheck(string txt)
        {
            int i = (int)float.Parse(txt);

            return i > 0;
        }

        private bool txtAgilityCheck(string txt)
        {
            int i = (int)float.Parse(txt);

            return i > 0;
        }
    }
}
