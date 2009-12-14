/*
 
    Helper class for scrolling windows using the mouse wheel.
    Found somewhere on a SilverLight blog - if anyone claims responsibility
    please let me know.
 
*/

using System;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;

namespace Scroller
{
    public interface IMouseWheelObserver
    {
        void OnMouseWheel(MouseWheelArgs args);
        event MouseEventHandler MouseEnter;
        event MouseEventHandler MouseLeave;
    }

    public class MouseWheelArgs : EventArgs
    {
        private readonly double _Delta;
        private readonly bool _ShiftKey;
        private readonly bool _CtrlKey;
        private readonly bool _AltKey;
        public double Delta
        {
            get { return this._Delta; }
        }

        public bool ShiftKey
        {
            get { return this._ShiftKey; }
        }

        public bool CtrlKey
        {
            get { return this._CtrlKey; }
        }

        public bool AltKey
        {
            get { return this._AltKey; }
        }

        public MouseWheelArgs(double delta, bool shiftKey, bool ctrlKey, bool altKey)
        {
            this._Delta = delta;
            this._ShiftKey = shiftKey;
            this._CtrlKey = ctrlKey;
            this._AltKey = altKey;
        }
    }

    public class WheelMouseListener
    {
        private Stack<IMouseWheelObserver> _ElementStack;
        private WheelMouseListener()
        {
            this._ElementStack = new Stack<IMouseWheelObserver>();
            HtmlPage.Window.AttachEvent("DOMMouseScroll", OnMouseWheel);
            HtmlPage.Window.AttachEvent("onmousewheel", OnMouseWheel);
            HtmlPage.Document.AttachEvent("onmousewheel", OnMouseWheel);
            Application.Current.Exit += new EventHandler(OnApplicationExit);
        }

        /// <summary>
        /// Detaches from the browser-generated scroll events.
        /// </summary>
        private void Dispose()
        {
            HtmlPage.Window.DetachEvent("DOMMouseScroll", OnMouseWheel);
            HtmlPage.Window.DetachEvent("onmousewheel", OnMouseWheel);
            HtmlPage.Document.DetachEvent("onmousewheel", OnMouseWheel);
        }

        public void AddObserver(IMouseWheelObserver element)
        {
            element.MouseEnter += new MouseEventHandler(OnElementMouseEnter);
            element.MouseLeave += new MouseEventHandler(OnElementMouseLeave);
        }

        private void OnMouseWheel(object sender, HtmlEventArgs args)
        {
            double delta = 0;
            ScriptObject e = args.EventObject;
            if (e.GetProperty("detail") != null)
            {
                delta = ((double)e.GetProperty("detail"));
            }
            else if (e.GetProperty("wheelDelta") != null)
            {
                delta = ((double)e.GetProperty("wheelDelta"));
            }
            delta = Math.Sign(delta);
            if (this._ElementStack.Count > 0)
            {
                this._ElementStack.Peek().OnMouseWheel(new MouseWheelArgs(delta, args.ShiftKey, args.CtrlKey, args.AltKey));
            }
        }

        private void OnElementMouseLeave(object sender, MouseEventArgs e)
        {
            if (this._ElementStack.Count > 0)
            {
                this._ElementStack.Pop();
            }
        }

        private void OnElementMouseEnter(object sender, MouseEventArgs e)
        {
            this._ElementStack.Push((IMouseWheelObserver)sender);
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private static WheelMouseListener _Instance = null;
        public static WheelMouseListener Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new WheelMouseListener();
                }
                return _Instance;
            }
        }
    }
}
