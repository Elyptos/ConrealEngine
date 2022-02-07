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
using ConUI.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ConUI.Helper;
using ConrealEngine;

namespace MonsterBattleSimulator
{
    public class SChooseCharacterScreen : SScene
    {
        //---------------------------------------------------------------PRIVATE MEMBER VARIABLES----------------------------------------------------------------------
        private const int MAX_MONSTER_COUNT = 7;

        private WMenu menu = new WMenu();
        private WPanel statsPanel = new WPanel();

        private WText titel = new WText();

        private WText txtName = new WText();
        private WText txtTyp = new WText();
        private WText txtHealth = new WText();
        private WText txtMana = new WText();
        private WText txtDefense = new WText();
        private WText txtAttack = new WText();
        private WText txtAgility = new WText();

        private MonsterData player = null;
        private MonsterData opponent = null;

        //---------------------------------------------------------------PROTECTED METHODS AND CONSTRUCTORS----------------------------------------------------------------------

        /// <summary>
        /// Construction function of this scene
        /// </summary>
        protected override void OnSceneConstruction()
        {
            List<ConsoleKey> acceptKeys = InputHandler.Instance.UIControls.Where(x => x.Value == "Accept").Select(x => x.Key).ToList();
            List<ConsoleKey> editKeys = InputHandler.Instance.UIControls.Where(x => x.Value == "Edit").Select(x => x.Key).ToList();
            List<ConsoleKey> deleteKeys = InputHandler.Instance.UIControls.Where(x => x.Value == "Delete").Select(x => x.Key).ToList();

            //Construct widget layout
            this.SetRoot(
                    new LAlignmentLayout()
                    + new LLayoutManager.Slot()
                    .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE)
                    .SetSize(new IntVector2(90, 30))
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
                                titel
                                .SetText("CHOOSE YOUR MONSTER")
                                .SetBackgroundColor(ConsoleColor.White)
                                .SetForegroundColor(ConsoleColor.DarkMagenta)
                                .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_SINGLE_ROUND_EDGE)
                            ]

                            + new LLayoutManager.Slot()
                            .SetExpandPercentage(new Vector2(0.5f, 0.75f))
                            //.SetMargin(new IntVector2(1, 1))
                            [
                               new WPanel()
                               .SetTransparency(true)
                                .SetLayoutManager<LFlowLayout>(new LFlowLayout())
                                    .SetFlowAlginment(LFlowLayout.EFlowAlignment.EFLOW_ALIGNMENT_HORIZONTAL)

                                    + new LFlowLayout.Slot()
                                    .SetMargin(new IntVector2(1, 1))
                                    .SetExpandPercentage(new Vector2(0.5f, 1.0f))
                                    [
                                        statsPanel
                                        .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_SINGLE_HEAVY)
                                        .SetLayoutManager<LGridLayout>(new LGridLayout(2, 9))
                                        .AddSlot(new LLayoutManager.Slot()
                                            [
                                                new WText()
                                                .SetText("Name: ")
                                            ]
                                        ,0, 0)
                                        .AddSlot(new LLayoutManager.Slot()[txtName], 1, 0)
                                        .AddSlot(new LLayoutManager.Slot()
                                            [
                                                new WText()
                                                .SetText("Typ: ")
                                            ]
                                        , 0, 1)
                                        .AddSlot(new LLayoutManager.Slot()[txtTyp], 1, 1)
                                        .AddSlot(new LLayoutManager.Slot()
                                            [
                                                new WText()
                                                .SetText("Health: ")
                                            ]
                                        , 0, 2)
                                        .AddSlot(new LLayoutManager.Slot()[txtHealth], 1, 2)
                                        .AddSlot(new LLayoutManager.Slot()
                                            [
                                                new WText()
                                                .SetText("Mana: ")
                                            ]
                                        , 0, 3)
                                        .AddSlot(new LLayoutManager.Slot()[txtMana], 1, 3)
                                        .AddSlot(new LLayoutManager.Slot()
                                            [
                                                new WText()
                                                .SetText("Strength: ")
                                            ]
                                        , 0, 4)
                                        .AddSlot(new LLayoutManager.Slot()[txtAttack], 1, 4)
                                        .AddSlot(new LLayoutManager.Slot()
                                            [
                                                new WText()
                                                .SetText("Defense: ")
                                            ]
                                        , 0, 5)
                                        .AddSlot(new LLayoutManager.Slot()[txtDefense], 1, 5)
                                        .AddSlot(new LLayoutManager.Slot()
                                            [
                                                new WText()
                                                .SetText("Agility: ")
                                            ]
                                        , 0, 6)
                                        .AddSlot(new LLayoutManager.Slot()[txtAgility], 1, 6)
                                    ]
                                    + new LFlowLayout.Slot()
                                    .SetExpandPercentage(new Vector2(0.5f, 1.0f))
                                    [
                                         menu
                                         .AddOnAdditionalRegister(OnRegisterMenuEvents)
                                         .SetFlowSizeManagement(LFlowLayout.EFlowSizeManagement.EFLOW_SIZE_MANAGEMENT_ABSOLUTE)
                                         .SetElementSize(3)
                                    ]
                            ]

                            + new LLayoutManager.Slot()
                            .SetExpandPercentage(new Vector2(0.5f, 0.10f))
                            [
                                new WText()
                                .SetText("Accept -> " + acceptKeys.First() + "Edit -> " + editKeys.First() + "Delete -> " + deleteKeys.First())
                            ]
                    ]
                );

            MonsterLib.LoadLib();

            //Add monster slots
            for(int i = 0; i < MAX_MONSTER_COUNT; i++)
            {
                menu.AddWidget(
                            new WMonsterSlot()
                            .SetMonster(i < MonsterLib.MonsterList.Count ? MonsterLib.MonsterList[i] : null)
                            .AddOnClick(OnMonsterSlotClick)
                            .AddOnFocus(OnMonsterSlotFocus)
                        );
            }
        }

        //---------------------------------------------------------------PRIVATE METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        /// <summary>
        /// Hookup of custom menu events
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Controller.InputEventDelegate> OnRegisterMenuEvents()
        {
            Dictionary<string, Controller.InputEventDelegate> events = new Dictionary<string, Controller.InputEventDelegate>();

            events.Add("Delete", OnDeleteElement);
            events.Add("Edit", OnEditElement);

            return events;
        }

        /// <summary>
        /// Button event
        /// </summary>
        private void OnEditElement()
        {
            if(menu.GetSelectedWidget() is WMonsterSlot)
            {
                MonsterData m = ((WMonsterSlot)menu.GetSelectedWidget()).Monster;

                if(m != null)
                {
                    menu.ShowPopup("Edit monster?", "Yes", "No", OnEditMonster);
                }
            }
        }

        /// <summary>
        /// Button event
        /// </summary>
        private void OnDeleteElement()
        {
            if (menu.GetSelectedWidget() is WMonsterSlot)
            {
                MonsterData m = ((WMonsterSlot)menu.GetSelectedWidget()).Monster;

                if (m != null)
                {
                    menu.ShowPopup("Delete monster?", "Yes", "No", OnDeleteMonster);
                }
            }
        }

        /// <summary>
        /// Called if user wishes to delete a monster
        /// </summary>
        /// <param name="caller"></param>
        private void OnDeleteMonster(WButton caller)
        {
            MonsterData m = ((WMonsterSlot)menu.GetSelectedWidget()).Monster;

            MonsterLib.MonsterList.Remove(m);
            MonsterLib.SaveLib();

            ((WMonsterSlot)menu.GetSelectedWidget()).Monster = null;
        }

        /// <summary>
        /// Called if user selects a monster
        /// </summary>
        /// <param name="btn"></param>
        private void OnMonsterSlotClick(WButton btn)
        {
            if(btn is WMonsterSlot)
            {
                MonsterData m = ((WMonsterSlot)btn).Monster;

                if(m == null)
                {
                    menu.ShowPopup("Create new monster?", "Yes", "No", OnCreateNewMonster);
                }
                else
                {
                    if(player == null)
                    {
                        titel.SetText("CHOOSE YOUR ENEMY!");
                        player = m;
                    }
                    else
                    {
                        if(m.Type != player.Type)
                        {
                            opponent = m;
                            BattleManager battle = new BattleManager(player, opponent);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when selction index changes
        /// </summary>
        /// <param name="btn"></param>
        private void OnMonsterSlotFocus(WButton btn)
        {
            if(btn is WMonsterSlot)
            {
                WMonsterSlot slot = (WMonsterSlot)btn;

                if(slot.Monster == null)
                {
                    txtName.Text = "";
                    txtTyp.Text = "";
                    txtHealth.Text = "";
                    txtMana.Text = "";
                    txtAttack.Text = "";
                    txtDefense.Text = "";
                    txtAgility.Text = "";
                }
                else
                {
                    txtName.Text = slot.Monster.Name;
                    txtTyp.Text = slot.Monster.TypeToString();
                    txtHealth.Text = slot.Monster.MaxHealth.ToString();
                    txtMana.Text = slot.Monster.MaxMana.ToString();
                    txtAttack.Text = slot.Monster.Strength.ToString();
                    txtDefense.Text = slot.Monster.Defense.ToString();
                    txtAgility.Text = slot.Monster.Agility.ToString();
                }
            }
        }

        /// <summary>
        /// Called when user creates new monster
        /// </summary>
        /// <param name="btn"></param>
        private void OnCreateNewMonster(WButton btn)
        {
            Renderer.Instance.SetScene(new SCreateMonsterScreen());
        }

        /// <summary>
        /// Called when user edits monster
        /// </summary>
        /// <param name="btn"></param>
        private void OnEditMonster(WButton btn)
        {
            SCreateMonsterScreen screen = new SCreateMonsterScreen(((WMonsterSlot)menu.GetSelectedWidget()).Monster);

            Renderer.Instance.SetScene(screen);
        }
    }
}
