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
using ConUI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Used for turn based battlesystem
/// </summary>
namespace MonsterBattleSimulator
{
    public class BattleManager
    {
        //---------------------------------------------------------------TYPE DECLARATIONS----------------------------------------------------------------------
        public enum EBattleState
        {
            BATTLE_STATE_OPENING = 0, //Opening phase of a battle
            BATTLE_STATE_PLAYER = 1, //Players turn
            BATTLE_STATE_OPPONENT = 2 //Enemys turn
        }

        //---------------------------------------------------------------PRIVATE MEMBER VARIABLES----------------------------------------------------------------------
        private EBattleState _battleState = EBattleState.BATTLE_STATE_OPENING;
        private SMonsterBattleScreen _battleScreen;

        //---------------------------------------------------------------PUBLIC MEMBER VARIABLES----------------------------------------------------------------------
        public EBattleState BattleState { get { return _battleState; } }

        public WMonster Player { get; set; }
        public WMonster Opponent { get; set; }

        //---------------------------------------------------------------PUBLIC METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        public BattleManager(MonsterData player, MonsterData opponent)
        {
            //Init player and oponent objects
            player.Init();
            opponent.Init();

            //Asign monster data to corisponding widget
            Player = new WMonster(player);
            Opponent = new WMonster(opponent);

            //Create battle scene
            _battleScreen = new SMonsterBattleScreen(this, Player, Opponent);
            Renderer.Instance.SetScene(_battleScreen);

            //Register battle log callback delegate
            _battleScreen.DialogBox.AddOnDialogFinished(DialogCallback);
        }

        /// <summary>
        /// Starts a new battle
        /// </summary>
        public void StartBattle()
        {
            _battleScreen.DialogBox.SetDialog(new List<string>()
            {
                "A wild " + Opponent.Status.TypeToString() + " appears!",
                "GO " + Player.Status.Name + "!"
            });

            _battleScreen.ShowDialogBox();
        }

        /// <summary>
        /// End current turn and switch to next one
        /// </summary>
        /// <param name="a"></param>
        public void Next(MonsterBattleSimulator.Actions.Action a = null)
        {
            switch(_battleState)
            {
                case EBattleState.BATTLE_STATE_OPENING:
                    //Determine who should strike first
                    if(Player.Status.Agility >= Opponent.Status.Agility)
                    {
                        _battleState = EBattleState.BATTLE_STATE_PLAYER;

                        _battleScreen.DialogBox.SetDialog(new List<string>()
                        {
                            Player.Status.Name + "(Player) is faster than " + Opponent.Status.Name + "(Enemy)"
                        });
                    }
                    else
                    {
                        _battleState = EBattleState.BATTLE_STATE_OPPONENT;

                        _battleScreen.DialogBox.SetDialog(new List<string>()
                        {
                            Opponent.Status.Name + "(Enemy) is faster than " + Player.Status.Name + "(Player)"
                        });
                    }

                    break;
                case EBattleState.BATTLE_STATE_PLAYER:
                    //We need to display the player input menu
                    _battleScreen.ShowPlayerMenu();

                    if(a != null)
                    {
                        _battleScreen.ShowDialogBox();

                        Player.Attack(Opponent);

                        _battleScreen.DialogBox.SetDialog(new List<string>()
                        {
                            Player.Status.Name + "(Player) hits " + Opponent.Status.Name + "(Opponent)"
                        });

                        _battleState = EBattleState.BATTLE_STATE_OPPONENT;
                    }

                    break;
                case EBattleState.BATTLE_STATE_OPPONENT:
                    Opponent.Attack(Player);

                    _battleScreen.DialogBox.SetDialog(new List<string>()
                        {
                            Opponent.Status.Name + "(Enemy) hits " + Player.Status.Name + "(Player)"
                        });

                    _battleState = EBattleState.BATTLE_STATE_PLAYER;

                    break;
            }
        }

        //---------------------------------------------------------------PRIVATE METHODS AND CONSTRUCTORS----------------------------------------------------------------------

        /// <summary>
        /// The dialog callback function. Called when the user finishes current dialog log
        /// </summary>
        private void DialogCallback()
        {
            //Check if game is over
            if(Opponent.Status.CurrentHealth <= 0)
            {
                //Show victory scene
                Renderer.Instance.SetScene(new SEndScreen(true));
            }
            else if(Player.Status.CurrentHealth <= 0)
            {
                //Show game over scene
                Renderer.Instance.SetScene(new SEndScreen(false));
            }
            else
            {
                //Trigger next battle round
                Next();
            }
        }
    }
}
