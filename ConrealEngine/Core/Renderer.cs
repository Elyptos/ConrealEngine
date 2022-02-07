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
using ConUI.Widgets;
using ConUI.Scenes;
using System.IO;
using System.Numerics;
using ConUI.Widgets.Layout;
using ConUI.Helper;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ConrealEngine
{
    public class Renderer : Submodule
    {
        public class RenderHandle
        {
            private int width = 0;
            private int height = 0;

            public CharInfo[] Buffer;
            public int BufferWidth { get { return width; } }
            public int BufferHeight { get { return height; } }

            public void ResizeBuffer(int width, int height)
            {
                if (width < 0 || height < 0)
                    return;

                this.width = width;
                this.height = height;

                Buffer = new CharInfo[width * height];
                //DrawMany((char)0x055e, new IntVector2(0, 0), new IntVector2(width - 1, height - 1));
            }

            public RenderHandle(int bufferWidth, int bufferHeight)
            {
                ResizeBuffer(bufferWidth, bufferHeight);
            }

            public void Draw(char c, IntVector2 position, ConsoleColor foreground, ConsoleColor background)
            {
                if (c == '\0' || position.X < 0 || position.X >= width || position.Y < 0 || position.Y >= height)
                {
                    return;
                }

                CharInfo cInfo = new CharInfo();
                cInfo.Char.UnicodeChar = c;
                cInfo.Attributes = (short)((int)foreground + (int)background * 16);

                Buffer[MathHelper.Get1DIndexFrom2D(position, width, height)] = cInfo;
            }

            public void DrawMany(char c, IntVector2 startPos, IntVector2 endPos, ConsoleColor foreground, ConsoleColor background)
            {
                for (int y = (int)startPos.Y; y <= endPos.Y; y++)
                {
                    for (int x = (int)startPos.X; x <= endPos.X; x++)
                    {
                        Draw(c, new IntVector2(x, y), foreground, background);
                    }
                }
            }

            public void DrawMany(char[,] c, IntVector2 startPos, ConsoleColor foreground, ConsoleColor background)
            {
                for (int y = 0; y < c.GetLength(1); y++)
                {
                    for (int x = 0; x < c.GetLength(0); x++)
                    {
                        Draw(c[x, y], new IntVector2(startPos.X + x, startPos.Y + y), foreground, background);
                    }
                }
            }
        }

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutputW(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct CharUnion
        {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        private bool forceResize = false;

        private SafeFileHandle hConsole;
        private static readonly Renderer instance = new Renderer();

        private int bufferWidth = 0;
        private int bufferHeight = 0;

        private SScene scene;

        private Renderer() { }
        static Renderer() { }

        public SScene Scene { get { return scene; } }

        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.White;

        public static Renderer Instance
        {
            get { return instance; }
        }

        public RenderHandle Handle { get; set; } = new Renderer.RenderHandle(0, 0);

        public override void Start()
        {
            active = true;

            hConsole = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.CursorVisible = false;

            if (scene == null)
            {
                ResetScene();
            }
        }

        public override void Stop()
        {
            active = false;

            //Cleanup
        }

        public void SetScene(SScene s, bool redrawScreen = true)
        {
            if (s != null)
            {
                if (scene != null)
                    scene.ClearScene();

                scene = s;
                s.ConstructScene();

                if (redrawScreen)
                {
                    Handle.ResizeBuffer(bufferWidth, bufferHeight);
                    scene.LayoutChanged(bufferWidth, bufferHeight);
                }
            }
        }

        //Render loop
        public void Render()
        {
            try
            {
                if (!active)
                    Start();

                if (forceResize || Console.BufferWidth != bufferWidth || Console.WindowHeight != bufferHeight)
                {
                    Console.BackgroundColor = BackgroundColor;
                    Console.Clear();

                    forceResize = false;

                    bufferWidth = Console.BufferWidth;
                    bufferHeight = Console.WindowHeight;

                    Console.BufferHeight = bufferHeight;

                    Handle.ResizeBuffer(bufferWidth, bufferHeight);
                    scene.LayoutChanged(bufferWidth, bufferHeight);
                }

                scene.RenderScene(Handle);

                //Console.Clear();
                DrawBuffer(Handle);
            }
            catch(Exception e)
            {
                forceResize = true;
            }

            Thread.Sleep(1);
        }

        private void DrawBuffer(RenderHandle h)
        {
            if(!hConsole.IsInvalid)
            {
                SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = (short)h.BufferWidth, Bottom = (short)h.BufferHeight };

                WriteConsoleOutputW(hConsole, h.Buffer,
                          new Coord() { X = rect.Right, Y = rect.Bottom },
                          new Coord() { X = 0, Y = 0 },
                          ref rect);

                Console.SetCursorPosition(0, 0);
            }
        }

        private void ResetScene()
        {
            SScene s = new SScene();

            s.SetRoot(
                new LAlignmentLayout()
                    + new LLayoutManager.Slot()
                    .SetHorizontalAlignment(LLayoutManager.EHorizontalAlignment.EHORIZONTAL_ALIGNMENT_CENTER)
                    .SetVerticalAlignment(LLayoutManager.EVerticalAlignment.EVERTICAL_ALIGNMENT_CENTER)
                    .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_RELATIVE)
                    .SetExpandPercentage(new Vector2(0.5f, 0.5f))
                    .SetPivot(LLayoutManager.EPivot.EPIVOT_CENTER)
                    .SetMargin(new IntVector2(1, 1))
                    [
                        new WText()
                        .SetText("This scene is empty and lonely. :-(")
                        .DrawBorder(WWidget.EBorderStyle.EBORDER_STYLE_DOUBLE)
                    ]
                );


            SetScene(s);
        }
    }
}
