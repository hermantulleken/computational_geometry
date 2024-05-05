using System.Drawing;
using System.Numerics;

namespace ColorX;

public static class ColorX
{
	public static Vector3 AsVector(this string hex)
	{
		// Parse hexadecimal components
		byte r = byte.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex[2..4], System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex[4..6], System.Globalization.NumberStyles.HexNumber);

		// Normalize RGB values to range [0, 1]
		float rf = r / 255f;
		float gf = g / 255f;
		float bf = b / 255f;

		return new Vector3(rf, gf, bf);
	}
	
	public static Color AsColor(this string hex)
	{
		// Parse hexadecimal components
		byte r = byte.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex[2..4], System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex[4..6], System.Globalization.NumberStyles.HexNumber);

		return Color.FromArgb(255, r, g, b);
	}
}
