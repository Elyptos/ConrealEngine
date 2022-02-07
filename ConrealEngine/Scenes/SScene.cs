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
using ConUI.Widgets;
using ConUI.Widgets.Layout;
using static ConrealEngine.Renderer;
using System.Numerics;
using ConUI.Helper;

namespace ConUI.Scenes
{
    public class SScene
    {
        private LLayoutManager root;

        public bool RedrawScene { get; set; }

        public void ClearScene()
        {
            RemoveWidget();
            OnSceneCleanup();
        }

        public void RemoveWidget()
        {
            root = null;
        }

        public void SetRoot(LLayoutManager w)
        {
            if(w != null)
                root = w;
        }

        public void ConstructScene()
        {
            OnSceneConstruction();

            root.RedirectConstruction();

            RedrawScene = true;
        }

        public void LayoutChanged(int bufferWidth, int bufferHeight)
        {
            if(root != null)
            {
                root.OnLayoutChanged(new IntVector2(0, 0), new IntVector2(bufferWidth - 1, bufferHeight - 1));
            }
        }

        public void RenderScene(RenderHandle r)
        {
            Box drArea = new Box();

            drArea.CornerTL = new IntVector2(0, 0);
            drArea.CornerTR = new IntVector2(Renderer.Instance.Handle.BufferWidth - 1, 0);
            drArea.CornerBR = new IntVector2(Renderer.Instance.Handle.BufferWidth - 1, Renderer.Instance.Handle.BufferHeight - 1);
            drArea.CornerBL = new IntVector2(0, Renderer.Instance.Handle.BufferHeight - 1);

            if (root != null)
                root.RedirectDrawcall(r, drArea);
        }

        public virtual void OnTick(float deltaSeconds)
        {

        }

        protected virtual void OnSceneConstruction()
        {

        }

        protected virtual void OnSceneCleanup()
        {
            if(this.root != null)
            {
                this.root.OnDestroy();
            }
        }
    }
}
