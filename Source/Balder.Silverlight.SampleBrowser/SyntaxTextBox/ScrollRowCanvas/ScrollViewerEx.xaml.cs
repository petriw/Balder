using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Scroller
{

  // to help with clipping
  public class CanvasClipper : Panel
  {
    private RectangleGeometry _clippingRectangle;

    public CanvasClipper()
    {
      _clippingRectangle = new RectangleGeometry();
    }

    // 
    protected override Size ArrangeOverride(Size finalSize)
    {
      finalSize = base.ArrangeOverride(finalSize);
      ClippingRect = new Rect(0, 0, finalSize.Width, finalSize.Height);
      _clippingRectangle.Rect = ClippingRect;
      Clip = _clippingRectangle;
      return finalSize;
    }

    // so we know the final size of the clipping rect and thus the true display size
    public Rect ClippingRect
    {
      get;
      set;
    }
  }

  /// <summary>A container modeling a row of controls</summary>
  public class ScrollRowCanvas : Canvas
  {
    private int _row;

    //New prototypy for SyntaxTextBox
    public ScrollRowCanvas( int row )
    {
      _row = row;
      this.Width  = 1;
      this.Height = 1;
      MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
      {
        OnMouseLeftButtonDown(e);
      };
      MouseEnter += delegate(object sender, MouseEventArgs e)
      {
        OnMouseEnter(e);
      };
      MouseLeave += delegate(object sender, MouseEventArgs e)
      {
        OnMouseLeave(e);
      };
    }

    public void AddWord( Fireball.Syntax.Word word, Size wordSize )
    {
      TextBlock tb = null;
      if (Children.Count == 0)
      {
        tb = new TextBlock()
        {
          Width = wordSize.Width,
          Height = wordSize.Height
        };
        if (word != null)
        {
          Run run = new Run()
          {
            Text = word.Text,
            Foreground = (word.Style != null ? new SolidColorBrush(word.Style.ForeColor) : new SolidColorBrush(Colors.Transparent))
          };
          tb.Inlines.Add(run);
        }
        base.Children.Add(tb);
        tb.SetValue(Canvas.LeftProperty, 0.0d);
      }
      else
      {
        tb = base.Children[0] as TextBlock;
        Run run = new Run()
        {
          Text = word.Text,
          Foreground = (word.Style != null ? new SolidColorBrush(word.Style.ForeColor) : new SolidColorBrush(Colors.Transparent))
        };
        tb.Inlines.Add(run);
        tb.Width += wordSize.Width;
      }
      if( Width < tb.Width && tb != null )
        Width = tb.Width;
      if (Height < tb.Height && tb != null)
        Height = tb.Height;
    }

    public void Clear()
    {
      base.Children.Clear();
      Width   = 1;
      Height  = 1;
    }

    public bool IsMouseOver { get; private set; }

    /// <summary>
    /// Called when the user presses the left mouse button over the ListBoxItem. 
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (!e.Handled)
      {
        e.Handled = true;
        Debug.WriteLine("OnMouseLeftButtonDown: " + _row.ToString());
      }
    }

    /// <summary> 
    /// Called when the mouse pointer enters the bounds of this element.
    /// </summary>
    /// <param name="e">The event data.</param> 
    protected virtual void OnMouseEnter(MouseEventArgs e)
    {
      IsMouseOver = true;
      Debug.WriteLine("OnMouseEnter " + _row.ToString());
    }

    /// <summary>
    /// Called when the mouse pointer leaves the bounds of this element.
    /// </summary> 
    /// <param name="e">The event data.</param> 
    protected virtual void OnMouseLeave(MouseEventArgs e)
    {
      IsMouseOver = false;
      Debug.WriteLine("OnMouseLeave " + _row.ToString());
    }

    /// <summary>
    /// Called when the control got focus. 
    /// </summary> 
    /// <param name="e">The event data.</param>
    protected virtual void OnGotFocus(RoutedEventArgs e)
    {
      Debug.WriteLine("OnGotFocus " + _row.ToString());
    }

    /// <summary> 
    /// Called when the control lost focus.
    /// </summary>
    /// <param name="e">The event data.</param> 
    protected virtual void OnLostFocus(RoutedEventArgs e)
    {
      Debug.WriteLine("OnLostFocus " + _row.ToString());
    }
  }

  /// <summary>
  /// ScrollViewerEx - here we also inherit from the MouseWheelObserver interface so we
  /// can scroll using the wheel. See MouseWheel.cs for details
  /// </summary>
  public partial class ScrollViewerEx : UserControl//, IMouseWheelObserver
  {
    // set a fixed cell width
    private int cellWidth  = 1;
    // and a fixed cell height
    private int cellHeight = 1;
    
    //
    private int _rows = 0;
    //
    private int _cols = 0;

    public int CellHeight
    {
      get { return cellHeight; }
      set 
      {
        cellHeight = value;
        InvalidateLayout();
      }
    }
    
    public int CellWidth
    {
      get { return cellWidth; }
      set
      {
        cellWidth = value;
        InvalidateLayout();
      }
    }

    /// <summary>
    /// Stores the current scroll bar position as an integral index
    /// </summary>
    public int VertPosition
    {
      get { return (int)VScroll.Value; }
      set
      {
        VScroll.Value = value;
        InvalidateLayout();
      }
    }

    /// <summary>
    /// Get the maximum range of the vertical scrollbar
    /// </summary>
    public double VertRange
    {
      get { return (int)VScroll.Maximum; }
      set
      {
        VScroll.Maximum = value;
      }
    }
    
    /// <summary>
    /// Get the maximum range of the vertical scrollbar
    /// </summary>
    public double HorzRange
    {
      get { return HScroll.Maximum; } set { HScroll.Maximum = value; }
    }

    /// <summary>
    /// Stores the current horizontal scrollbar position as an integral index
    /// </summary>
    public int HorzPosition
    {
      get { return (int)HScroll.Value; }
      set
      {
        HScroll.Value = value;
        InvalidateLayout();
      }
    }

    private bool useClipper = true;

    /// <summary>
    /// Hows many rows can we display on a page? N.B. assumes fixed height
    /// </summary>
    private int RowsPerPage
    {
      get
      {
        if (useClipper)
          return (int)(ElementContentClipper.ClippingRect.Height / cellHeight);
        else
          return _rows;
      }
    }

    /// <summary>
    /// How many columns can we display on a page? N.B. assumes fixed width
    /// </summary>
    private int ColsPerPage
    {
      get
      {
        if (useClipper)
          return (int)(ElementContentClipper.ClippingRect.Width / cellWidth);
        else
          return _cols;
      }
    }

    /// <summary>
    /// List of all visible items
    /// </summary>
    public List<UIElement> VisibleItems
    {
      get;
      private set;
    }

    /// <summary>
    /// Lock for recursion in ArrangeOverride
    /// </summary>
    public bool Locked
    {
      get;
      set;
    }

    /// <summary>
    /// if FastMode == true then use fast scrolling ....
    /// </summary>
    public bool FastMode
    {
      get;
      private set;
    }

    private TranslateTransform Translation
    {
      get;
      set;
    }

    public int Rows
    {
      get { return _rows; }
      private set { }
    }

    public void AddWord(int row, Fireball.Syntax.Word word, Size wordSize)
    {
      ScrollRowCanvas sr = 
        row >= 0 && row <= ElementContent.Children.Count-1 
        ? ElementContent.Children[row] as ScrollRowCanvas: null;
      if (sr == null)
        AddRow();
      sr = ElementContent.Children[row] as ScrollRowCanvas;
      sr.AddWord(word, wordSize);
      if (CellHeight < sr.Height)
        CellHeight = (int)sr.Height;
    }

    public void RemoveRow(int Index, bool KeepLocked)
    {
      bool WasLocked = Locked;
      _rows--;
      double topDecrementer = (ElementContent.Children[Index] as FrameworkElement).Height;
      ElementContent.Children.RemoveAt(Index);
      IEnumerable<UIElement> rows = (from child in ElementContent.Children where ElementContent.Children.IndexOf(child) > Index select child);
      if (rows.Count() > 0)
      {
        rows.ToList().ForEach(row =>
          {
            double top = ((double)row.GetValue(Canvas.TopProperty)) - topDecrementer;
            row.SetValue(Canvas.TopProperty, top);
          });
      }
      if (KeepLocked == false)
        Locked = false;
      //
      SwitchStrategy(false, WasLocked);
      // force recalc etc
      InvalidateLayout();
      //
      this.Cursor = Cursors.Arrow;
    }

    public void AddRow()
    {
      AddRow(false);
    }

    public void InvalidateRows(bool KeepLocked)
    {
      bool WasLocked = Locked;
      double actualHeight = 0.0d;
      ElementContent.Children.Cast<ScrollRowCanvas>().ToList().ForEach(rw =>
      {
        rw.SetValue(Canvas.TopProperty, actualHeight);
        actualHeight += rw.ActualHeight;
      });
      Locked = false;
      SwitchStrategy(false, WasLocked);
      // force recalc etc
      InvalidateLayout();
      //
      this.Cursor = Cursors.Arrow;
    }

    public void AddRow(bool KeepLocked)
    {
      bool WasLocked = Locked;
      Locked = true;
      _rows++;
      ScrollRowCanvas sr = new ScrollRowCanvas(_rows-1);
      // add to the canvas
      double actualHeight = ElementContent.Children.Sum(s => (s as ScrollRowCanvas).ActualHeight);
      ElementContent.Children.Add(sr);
      sr.SetValue(Canvas.LeftProperty, 0.0d);
      // equivalent to <ScrollRowCanvas Canvas.Top="yoff">
      sr.SetValue(Canvas.TopProperty, actualHeight);
      if( KeepLocked == false )
        Locked = false;
      //
      actualHeight = 0.0d;
      ElementContent.Children.Cast<ScrollRowCanvas>().ToList().ForEach(rw =>
        {
          rw.SetValue(Canvas.TopProperty, actualHeight);
          actualHeight += rw.ActualHeight;
        });
      SwitchStrategy(false, WasLocked);
      // force recalc etc
      InvalidateLayout();
      //
      this.Cursor = Cursors.Arrow;
    }

    public void RemoveRow(int index)
    {
      if (index <= ElementContent.Children.Count - 1 && index >= 0)
        ElementContent.Children.RemoveAt(index);
      else
        return;
      Locked = true;
      _rows--;
      double xoff = 0;
      double yoff = 0;
      for (int row = 0; row < _rows; row++)
      {
        // new item
        ScrollRowCanvas sr = ElementContent.Children[row] as ScrollRowCanvas;
        // equivalent to <ScrollRowCanvas Canvas.Left="xoff">
        sr.SetValue(Canvas.LeftProperty, xoff);
        // equivalent to <ScrollRowCanvas Canvas.Top="yoff">
        sr.SetValue(Canvas.TopProperty, yoff);
        // next vertical slot
        yoff += cellHeight;
      }
      Locked = false;
      //
      SwitchStrategy(false);
      // force recalc etc
      InvalidateLayout();
      //
      this.Cursor = Cursors.Arrow;
    }

    public void ScrollIntoPosition(double hV, double vV)
    {
      bool update = false;
      if( HScroll != null && (update=HScroll.Value != hV))
        HScroll.Value = hV;
      if (VScroll != null && (update=VScroll.Value != vV))
        VScroll.Value = vV;
      if( update )
        InvalidateLayout();
    }

    public void Clear()
    {
      this.Cursor = Cursors.Wait;
      Locked = true;
      ElementContent.Children.Clear();
      //
      SwitchStrategy(false);
      // force recalc etc
      InvalidateLayout();
      //
      this.Cursor = Cursors.Arrow;
    }

    public ScrollRowCanvas this[int index]
    {
      get 
      {
        if (index >= 0 && index <= ElementContent.Children.Count - 1)
          return ElementContent.Children[index] as ScrollRowCanvas;
        else 
          return null;
      }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ScrollViewerEx()
    {
      InitializeComponent();
      Debug.Assert(ElementContent != null);
      this.Loaded += OnLoaded;
      // event handlers
      KeyDown += delegate(object sender, KeyEventArgs e)
      {
        OnKeyDown(e);
      };
      // list of *all* row items we manage
      VisibleItems = new List<UIElement>();
      // apply the scrolling translation
      Translation = new TranslateTransform();
      
      // mouse wheel listener - DISABLED
      //WheelMouseListener.Instance.AddObserver(this);
    }

    /// <summary>
    /// Ensure we get keyboard events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnLoaded(object sender, RoutedEventArgs e)
    {
      // fast by default
      FastMode = true;
      // set up scroll bars - actual ranges get calculated in
      // ArrangeOverride
      VScroll.Value   = 0;
      HScroll.Value   = 0;
      VScroll.Minimum = 0;
      VScroll.Maximum = _rows - 1;
      VScroll.Value   = 0;
      HScroll.Minimum = 0;
      HScroll.Maximum = _cols - 1;
      HScroll.Value   = 0;
      //
      SwitchStrategy(false);
    }

    public void SwitchStrategy(bool change)
    {
      SwitchStrategy(change, false);
    }
    /// <summary>Switch scrolling strategies</summary>
    /// <param name="change"></param>
    public void SwitchStrategy(bool change, bool WasLocked)
    {
      if (change)
      {
        FastMode = !FastMode;
      }

      int limit = ElementContent.Children.Count;
      Locked = true;
      if (FastMode)
      {
        Color color = Color.FromArgb(0xFF, 0x80, 0x00, 0x00);
        SolidColorBrush br = new SolidColorBrush(color);
        ColHeaderContent.Background = br;
        RowHeaderContent.Background = br;
        for (int row = 0; row < limit; row++)
        {
          ElementContent.Children[row].Visibility = Visibility.Collapsed;
        }
      }
      else
      {
        Color color = Color.FromArgb(0xFF, 0x40, 0x00, 0x00);
        SolidColorBrush br = new SolidColorBrush(color);
        ColHeaderContent.Background = br;
        RowHeaderContent.Background = br;
        for (int row = 0; row < limit; row++)
        {
          ElementContent.Children[row].Visibility = Visibility.Visible;
        }
      }
      if( WasLocked == false )
        Locked = false;
      InvalidateLayout();
    }

    //// mouse wheel - move vertical scroll bar as appropriate
    //public void OnMouseWheel(MouseWheelArgs args)
    //{
    //  // update the scrollbar thumb according to wheel motion
    //  double pos = VScroll.Value;
    //  pos += -args.Delta;
    //  VScroll.Value = pos;
    //  //
    //  InvalidateLayout();
    //  //_strategy.Layout(HorzPosition, VertPosition, RowsPerPage, ColsPerPage);
    //}

    /// <summary>
    /// N.B Simplified for the sake of example - we are only interested in thumb events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void VScroll_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
    {
      InvalidateLayout();
    }

    /// <summary>
    /// N.B Simplified for the sake of example - we are only interested in thumb events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HScroll_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
    {
      InvalidateLayout();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      return base.MeasureOverride(availableSize);
    }

    public void InvalidateLayout()
    {
      if (Locked)
        return;
      InvalidateArrange();
    }

    // establish how many rows and columns we can display and
    // set scroll bars accordingly
    protected override Size ArrangeOverride(Size finalSize)
    {
      // let the base class handle the arranging
      finalSize = base.ArrangeOverride(finalSize);
      // here's the magic ...
      ApplyLayoutOptimizer();
      //
      return finalSize;
    }

    /// <summary>
    /// Set the vertical and horizontal scroll bar ranges
    /// </summary>
    protected void SetScrollRanges()
    {
      // what is the view-port size?
      Rect clipRect = ElementContentClipper.ClippingRect;
      // how many integral lines can we display ?
      int rowsPerPage = (int)(clipRect.Height / cellHeight);
      // set the scroll count
      VScroll.Maximum = (_rows - rowsPerPage);
    }

    /// <summary>
    /// Use the Translation to scroll the content canvas
    /// </summary>
    protected void HandleScrolling()
    {
      // offset by scroll positions
      Translation.X = -(HScroll.Value);
      Translation.Y = -((VScroll.Value * cellHeight) - (VScroll.Value > 0 ? 5 : 0 ));
      // apply the transform to the content container
      ElementContent.RenderTransform = Translation;
    }

    public void ApplyLayoutOptimizer()
    {
      // beware recursion - settings visibility will trigger 
      // another ArrangeOverride invocation
      if (Locked == false)
      {
        // lock
        Locked = true;
        // set up the scroll bars
        SetScrollRanges();

        // hide the visible items
        foreach (UIElement uie in VisibleItems)
        {
          uie.Visibility = Visibility.Collapsed;
        }
        // remove from list
        VisibleItems.Clear();
        // layout a page worth of rows
        int maxRow = System.Math.Min(VertPosition + RowsPerPage, ElementContent.Children.Count);
        for (int row = VertPosition; row < maxRow; row++)
        {
          UIElement uie = ElementContent.Children[row];
          //
          uie.Visibility = Visibility.Visible;
          //
          VisibleItems.Add(uie);
        }
        // scroll the canvas
        HandleScrolling();
        // unlock
        Locked = false;
      }
    }
  }
}
