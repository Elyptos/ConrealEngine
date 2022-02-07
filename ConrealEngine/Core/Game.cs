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
using System.Threading;
using ConrealEngine;

namespace ConrealEngine
{
    public class Game
    {
        public virtual void OnGameBegin()
        {

        }

        public virtual void OnGameTick(float deltaSeconds)
        {

        }

        public virtual void OnGameEnd()
        {

        }

        public virtual void GetDefaultGameControls(out Dictionary<ConsoleKey, string> uiControls, out Dictionary<ConsoleKey, string> gameplayControls)
        {
            uiControls = new Dictionary<ConsoleKey, string>()
            {
                {ConsoleKey.UpArrow, "Up" },
                {ConsoleKey.DownArrow, "Down" },
                {ConsoleKey.RightArrow, "Right" },
                {ConsoleKey.LeftArrow, "Left" },
                {ConsoleKey.Enter, "Accept" }
            };

            gameplayControls = new Dictionary<ConsoleKey, string>()
            {
                {ConsoleKey.W, "Up" },
                {ConsoleKey.S, "Down" },
                {ConsoleKey.D, "Right" },
                {ConsoleKey.A, "Left" },
            };
        }
    }
}
