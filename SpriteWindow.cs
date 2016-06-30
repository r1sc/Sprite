﻿using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sprite
{
    public class SpriteWindow
    {
        #region Win32
        private struct PIXELFORMATDESCRIPTOR
        {
            public Int16 nSize;
            public Int16 nVersion;
            public Int32 dwFlags;
            public byte iPixelType;
            public byte cColorBits;
            public byte cRedBits;
            public byte cRedShift;
            public byte cGreenBits;
            public byte cGreenShift;
            public byte cBlueBits;
            public byte cBlueShift;
            public byte cAlphaBits;
            public byte cAlphaShift;
            public byte cAccumBits;
            public byte cAccumRedBits;
            public byte cAccumGreenBits;
            public byte cAccumBlueBits;
            public byte cAccumAlphaBits;
            public byte cDepthBits;
            public byte cStencilBits;
            public byte cAuxBuffers;
            public byte iLayerType;
            public byte bReserved;
            public Int32 dwLayerMask;
            public Int32 dwVisibleMask;
            public Int32 dwDamageMask;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr handle;
            public UInt32 msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public UInt64 time;
            public Point p;
        }
        private const int PFD_SUPPORT_OPENGL = 0x00000020;
        private const int PFD_DOUBLEBUFFER = 0x00000001;
        public const int GL_FRAGMENT_SHADER = 0x8B30;
        public const uint WM_PAINT = 0x000F;
        public const uint WM_CREATE = 0x0001;
        public const uint WM_QUIT = 0x0012;
        public const uint WM_CLOSE = 0x0010;
        public const uint WM_DESTROY = 0x002;
        [DllImport("gdi32.dll")]
        private static extern int ChoosePixelFormat(IntPtr hdc, [In] ref PIXELFORMATDESCRIPTOR ppfd);
        [DllImport("gdi32.dll")]
        private static extern bool SetPixelFormat(IntPtr hdc, int iPixelFormat, ref PIXELFORMATDESCRIPTOR ppfd);
        [DllImport("gdi32.dll")]
        private static extern bool SwapBuffers(IntPtr hdc);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
        [DllImport("user32.dll")]
        static extern IntPtr DispatchMessage([In] ref NativeMessage lpmsg);
        #endregion

        #region OpenGL
        [DllImport("opengl32.dll")]
        private static extern IntPtr wglCreateContext(IntPtr hdc);
        [DllImport("opengl32.dll")]
        private static extern bool wglMakeCurrent(IntPtr hdc, IntPtr hglrc);
        #endregion

        private IntPtr _hdc;
        private IntPtr _glContext;
        private TimeSpan _start;
        private uint _quadList;
        public InputForm Form { get; }
        public event EventHandler OnLoadResources;
        private Size VirtualSize { get; set; }
        private string _originalWindowTitle;
        public bool IsPaused => Form.IsPaused;
        public SpriteWindow(string windowTitle) : this(windowTitle, new Size(320, 200)) { }
        public SpriteWindow(string windowTitle, Size virtualSize)
        {
            Running = true;
            _originalWindowTitle = windowTitle;
            VirtualSize = new Size(virtualSize.Width, virtualSize.Height);
            Form = new InputForm { Text = _originalWindowTitle, ClientSize = new Size(640, 400) };
            Form.Shown += Form_Shown;
            Form.Resize += _form_Resize;
            Form.Closed += _form_Closed;
            Form.Show();
            _start = DateTime.Now.TimeOfDay;
        }
        private void Form_Shown(object sender, EventArgs e)
        {
            MakeContext();
        }
        private void MakeContext()
        {
            var g = Graphics.FromHwnd(Form.Handle);
            _hdc = g.GetHdc();
            var pfd = new PIXELFORMATDESCRIPTOR {dwFlags = PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER};
            var pf = ChoosePixelFormat(_hdc, ref pfd);
            SetPixelFormat(_hdc, pf, ref pfd);
            wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            _glContext = wglCreateContext(_hdc);
            wglMakeCurrent(_hdc, _glContext);
            Gl.glViewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height);
            Gl.glMatrixMode((uint) Gl.MatrixMode.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, VirtualSize.Width, VirtualSize.Height, 0, 0, 1);
            Gl.glMatrixMode((uint) Gl.MatrixMode.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glEnable((uint) Gl.GetTarget.GL_TEXTURE_2D);
            Gl.glBlendFunc((uint)Gl.BlendingFactorDest.GL_SRC_ALPHA, (uint)Gl.BlendingFactorDest.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable((uint)Gl.GetTarget.GL_BLEND);
            Gl.glEnable((uint)Gl.GetTarget.GL_ALPHA_TEST);
            Gl.glAlphaFunc((uint)Gl.AlphaFunctions.GL_GREATER, 0);
            _quadList = Gl.glGenLists(1);
            Gl.glNewList(_quadList, (uint) Gl.ListMode.GL_COMPILE);
            Gl.glBegin(7);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex2f(0, 0);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex2f(1, 0);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex2f(1, 1);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex2f(0, 1);
            Gl.glEnd();
            Gl.glEndList();
            if (OnLoadResources != null)
                OnLoadResources(this, null);
        }
        private void _form_Closed(object sender, EventArgs e)
        {
            Running = false;
        }
        private void _form_Resize(object sender, EventArgs e) => Gl.glViewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height);
        public bool IsKeyDown(VirtualKeys key) => Form.Keys[(byte)key] != null && (bool)Form.Keys[(byte)key];
        public void DrawGlBitmap(SpriteBitmap bitmap, int x, int y, int frame = 0)
        {
            if (_glContext == IntPtr.Zero)
                return;
            var frameOffset = new PointF((frame % bitmap.Cols) * bitmap.FrameSize.Width, (int)(frame / bitmap.Cols) * bitmap.FrameSize.Height);
            Gl.glBindTexture((uint) Gl.GetTarget.GL_TEXTURE_2D, bitmap.GLTexture);
            Gl.glMatrixMode((uint)Gl.MatrixMode.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glTranslatef(x, y, 0);
            Gl.glScalef(bitmap.FramePixelSize.Width, bitmap.FramePixelSize.Height, 1);
            Gl.glMatrixMode((uint)Gl.MatrixMode.GL_TEXTURE);
            Gl.glLoadIdentity();
            Gl.glTranslatef(frameOffset.X, frameOffset.Y, 0);
            Gl.glScalef(bitmap.FrameSize.Width, bitmap.FrameSize.Height, 1);
            Gl.glCallList(_quadList);
        }
        public void Swap()
        {
            SwapBuffers(_hdc);
            Application.DoEvents();
            Gl.glClear((uint) Gl.AttribMask.GL_COLOR_BUFFER_BIT);
        }
        public bool Running { get; set; }
    }
    public class InputForm : Form
    {
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        public Hashtable Keys = new Hashtable();
        private bool IsFullscreen { get; set; }
        public bool IsPaused { get; private set; }
        public InputForm()
        {
            KeyDown += WindowsFormsKeyEvent;
            Activated += ActivateWindow;
            Deactivate += DeactivateWindow;
            IsPaused = false;
        }
        private void WindowsFormsKeyEvent(object sender, KeyEventArgs e)
        {
            if ((e.Alt && e.KeyCode == System.Windows.Forms.Keys.Enter) || e.KeyCode == System.Windows.Forms.Keys.F11)
                ToggleFullscreen();
        }
        private void ActivateWindow(object sender, EventArgs e) => IsPaused = false;
        private void DeactivateWindow(object sender, EventArgs e) => IsPaused = true;
        private void ToggleFullscreen()
        {
            
        }
        protected override void WndProc(ref Message m)
        {
            var key = (uint)m.WParam;
            if (m.Msg == WM_KEYDOWN)
            {
                Keys[(byte)key] = true;
            }
            else if (m.Msg == WM_KEYUP)
            {
                Keys[(byte)key] = false;
            }
            base.WndProc(ref m);
        }
    }
}
