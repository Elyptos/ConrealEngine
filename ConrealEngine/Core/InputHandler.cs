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
using System.IO;
using ConUI.Scenes;

namespace ConrealEngine
{
    public class InputHandler : Submodule
    {
        private static readonly InputHandler instance = new InputHandler();

        public readonly string PATH_TO_CONTROLS_CONFIG = "Config/Controls.txt";

        public PlayerController PlayerController { get; set; } = new PlayerController();
        public UIController UIController { get; set; } = new UIController();

        public Dictionary<ConsoleKey, string> UIControls { get; set; } = new Dictionary<ConsoleKey, string>();

        public Dictionary<ConsoleKey, string> GameplayControls { get; set; } = new Dictionary<ConsoleKey, string>();

        public static InputHandler Instance { get { return instance; } }

        static InputHandler() { }

        private InputHandler() { }

        public bool FocusUI { get; set; } = true;

        public void HandleInput()
        {
            if (!Console.KeyAvailable)
                return;

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if(FocusUI)
            {
                if(UIControls.ContainsKey(keyInfo.Key))
                {
                    UIController.OnInputRecieved(UIControls[keyInfo.Key], keyInfo);
                }
                else
                {
                    UIController.OnInputRecieved("noReg", keyInfo);
                }
            }
            else
            {
                if (GameplayControls.ContainsKey(keyInfo.Key))
                {
                    PlayerController.OnInputRecieved(GameplayControls[keyInfo.Key], keyInfo);
                }
            }
        }

        public override void Start()
        {
            if(!active)
            {
                active = true;
            }

            if(!LoadControlsFromDisk())
            {
                CreateDefaultControlsConfig();
            }
        }

        public override void Stop()
        {
            if(active)
            {
                active = false;
            }
        }

        public bool SaveControlsToDisk()
        {
            List<Dictionary<ConsoleKey, string>> controls = new List<Dictionary<ConsoleKey, string>>();

            controls.Add(UIControls);
            controls.Add(GameplayControls);

            return ResourceHandler.Instance.SaveToJSON(controls, PATH_TO_CONTROLS_CONFIG);
        }

        public bool LoadControlsFromDisk()
        {
            List<Dictionary<ConsoleKey, string>> controls = new List<Dictionary<ConsoleKey, string>>();

            if(ResourceHandler.Instance.LoadFromJSON< List<Dictionary<ConsoleKey, string>>>(out controls, PATH_TO_CONTROLS_CONFIG))
            {
                if(controls.Count != 2)
                {
                    return false;
                }

                UIControls = controls[0];
                GameplayControls = controls[1];

                return true;
            }
            else
            {
                return false;
            }
        }

        private void CreateDefaultControlsConfig()
        {
            Dictionary<ConsoleKey, string> uiCont = new Dictionary<ConsoleKey, string>();
            Dictionary<ConsoleKey, string> gameCont = new Dictionary<ConsoleKey, string>();

            Engine.Instance.GameInstance.GetDefaultGameControls(out uiCont, out gameCont);

            UIControls = uiCont;
            GameplayControls = gameCont;

            SaveControlsToDisk();
        }
    }
}
