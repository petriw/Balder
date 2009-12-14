
//	Copyright (C) 2005  Sebastian Faltoni <sebastian@dotnetfireball.org>
//	
//	Copyright (C) compona.com 
//	
//	This library is free software; you can redistribute it and/or
//	modify it under the terms of the GNU Lesser General Public
//	License as published by the Free Software Foundation; either
//	version 2.1 of the License, or (at your option) any later version.
//	
//	This library is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//	Lesser General Public License for more details.
//	
//	You should have received a copy of the GNU Lesser General Public
//	License along with this library; if not, write to the Free Software
//	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Fireball.Syntax;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace Fireball.Syntax
{
	/// <summary>
	/// 
	/// </summary>
	public class SyntaxLoader
	{
		private Dictionary<string, TextStyle> _Styles = new Dictionary<string, TextStyle>();
		private Dictionary<string, BlockType> _Blocks = new Dictionary<string, BlockType>();

		private Language _Language = new Language();
		private static bool _UseUserCustomStyles = false;

		//protected BlockType		mLanguage.MainBlock=null;

		private static string _UserCustomStyles = null;


		/// <summary>
		/// Directory where is saved language font and color configurations
		/// </summary>
		public static string UserCustomStyles
		{
			get { return SyntaxLoader._UserCustomStyles; }
			set { SyntaxLoader._UserCustomStyles = value; }
		}

		/// <summary>
		/// Set if user can user custom user styles for the Language
		/// </summary>
		public static bool UseUserCustomStyles
		{
			get { return _UseUserCustomStyles; }
			set { _UseUserCustomStyles = value; }
		}

		/// <summary>
		/// Load a specific language file
		/// </summary>
		/// <param name="File">File name</param>
		/// <returns>Language object</returns>
		public Language Load(string filename)
		{
			return Load(File.OpenRead(filename));
		}

		public Language Load(Stream stream)
		{
			_Styles = new Dictionary<string, TextStyle>();
			_Blocks = new Dictionary<string, BlockType>();
			_Language = new Language();

			XDocument langDocument = XDocument.Load(stream);

			if (_UseUserCustomStyles && Directory.Exists(_UserCustomStyles))
			{
				string langName = langDocument.Element("Language").Attribute("Name").Value;

				string path = Path.Combine(SyntaxLoader.UserCustomStyles, langName + ".conf");

				if (File.Exists(path))
				{
					// XDocument userConf = XDocument.Load(File.OpenRead(filename));

					//IEnumerable<XElement> xlist = langDocument.Element("Language").Elements("Style");

					//foreach (XElement current in xlist)
					//{
					//  XElement userStyleNode = userConf.SelectSingleNode("styles/Style[@Name='" +
					//      current.Attributes["Name"].InnerText + "']");

					//  if (userStyleNode == null)
					//    continue;

					//  foreach (XmlAttribute userAtt in userStyleNode.Attributes)
					//  {
					//    current.Attributes[userAtt.LocalName].InnerText = userAtt.InnerText;
					//  }
					//}
				}
			}

			ReadLanguageDef(langDocument);

			return _Language;
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="File"></param>
		///// <returns></returns>
		//public Language Load(string File, string Separators)
		//{
		//  _Styles = new Hashtable();
		//  _Blocks = new Hashtable();
		//  _Language = new Language();
		//  _Language.Separators = Separators;

		//  XmlDocument myXmlDocument = new XmlDocument();
		//  myXmlDocument.Load(File);
		//  ReadLanguageDef(myXmlDocument);

		//  return _Language;
		//}

		/// <summary>
		/// Load a specific language from an xml string
		/// </summary>
		/// <param name="XML"></param>
		/// <returns></returns>
		public Language LoadXML(string xml)
		{
			_Styles = new Dictionary<string, TextStyle>();
			_Blocks = new Dictionary<string, BlockType>();
			_Language = new Language();

			XDocument xDoc = XDocument.Parse(xml);
			ReadLanguageDef(xDoc);

			return _Language;
		}

		private void ReadLanguageDef(XDocument xml)
		{
			ParseLanguage(xml.Element("Language"));
		}

		private void ParseLanguage(XElement node)
		{

			//get language name and startblock
			string name = "";
			string startBlock = "";

			foreach (XAttribute att in node.Attributes())
			{
				if (att.Name.LocalName.ToLower() == "name")
					name = att.Value;

				if (att.Name.LocalName.ToLower() == "startblock")
					startBlock = att.Value;
			}

			_Language.Name = name;
			_Language.MainBlock = GetBlock(startBlock);

			foreach (XElement n in node.Elements())
			{
				if (n.NodeType == XmlNodeType.Element)
				{
					if (n.Name.LocalName.ToLower() == "filetypes")
						ParseFileTypes(n);
					if (n.Name.LocalName.ToLower() == "block")
						ParseBlock(n);
					if (n.Name.LocalName.ToLower() == "style")
						ParseStyle(n);
				}
			}
		}

		private void ParseFileTypes(XElement node)
		{

			foreach (XElement current in node.Elements())
			{
				if (current.NodeType == XmlNodeType.Element)
				{
					if (current.Name.LocalName.ToLower() == "filetype")
					{
						//add filetype
						string Extension = "";
						string Name = "";
						foreach (XAttribute a in current.Attributes())
						{
							if (a.Name.LocalName.ToLower() == "name")
								Name = a.Value;
							if (a.Name.LocalName.ToLower() == "extension")
								Extension = a.Value;
						}
						FileType ft = new FileType();
						ft.Extension = Extension;
						ft.Name = Name;
						_Language.FileTypes.Add(ft);
					}
				}
			}
		}

		private void ParseBlock(XElement node)
		{
			string Name = "", Style = "", PatternStyle = "";
			bool IsMultiline = false;
			bool TerminateChildren = false;
			Color BackColor = Colors.Transparent;
			foreach (XAttribute att in node.Attributes())
			{
				if (att.Name.LocalName.ToLower() == "name")
					Name = att.Value;
				if (att.Name.LocalName.ToLower() == "style")
					Style = att.Value;
				if (att.Name.LocalName.ToLower() == "patternstyle")
					PatternStyle = att.Value;
				if (att.Name.LocalName.ToLower() == "ismultiline")
					IsMultiline = bool.Parse(att.Value);
				if (att.Name.LocalName.ToLower() == "terminatechildren")
					TerminateChildren = bool.Parse(att.Value);
				if (att.Name.LocalName.ToLower() == "backcolor")
				{
					BackColor = new Color().FromHex(att.Value);
					//Transparent =false;
				}
			}

			//create block object here
			BlockType bl = GetBlock(Name);
			bl.BackColor = BackColor;
			bl.Name = Name;
			bl.MultiLine = IsMultiline;
			bl.Style = GetStyle(Style);
			bl.TerminateChildren = TerminateChildren;

			foreach (XElement n in node.Elements())
			{
				if (n.NodeType == XmlNodeType.Element)
				{
					if (n.Name.LocalName.ToLower() == "scope")
					{
						string Start = "";
						string End = "";
						string style = "";
						string text = "";
						string EndIsSeparator = "";
						string StartIsSeparator = "";
						string StartIsComplex = "false";
						string EndIsComplex = "false";
						string StartIsKeyword = "false";
						string EndIsKeyword = "false";
						string spawnstart = "";
						string spawnend = "";
						string EscapeChar = "";
						string CauseIndent = "false";

						bool expanded = true;

						foreach (XAttribute att in n.Attributes())
						{
							if (att.Name.LocalName.ToLower() == "start")
								Start = att.Value;
							if (att.Name.LocalName.ToLower() == "escapechar")
								EscapeChar = att.Value;
							if (att.Name.LocalName.ToLower() == "end")
								End = att.Value;
							if (att.Name.LocalName.ToLower() == "style")
								style = att.Value;
							if (att.Name.LocalName.ToLower() == "text")
								text = att.Value;
							if (att.Name.LocalName.ToLower() == "defaultexpanded")
								expanded = bool.Parse(att.Value);
							if (att.Name.LocalName.ToLower() == "endisseparator")
								EndIsSeparator = att.Value;
							if (att.Name.LocalName.ToLower() == "startisseparator")
								StartIsSeparator = att.Value;
							if (att.Name.LocalName.ToLower() == "startiskeyword")
								StartIsKeyword = att.Value;
							if (att.Name.LocalName.ToLower() == "startiscomplex")
								StartIsComplex = att.Value;
							if (att.Name.LocalName.ToLower() == "endiscomplex")
								EndIsComplex = att.Value;
							if (att.Name.LocalName.ToLower() == "endiskeyword")
								EndIsKeyword = att.Value;
							if (att.Name.LocalName.ToLower() == "spawnblockonstart")
								spawnstart = att.Value;
							if (att.Name.LocalName.ToLower() == "spawnblockonend")
								spawnend = att.Value;
							if (att.Name.LocalName.ToLower() == "causeindent")
								CauseIndent = att.Value;
						}
						if (Start != "")
						{
							//bl.StartPattern =new Pattern (Pattern,IsComplex,false,IsSeparator);
							//bl.StartPatterns.Add (new Pattern (Pattern,IsComplex,IsSeparator,true));
							Scope scop = new Scope();
							scop.Style = GetStyle(style);
							scop.ExpansionText = text;
							scop.DefaultExpanded = expanded;
							bool blnStartIsComplex = bool.Parse(StartIsComplex);
							bool blnEndIsComplex = bool.Parse(EndIsComplex);
							bool blnCauseIndent = bool.Parse(CauseIndent);
							scop.CauseIndent = blnCauseIndent;

							Pattern StartP = new Pattern(Start, blnStartIsComplex, false, bool.Parse(StartIsKeyword));
							Pattern EndP = null;
							if (EscapeChar != "")
							{
								EndP = new Pattern(End, blnEndIsComplex, false, bool.Parse(EndIsKeyword), EscapeChar);
							}
							else
							{
								EndP = new Pattern(End, blnEndIsComplex, false, bool.Parse(EndIsKeyword));
							}

							if (EndIsSeparator != "")
								EndP.IsSeparator = bool.Parse(EndIsSeparator);
							scop.Start = StartP;
							scop.EndPatterns.Add(EndP);
							bl.ScopePatterns.Add(scop);
							if (spawnstart != "")
							{
								scop.SpawnBlockOnStart = GetBlock(spawnstart);
							}
							if (spawnend != "")
							{
								scop.SpawnBlockOnEnd = GetBlock(spawnend);
							}
						}
					}
					if (n.Name.LocalName.ToLower() == "bracket")
					{
						string Start = "";
						string End = "";
						string style = "";

						string EndIsSeparator = "";
						string StartIsSeparator = "";

						string StartIsComplex = "false";
						string EndIsComplex = "false";

						string StartIsKeyword = "false";
						string EndIsKeyword = "false";
						string IsMultiLineB = "true";

						foreach (XAttribute att in n.Attributes())
						{
							if (att.Name.LocalName.ToLower() == "start")
								Start = att.Value;
							if (att.Name.LocalName.ToLower() == "end")
								End = att.Value;
							if (att.Name.LocalName.ToLower() == "style")
								style = att.Value;
							if (att.Name.LocalName.ToLower() == "endisseparator")
								EndIsSeparator = att.Value;
							if (att.Name.LocalName.ToLower() == "startisseparator")
								StartIsSeparator = att.Value;
							if (att.Name.LocalName.ToLower() == "startiskeyword")
								StartIsKeyword = att.Value;
							if (att.Name.LocalName.ToLower() == "startiscomplex")
								StartIsComplex = att.Value;
							if (att.Name.LocalName.ToLower() == "endiscomplex")
								EndIsComplex = att.Value;
							if (att.Name.LocalName.ToLower() == "endiskeyword")
								EndIsKeyword = att.Value;
							if (att.Name.LocalName.ToLower() == "ismultiline")
								IsMultiLineB = att.Value;
						}
						if (Start != "")
						{
							PatternList pl = new PatternList();
							pl.Style = GetStyle(style);

							bool blnStartIsComplex = bool.Parse(StartIsComplex);
							bool blnEndIsComplex = bool.Parse(EndIsComplex);
							bool blnIsMultiLineB = bool.Parse(IsMultiLineB);

							Pattern StartP = new Pattern(Start, blnStartIsComplex, false, bool.Parse(StartIsKeyword));
							Pattern EndP = new Pattern(End, blnEndIsComplex, false, bool.Parse(EndIsKeyword));

							StartP.MatchingBracket = EndP;
							EndP.MatchingBracket = StartP;
							StartP.BracketType = BracketType.StartBracket;
							EndP.BracketType = BracketType.EndBracket;
							StartP.IsMultiLineBracket = EndP.IsMultiLineBracket = blnIsMultiLineB;

							pl.Add(StartP);
							pl.Add(EndP);
							bl.OperatorsList.Add(pl);
						}
					}
				}

				if (n.Name.LocalName.ToLower() == "keywords")
					foreach (XElement cn in n.Elements())
					{
						if (cn.Name.LocalName.ToLower() == "patterngroup")
						{
							PatternList pl = new PatternList();
							bl.KeywordsList.Add(pl);
							foreach (XAttribute att in cn.Attributes())
							{
								if (att.Name.LocalName.ToLower() == "style")
									pl.Style = GetStyle(att.Value);

								if (att.Name.LocalName.ToLower() == "name")
									pl.Name = att.Value;

								if (att.Name.LocalName.ToLower() == "normalizecase")
									pl.NormalizeCase = bool.Parse(att.Value);

								if (att.Name.LocalName.ToLower() == "casesensitive")
									pl.CaseSensitive = bool.Parse(att.Value);

							}
							foreach (XElement pt in cn.Elements())
							{
								if (pt.Name.LocalName.ToLower() == "pattern")
								{
									bool IsComplex = false;
									bool IsSeparator = false;
									string Category = null;
									string Pattern = "";
									if (pt.Attributes() != null)
									{
										foreach (XAttribute att in pt.Attributes())
										{
											if (att.Name.LocalName.ToLower() == "text")
												Pattern = att.Value;
											if (att.Name.LocalName.ToLower() == "iscomplex")
												IsComplex = bool.Parse(att.Value);
											if (att.Name.LocalName.ToLower() == "isseparator")
												IsSeparator = bool.Parse(att.Value);
											if (att.Name.LocalName.ToLower() == "category")
												Category = (att.Value);

										}
									}
									if (Pattern != "")
									{
										Pattern pat = new Pattern(Pattern, IsComplex, IsSeparator, true);
										pat.Category = Category;
										pl.Add(pat);
									}

								}
								else if (pt.Name.LocalName.ToLower() == "patterns")
								{
									string Patterns = pt.Value;
									Patterns = Patterns.Replace("\t", " ");
									while (Patterns.IndexOf("  ") >= 0)
										Patterns = Patterns.Replace("  ", " ");


									foreach (string Pattern in Patterns.Split())
									{
										if (Pattern != "")
											pl.Add(new Pattern(Pattern, false, false, true));
									}
								}
							}
						}
					}
				//if (n.Name == "Operators")
				//	ParseStyle(n);
				if (n.Name.LocalName.ToLower() == "operators")
					foreach (XElement cn in n.Elements())
					{
						if (cn.Name.LocalName.ToLower() == "patterngroup")
						{
							PatternList pl = new PatternList();
							bl.OperatorsList.Add(pl);
							foreach (XAttribute att in cn.Attributes())
							{
								if (att.Name.LocalName.ToLower() == "style")
									pl.Style = GetStyle(att.Value);

								if (att.Name.LocalName.ToLower() == "name")
									pl.Name = att.Value;

								if (att.Name.LocalName.ToLower() == "normalizecase")
									pl.NormalizeCase = bool.Parse(att.Value);

								if (att.Name.LocalName.ToLower() == "casesensitive")
									pl.CaseSensitive = bool.Parse(att.Value);
							}

							foreach (XElement pt in cn.Elements())
							{
								if (pt.Name.LocalName.ToLower() == "pattern")
								{
									bool IsComplex = false;
									bool IsSeparator = false;
									string Pattern = "";
									string Category = null;
									if (pt.Attributes() != null)
									{
										foreach (XAttribute att in pt.Attributes())
										{
											if (att.Name.LocalName.ToLower() == "text")
												Pattern = att.Value;
											if (att.Name.LocalName.ToLower() == "iscomplex")
												IsComplex = bool.Parse(att.Value);
											if (att.Name.LocalName.ToLower() == "isseparator")
												IsSeparator = bool.Parse(att.Value);
											if (att.Name.LocalName.ToLower() == "category")
												Category = (att.Value);

										}
									}
									if (Pattern != "")
									{
										Pattern pat = new Pattern(Pattern, IsComplex, IsSeparator, false);
										pat.Category = Category;
										pl.Add(pat);
									}
								}
								else if (pt.Name.LocalName.ToLower() == "patterns")
								{
									string Patterns = pt.Value;
									Patterns = Patterns.Replace("\t", " ");
									while (Patterns.IndexOf("  ") >= 0)
										Patterns = Patterns.Replace("  ", " ");

									string[] pattSplit = Patterns.Split();

									foreach (string Pattern in pattSplit)
									{
										if (Pattern != "")
											pl.Add(new Pattern(Pattern, false, false, false));
									}
								}
							}
						}
					}

				if (n.Name.LocalName.ToLower() == "childblocks")
				{
					foreach (XElement cn in n.Elements())
					{
						if (cn.Name.LocalName.ToLower() == "child")
						{
							foreach (XAttribute att in cn.Attributes())
								if (att.Name.LocalName.ToLower() == "name")
									bl.ChildBlocks.Add(GetBlock(att.Value));
						}
					}
				}
			}
		}


		//done
		private TextStyle GetStyle(string Name)
		{
			if (_Styles.ContainsKey(Name) == false)
			{
				TextStyle s = new TextStyle();
				_Styles.Add(Name, s);
			}

			return (TextStyle)_Styles[Name];
		}

		//done
		private BlockType GetBlock(string Name)
		{
			if (_Blocks.ContainsKey(Name) == false)
			{
				Fireball.Syntax.BlockType b = new Fireball.Syntax.BlockType(_Language);
				_Blocks.Add(Name, b);
			}
			return (BlockType)_Blocks[Name];
		}

		//done
		private void ParseStyle(XElement node)
		{
			string Name = "";
			string ForeColor = "", BackColor = "";
			bool Bold = false, Italic = false, Underline = false;


			foreach (XAttribute att in node.Attributes())
			{
				if (att.Name.LocalName.ToLower() == "name")
					Name = att.Value;

				if (att.Name.LocalName.ToLower() == "forecolor")
					ForeColor = att.Value;

				if (att.Name.LocalName.ToLower() == "backcolor")
					BackColor = att.Value;

				if (att.Name.LocalName.ToLower() == "bold")
					Bold = bool.Parse(att.Value);

				if (att.Name.LocalName.ToLower() == "italic")
					Italic = bool.Parse(att.Value);

				if (att.Name.LocalName.ToLower() == "underline")
					Underline = bool.Parse(att.Value);
			}

			TextStyle st = GetStyle(Name);

			st.BackColor = GetColor(BackColor);
			st.ForeColor = GetColor(ForeColor);
			
			st.Bold = Bold;
			st.Italic = Italic;
			st.Underline = Underline;
			st.Name = Name;
		}

		private static Color GetColor(string color)
		{
			if( color.StartsWith("#"))
			{
				return new Color().FromHex(color);
			}

			var type = typeof (System.Windows.Media.Colors);
			var colorProperty = type.GetProperty(color);
			if (null != colorProperty)
			{
				var actualColor = (System.Windows.Media.Color) colorProperty.GetValue(null, null);
				return actualColor;
			}
			return Colors.Black;
		}
	}
}
