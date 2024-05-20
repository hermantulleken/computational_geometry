using System.Drawing;

namespace ColorMap;

public static class ColorX
{
	private const char Space = ' ';
	private const char PercentSymbol = '%';
	
	public static Color Parse(string colorString)
	{
		if (TryParse(colorString, out var color))
		{
			return color;
		}

		throw new FormatException($"The color string '{colorString}' is not in a valid format.");
	}
	
	/// <summary>
	/// Tries to parse a <see cref="System.Drawing.Color"/> from a string.
	/// </summary>
	/// <param name="colorString">The string to parse.</param>
	/// <param name="color">The color if the parsing was successful, <see cref="System.Drawing.Color.Black"/> otherwise.</param>
	/// <returns>Whether the parsing was successful.</returns>
	/// <remarks>
	/// The following formats are supported:
	/// <list type="bullet">
	/// <li>Hexadecimal color codes, such as <c>"#FF0000"</c> or <c>"ff0000ff"</c>.</li>
	/// <li>Color names from the <see cref="KnownColor"/> enumeration, such as <c>"Red"</c>.</li>
	/// <li>Color names from the <see cref="KnownColor"/> enumeration with alpha, such as <c>"Red 255"</c>, <c>"Red 1.0"</c>, or
	/// <c>"Red 100%"</c>.</li>
	/// <li>Gray color codes, such as <c>"128"</c>, <c>"0.5"</c>, or <c>"50%"</c>.</li>
	/// <li>Gray color codes with alpha, such as <c>"128 255"</c>, <c>"0.5, 1.0"</c>, or <c>"50% 100%"</c>.</li>
	/// <li>RGB color codes, such as <c>"255 0 0"</c>, <c>"1.0 0.0 0.0"</c>, or <c>"100% 0% 0%"</c>.</li>
	/// <li>RGBA color codes, such as <c>"255 0 0 255"</c>, <c>"1.0 0.0 0.0 1.0"</c>, or <c>"100% 0% 0% 100%"</c>.</li> 
	/// </list>
	/// </remarks>
	public static bool TryParse(string colorString, out Color color)
	{
		if (string.IsNullOrEmpty(colorString))
		{
			return Failed(out color);
		}

		string[] parts = colorString.Split(Space, StringSplitOptions.RemoveEmptyEntries);

		return parts.Length switch
		{
			1 => TryParseSingleToken(parts[0], out color),
			2 => TryParseColorWithAlpha(parts, out color),
			3 => TryParseRgb(parts, out color),
			4 => TryParseRgba(parts, out color),
			_ => Failed(out color)
		};
	}
	
	private static bool Failed(out Color color)
	{
		color = Color.Black;
		return false;
	}

	private static bool TryParseSingleToken(string token, out Color color) 
		=> TryParseToken(token, out color) || TryParseColorFromHex(token, out color);

	private static bool TryParseColorWithAlpha(IReadOnlyList<string> parts, out Color color)
	{
		if (TryParseToken(parts[0], out color) && TryParseNumber(parts[1], out byte alpha))
		{
			color = Color.FromArgb(alpha, color.R, color.G, color.B);
			return true;
		}

		return Failed(out color);
	}

	private static bool TryParseRgb(IReadOnlyList<string> parts, out Color color)
	{
		if (
			TryParseNumber(parts[0], out byte red) && 
			TryParseNumber(parts[1], out byte green) && 
			TryParseNumber(parts[2], out byte blue))
		{
			color = Color.FromArgb(red, green, blue);
			return true;
		}
		
		return Failed(out color);
	}

	private static bool TryParseRgba(IReadOnlyList<string> parts, out Color color)
	{
		if (
			TryParseNumber(parts[0], out byte red) &&
			TryParseNumber(parts[1], out byte green) &&
			TryParseNumber(parts[2], out byte blue) &&
			TryParseNumber(parts[3], out byte alpha))
		{
			color = Color.FromArgb(alpha, red, green, blue);
			return true;
		}
		
		return Failed(out color);
	}

	private static bool TryParseToken(string token, out Color color)
	{
		if (Enum.TryParse(token, true, out KnownColor knownColor))
		{
			color = Color.FromKnownColor(knownColor);
			return true;
		}
				
		if(TryParseNumber(token, out byte gray))
		{
			color = Color.FromArgb(gray, gray, gray);
			return true;
		}
				
		return Failed(out color);
	}
	
	private static bool TryParseNumber(string str, out byte gray)
	{
		if(byte.TryParse(str, out gray))
		{
			return true;
		}
				
		if(float.TryParse(str, out float grayFloat))
		{
			if(grayFloat is < 0 or > 1)
			{
				gray = 0;
				return true;
			}
			
			gray = (byte) (grayFloat * 255);
			return true;
		}
		
		if(TryParsePercentage(str, out grayFloat))
		{
			if(grayFloat is < 0 or > 1)
			{
				gray = 0;
				return true;
			}
			
			gray = (byte) (grayFloat * 255);
			return true;
		}

		gray = 0;
		return false;
	}

	private static bool TryParsePercentage(string str, out float grayFloat)
	{
		if(str[^1] == PercentSymbol)
		{
			return float.TryParse(str[..^1], out grayFloat);
		}
		
		grayFloat = 0;
		return false;
	}
	
	public static bool TryParseColorFromHex(this string hex, out Color color)
	{
		if (hex[0] == '#')
		{
			hex = hex[1..];
		}
		
		if(hex.Length != 6 && hex.Length != 8)
		{
			color = Color.Black;
			return false;
		}

		byte a = 255;
		byte g = 0, b = 0;
		
		bool succeeded =
			byte.TryParse(hex[..2], System.Globalization.NumberStyles.HexNumber, null, out byte r) && 
			byte.TryParse(hex[2..4], System.Globalization.NumberStyles.HexNumber, null, out g) && 
			byte.TryParse(hex[4..6], System.Globalization.NumberStyles.HexNumber, null, out b) &&
			(hex.Length == 6 || byte.TryParse(hex[6..8], System.Globalization.NumberStyles.HexNumber, null, out a));

		if (succeeded)
		{
			color = Color.FromArgb(a, r, g, b);
			return true;
		}

		return Failed(out color);
	}
}
