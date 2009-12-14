using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Browser;

namespace System.Windows.Controls
{
  public class TextBoxExtended : TextBox
  {
    //Template internal content scroll viewer
    ScrollViewer  _content = null;
    //Content border
    Border        _content_border = null;
    //Text calculations
    TextBlock     _size_block = null;

    /// <summary>An Event beeing fired when template content found and initialized</summary>
    public event RoutedEventHandler ContentFound;

    public TextBoxExtended()
    {
      //Since we do not use any own style for this control set DefaultStyleKey to TextBox
      DefaultStyleKey = typeof(TextBoxExtended);
    }

    //In this part we have Get a Template Child 'ContentElement'.
    //This is ScrollViewer of TexBox
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      _content = base.GetTemplateChild("ContentElement") as ScrollViewer;
      if (_content == null)
        return;
      _content.LayoutUpdated += new EventHandler(OnContent_LayoutUpdated);
    }

    //Getting child content of scrollviewer
    private void OnContent_LayoutUpdated(object sender, EventArgs e)
    {
      if (_content_border != null)
        return;
      _content_border = VisualTreeHelper.GetChild(_content, 0) as Border;
      if (_content_border != null)
      {
        int count = VisualTreeHelper.GetChildrenCount(_content_border);
        if (count > 0)
        {
          Grid grid = VisualTreeHelper.GetChild(_content_border, 0) as Grid;
          if (grid != null)
          {
            //OK NOW TRY TO CREATE A LITTLE TextBlock for text mesurament calculations
            _size_block = new TextBlock()
            {
             Foreground = null,
             VerticalAlignment = VerticalAlignment.Top,
             HorizontalAlignment = HorizontalAlignment.Left,
             FontFamily  = FontFamily,
             FontSize    = FontSize,
             FontStretch = FontStretch,
             FontStyle   = FontStyle,
             FontWeight  = FontWeight
            };
            grid.Children.Add(_size_block);

            IEnumerable<System.Windows.Controls.Primitives.ScrollBar> found = (from child in grid.Children.ToList() where child is System.Windows.Controls.Primitives.ScrollBar select child as System.Windows.Controls.Primitives.ScrollBar);
            if (found.Count() > 0)
            {
              VerticalScrollBar = (from sc in found where sc.Name == "VerticalScrollBar" select sc).First();
              HorizontalScrollBar = (from sc in found where sc.Name == "HorizontalScrollBar" select sc).First();
              if (ContentFound != null)
                ContentFound(this, new RoutedEventArgs());
            }
            //_content.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
          }
        }
      }
    }

    public Size MesureText(string Text)
    {
      if (_size_block != null)
      {
        _size_block.Text = string.IsNullOrEmpty(Text.Replace("\r","").Replace("\n","")) ? " ":Text;
        return new Size(_size_block.ActualWidth, _size_block.ActualHeight);
      }
      return Size.Empty;
    }

    public bool CanDoTextMesure
    {
      get { return _size_block != null; }
    }

    public System.Windows.Controls.Primitives.ScrollBar VerticalScrollBar
    {
      get;
      set;
    }

    public System.Windows.Controls.Primitives.ScrollBar HorizontalScrollBar
    {
      get;
      set;
    }
  }
}
