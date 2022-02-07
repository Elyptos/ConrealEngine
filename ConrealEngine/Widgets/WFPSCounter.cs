﻿/*
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

namespace ConUI.Widgets
{
    public class WFPSCounter : WText
    {
        private int fps = 0;

        private float time = 0.0f;

        public override void OnTick(float deltaSeconds)
        {
            base.OnTick(deltaSeconds);

            time += deltaSeconds;

            if (time > 1)
            {
                this.Text = fps.ToString();

                time = 0;
                fps = 0;
            }
            else
            {
                fps++;
            }
        }

        protected override bool ShouldTick()
        {
            return true;
        }
    }
}
