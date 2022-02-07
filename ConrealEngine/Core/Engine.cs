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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConrealEngine
{
    public class Engine : Submodule
    {
        private static readonly Engine instance = new Engine();
        private List<WWidget> tickableWidgets = new List<WWidget>();

        public static Engine Instance { get { return instance; } }

        public Game GameInstance { get; set; }

        public void AddTickableActor(WWidget a)
        {
            if (tickableWidgets.Contains(a))
                return;

            tickableWidgets.Add(a);
        }

        public void RemoveTickableActor(WWidget a)
        {
            tickableWidgets.Remove(a);
        }

        public override void Start()
        {
            if (active || GameInstance == null)
            {
                Console.WriteLine("Engine already started or no game instance specified!");

                return;
            }

            InputHandler.Instance.Start();
            Renderer.Instance.Start();
            ResourceHandler.Instance.Start();

            GameInstance.OnGameBegin();

            active = true;

            Game();
        }

        public override void Stop()
        {
            if (!active)
                return;

            GameInstance.OnGameEnd();

            InputHandler.Instance.Stop();
            Renderer.Instance.Stop();
            ResourceHandler.Instance.Stop();

            active = false;
        }

        //--------------------------Private Methods-----------------------------

        /// <summary>
        /// The game loop
        /// </summary>
        private void Game()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (IsActive)
            {
                sw.Reset();
                sw.Start();

                InputHandler.Instance.HandleInput();
                Renderer.Instance.Render();

                GameInstance.OnGameTick((float)sw.Elapsed.TotalSeconds);

                foreach(WWidget a in tickableWidgets)
                {
                    a.OnTick((float)sw.Elapsed.TotalSeconds);
                }

                Renderer.Instance.Scene.OnTick((float)sw.Elapsed.TotalSeconds);

                Thread.Sleep(1);
            }
        }
    }
}
