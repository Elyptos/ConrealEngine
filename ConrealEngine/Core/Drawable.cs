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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConrealEngine.Renderer;

namespace ConrealEngine
{
    public abstract class Drawable
    {
        protected abstract void OnPreDraw();
        protected abstract void OnDraw(RenderHandle r, Box drawingArea);
        protected abstract void OnPostDraw();
        protected abstract bool ShouldTick();
        public abstract void EnableTick(bool b);
        public abstract void OnTick(float deltaSeconds);
        public abstract void OnConstruction();
    }
}
