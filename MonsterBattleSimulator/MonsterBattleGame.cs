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

namespace MonsterBattleSimulator
{
    class MonsterBattleGame : Game
    {
        //---------------------------------------------------------------PRIVATE MEMBER VARIABLES----------------------------------------------------------------------
        private MonsterData _testMonster1 = new MonsterData();
        private MonsterData _testMonster2 = new MonsterData();

        //---------------------------------------------------------------PUBLIC METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        /// <summary>
        /// Registers game specific controls with the input manager
        /// </summary>
        /// <param name="uiControls"></param>
        /// <param name="gameplayControls"></param>
        public override void GetDefaultGameControls(out Dictionary<ConsoleKey, string> uiControls, out Dictionary<ConsoleKey, string> gameplayControls)
        {
            base.GetDefaultGameControls(out uiControls, out gameplayControls);

            uiControls.Add(ConsoleKey.X, "Delete");
            uiControls.Add(ConsoleKey.E, "Edit");
        }

        /// <summary>
        /// Called when the game begins
        /// </summary>
        public override void OnGameBegin()
        {
            Console.SetWindowSize((int)(Console.LargestWindowWidth / 1.5f), (int)(Console.LargestWindowHeight / 1.2f));

            Renderer.Instance.SetScene(new SMainMenu());
        }

        /// <summary>
        /// Called when the game ends
        /// </summary>
        public override void OnGameEnd()
        {
            base.OnGameEnd();
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        /// <param name="deltaSeconds"></param>
        public override void OnGameTick(float deltaSeconds)
        {
            base.OnGameTick(deltaSeconds);
        }
    }
}
