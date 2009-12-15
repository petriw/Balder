using System;
using System.Collections;
using System.Collections.Generic;

namespace Fireball.Syntax
{
  /// <summary>
  /// A List containing patterns.
  /// this could be for example a list of keywords or operators
  /// </summary>
  public sealed class PatternList : IEnumerable
  {
    private PatternCollection mPatterns = new PatternCollection();

    /// <summary>
    /// for public use only
    /// </summary>
    public Dictionary<string, Pattern> SimplePatterns = new Dictionary<string, Pattern>();

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, Pattern> SimplePatterns1Char = new Dictionary<string, Pattern>();

    /// <summary>
    /// For public use only
    /// </summary>
    public Dictionary<string, PatternCollection> SimplePatterns2Char = new Dictionary<string, PatternCollection>();

    /// <summary>
    /// For public use only
    /// </summary>
    public PatternCollection ComplexPatterns = new PatternCollection();

    /// <summary>
    /// Gets or Sets the TextStyle that should be assigned to patterns in this list
    /// </summary>
    public TextStyle Style = new TextStyle();

    /// <summary>
    /// Gets or Sets if this list contains case seinsitive patterns
    /// </summary>		
    public bool CaseSensitive = false;

    /// <summary>
    /// Gets or Sets if the patterns in this list should be case normalized
    /// </summary>
    public bool NormalizeCase = false;

    /// <summary>
    /// 
    /// </summary>
    public PatternListList Parent = null;

    /// <summary>
    /// The parent BlockType of this list
    /// </summary>
    public BlockType ParentBlock = null;

    /// <summary>
    /// The name of the pattern list
    /// </summary>
    public string Name = "";

    /// <summary>
    /// 
    /// </summary>
    public PatternList()
    {
      SimplePatterns = new Dictionary<string,Pattern>();//new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetEnumerator()
    {
      return mPatterns.GetEnumerator();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Pattern"></param>
    /// <returns></returns>
    public Pattern Add(Pattern Pattern)
    {
      if (this.Parent != null && this.Parent.Parent != null && this.Parent.Parent.Parent != null)
      {
        Pattern.Separators = this.Parent.Parent.Parent.Separators;
        this.Parent.Parent.Parent.ChangeVersion();

      }

      if (!Pattern.IsComplex && !Pattern.ContainsSeparator)
      {
        //store pattern in lookuptable if it is a simple pattern
        string s = "";

        if (Pattern.StringPattern.Length >= 2)
          s = Pattern.StringPattern.Substring(0, 2);
        else
          s = Pattern.StringPattern.Substring(0, 1) + " ";

        s = s.ToLower();

        if (Pattern.StringPattern.Length == 1)
        {
          if( SimplePatterns1Char.ContainsKey(Pattern.StringPattern) == false )
            SimplePatterns1Char.Add(Pattern.StringPattern, Pattern);
          SimplePatterns1Char[Pattern.StringPattern] = Pattern;
        }
        else
        {
          if (SimplePatterns2Char.ContainsKey(s) == false)
            SimplePatterns2Char.Add(s, new PatternCollection());
          PatternCollection ar = (PatternCollection)SimplePatterns2Char[s];
          ar.Add(Pattern);
        }

        if (this.CaseSensitive)
        {
          if (SimplePatterns.ContainsKey(Pattern.LowerStringPattern) == false)
            SimplePatterns.Add(Pattern.LowerStringPattern, Pattern);          
          SimplePatterns[Pattern.LowerStringPattern] = Pattern;
        }
        else
        {
          if (SimplePatterns.ContainsKey(Pattern.StringPattern) == false)
            SimplePatterns.Add(Pattern.StringPattern, Pattern);
          SimplePatterns[Pattern.StringPattern] = Pattern;
        }
      }
      else
      {
        ComplexPatterns.Add(Pattern);
      }

      mPatterns.Add(Pattern);
      if (Pattern.Parent == null)
        Pattern.Parent = this;
      else
      {
        throw (new Exception("Pattern already assigned to another PatternList"));
      }
      return Pattern;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Clear()
    {
      mPatterns.Clear();
    }
  }
}