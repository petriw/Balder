using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Xml.Linq;
using Fireball.CodeEditor.SyntaxFiles;

namespace System.Windows.Controls
{
	public partial class SyntaxTextBox
	{
		Fireball.Syntax.SyntaxDocument _document;
		bool _updated_locked = false;
		bool _is_loaded = false;

		public static readonly DependencyProperty SyntaxLanguageProperty = DependencyProperty.Register("SyntaxLanguage", typeof(SyntaxLanguage), typeof(SyntaxTextBox),
		  new PropertyMetadata(SyntaxLanguage.CSharp, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e)
		  {
			  SyntaxTextBox box = d as SyntaxTextBox;
			  if( null != e.NewValue && null != box )
			  {
			  	box.SyntaxLanguage = (SyntaxLanguage) e.NewValue;
			  }
		  }));


		public SyntaxLanguage SyntaxLanguage
		{
			get { return (SyntaxLanguage)GetValue(SyntaxLanguageProperty); }
			set { SetValue(SyntaxLanguageProperty, value); }
		}


		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(SyntaxTextBox),
		  new PropertyMetadata(false, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e)
		  {
			  SyntaxTextBox box = d as SyntaxTextBox;
			  if (box != null && box._text_box != null)
			  {
				  box._text_box.IsReadOnly = (bool)e.NewValue;
			  }
		  }));

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SyntaxTextBox),
		  new PropertyMetadata("", delegate(DependencyObject d, DependencyPropertyChangedEventArgs e)
			{
				if (e.NewValue != null)
				{
					SyntaxTextBox box = d as SyntaxTextBox;
					if (box != null)
					{
						string new_val = (string)e.NewValue;
						string old_val = box._document.Text;
						string[] new_lines = null;
						string[] old_lines = null;
						System.IO.StringReader r = new System.IO.StringReader(new_val);
						string line = null;
						List<string> l_lines = new List<string>();

						while ((line = r.ReadLine()) != null)
						{
							l_lines.Add(line);
						}
						new_lines = l_lines.ToArray();
						r = new System.IO.StringReader(old_val);
						line = null;
						l_lines = new List<string>();
						while ((line = r.ReadLine()) != null)
						{
							l_lines.Add(line);
						}
						old_lines = l_lines.ToArray();

						bool has_changes = false;
						for (int i = 0; i < new_lines.Count(); i++)
						{
							if (i <= box._document.Count - 1)
							{
								if (box._document[i].Text != new_lines[i])
								{
									box._document[i].SetText(new_lines[i]);
									box._document[i].IsRendered = false;
									has_changes = true;
								}
							}
							else
							{
								Fireball.Syntax.Row row = box._document.Add(new_lines[i], false);
								box._document.ParseRow(row, true);
								has_changes = true;
								row.IsRendered = false;
							}
						}
						if (old_lines.Count() > new_lines.Count())
						{
							for (int i = new_lines.Count(); ; )
							{
								if (box._document.Count == new_lines.Count())
									break;
								if (box._document.Count == 1)
								{
									box._document[i].SetText("");
									has_changes = true;
									break;
								}
								box._document.Remove(i);
							}
						}
						if (has_changes)
						{
							box.RenderDocument();
						}
					}
				}
			}));

		public SyntaxTextBox()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(OnLoaded);
		}

		//External Language Syntax Loading...
		public void SetSyntax(string SyntaxSrc, System.Text.Encoding SrcEncoding, Fireball.CodeEditor.SyntaxFiles.SyntaxLanguage language)
		{
			if (_is_loaded != true)
				return;
			System.IO.MemoryStream s = new System.IO.MemoryStream(SrcEncoding.GetBytes(SyntaxSrc));
			SetSyntax(s, language);
		}

		//External Language Syntax Loading...
		public void SetSyntax(System.IO.Stream SyntaxSrc, Fireball.CodeEditor.SyntaxFiles.SyntaxLanguage language)
		{
			if (_is_loaded != true)
				return;
			_document.Parser.Init(Fireball.Syntax.Language.FromSyntaxFile(SyntaxSrc));
		}

		protected void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.Focus();
			_document = new Fireball.Syntax.SyntaxDocument();
			CodeEditorSyntaxLoader.SetSyntax(_document, SyntaxLanguage);
			//*****************TEST ONLY***********************//
			XElement elm = new XElement("Objects",
			  new XAttribute("type", "None"),
			  new XElement("Hello")
			  );
			//************************************************//
			_text_box.ContentFound += OnContentFound;
			_text_box.TextChanged += OnTextChanged;
			_text_box.KeyUp += OnTextBox_KeyUp;
			//_text_box.Text = "public class MyClass : ISomething\n{\n	public void Something(float value)\n	{		Console.WriteLine(\"Hello world\");\n	}\n}"; // elm.ToString();
			_text_box.LayoutUpdated += new EventHandler(OnTextLayoutUpdated);
			_text_box.IsReadOnly = IsReadOnly;
		}

		protected void OnTextLayoutUpdated(object sender, EventArgs e)
		{
			if (_updated_locked)
			{
				_updated_locked = !_updated_locked;
				return;
			}
			UpdateScrolls();
			_updated_locked = true;
		}

		protected void OnContentFound(object sender, RoutedEventArgs e)
		{
			RenderDocument();
		}

		public void UpdateScrolls()
		{
			if (
				  _text_box.VerticalScrollBar != null &&
				  _text_box.HorizontalScrollBar != null
			   )
			{
				double pVt = 0;
				double pHt = 0;
				double pVs = 0;
				double pHs = 0;
				if (_text_box.VerticalScrollBar.Maximum > 0 && _text_box.VerticalScrollBar.Value > 0)
					pVt = (_text_box.VerticalScrollBar.Value / _text_box.VerticalScrollBar.Maximum) * 100;
				pVs = (_scroll.VertRange / 100) * pVt;
				if (_text_box.HorizontalScrollBar.Maximum > 0 && _text_box.HorizontalScrollBar.Value > 0)
					pHt = (_text_box.HorizontalScrollBar.Value / _text_box.HorizontalScrollBar.Maximum) * 100;
				pHs = (_scroll.HorzRange / 100) * pHt;
				_scroll.HorzRange = _text_box.HorizontalScrollBar.Maximum;
				_scroll.ScrollIntoPosition(_text_box.HorizontalScrollBar.Value/*Math.Round(pHs)*/, Math.Round(pVs));
			}
		}

		protected void OnTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			UpdateScrolls();
		}

		protected void OnTextChanged(object sender, RoutedEventArgs e)
		{
			Text = _text_box.Text;
			_text_box.Focus();
		}

		protected void RenderDocument()
		{
			List<Fireball.Syntax.Row> rows = _document.Rows.OfType<Fireball.Syntax.Row>().ToList();//.VisibleRows.OfType<Fireball.Syntax.Row>().ToList();
			List<Fireball.Syntax.Row> total_rows = _document.Rows.OfType<Fireball.Syntax.Row>().ToList();//.VisibleRows.OfType<Fireball.Syntax.Row>().ToList();
			rows.ForEach(row =>
			  {
				  if (_document[rows.IndexOf(row)].RowState == Fireball.Syntax.RowState.SegmentParsed)
				  {
					  row.IsRendered = false;
					  _document.Parser.ParseLine(rows.IndexOf(row), true);
				  }
				  if (_document[rows.IndexOf(row)].RowState == Fireball.Syntax.RowState.NotParsed)
				  {
					  row.IsRendered = false;
					  _document.ParseRow(row, true);
				  }
			  });
			if (_text_box.CanDoTextMesure == false)
				return;
			_scroll.Locked = true;
			bool ValidateRows = false;
			rows.ForEach(row =>
			  {
				  if (row.IsRendered)
					  return;
				  if (row.Index > _scroll.Rows - 1)
				  {
					  ValidateRows = true;
					  _scroll.AddRow(true);
				  }

				  Fireball.Syntax.WordCollection words = row.FormattedWords;
				  row.IsRendered = true;
				  Scroller.ScrollRowCanvas block = _scroll[row.Index] as Scroller.ScrollRowCanvas;
				  block.Clear();
				  if (words.Count > 0)
				  {
					  words.OfType<Fireball.Syntax.Word>().ToList().ForEach(word =>
						{
							if (_text_box.CanDoTextMesure)
								_scroll.AddWord(row.Index, word, _text_box.MesureText(word.Text));
						});
				  }
				  else
				  {
					  if (_text_box.CanDoTextMesure)
						  _scroll.AddWord(row.Index, null, _text_box.MesureText(""));
				  }
			  });
			if (total_rows.Count < _scroll.Rows)
			{
				while (_scroll.Rows > total_rows.Count)
				{
					_scroll.RemoveRow(_scroll.Rows - 1, true);
					ValidateRows = true;
				}
			}
			if (ValidateRows)
			{
				_scroll.InvalidateRows(true);
			}
			_scroll.Locked = false;
			_scroll.InvalidateLayout();
			_text_box.Focus();
			UpdateScrolls();
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


		public string Text
		{
			get { return (string)base.GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public bool IsReadOnly
		{
			get { return (bool)base.GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
	}
}

