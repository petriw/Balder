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

namespace Fireball.Syntax
{
  public static class Extensions
  {
    static readonly System.Collections.Generic.List<char> _hex_symbols;
    static Extensions()
    {
        _hex_symbols = new System.Collections.Generic.List<char>( new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' } );
    }
  
    public static Color FromHex(this Color c, string Name)
    {
      if (string.IsNullOrEmpty(Name))
        return new Color();
      Name = Name.Replace("#", "");
      byte[] bytes = FromHexString(Name);
      return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
    }
    
    public static bool IsEmpty(this Color c)
    {
      return c.A == 0 && c.R == 0 && c.G == 0 && c.B == 0;
    }
    
    public static string ToHexString(this Color c)
    {
      return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}",
          c.A, c.R, c.G, c.B);
    }

    public static byte[] FromHexString(string str)
    {
      int i;
      if (str == null)
        return null;
      if ((str.Length % 2) > 0)
        throw new Exception("Wrong argument specified. String length must be even.");
      char[] chr = str.ToCharArray();
      byte[] ret = new byte[chr.Length / 2];
      for (i = 0; i < chr.Length; i += 2)
      {
        if (_hex_symbols.Contains(chr[i]) && _hex_symbols.Contains(chr[i + 1]))
          ret[i / 2] = byte.Parse(chr[i].ToString() + chr[i + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
        else
          throw new Exception("Bad symbol found in hexidecimal string value [" + chr[i] + chr[i + 1]);
      }
      return ret;
    }

    public static bool IsValidHexString(string str)
    {
      int i;
      if (str == null)
        return false;
      char[] chr = str.ToCharArray();
      for (i = 0; i < chr.Length; i += 2)
      {
        if (_hex_symbols.Contains(chr[i]) && _hex_symbols.Contains(chr[i + 1]) == false)
          return false;
      }
      return true;
    }
  }
}
