// // Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using System.Drawing;
using System.Drawing.Imaging;

namespace ImageGenerator;

public class ImageX
{
	public static Bitmap GenerateImage(int width, int height, Color[] cornerColors)
	{
		var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
		
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				var color = Bilerp(cornerColors, x / (float) width, y  / (float) height);
				bitmap.SetPixel(x, y, color);
			}
		}
		
		return bitmap;
	}

	public static Bitmap ToRedCyan(Bitmap input)
	{
		var output = new Bitmap(input.Width, input.Height, PixelFormat.Format32bppArgb);
		
		for (int y = 0; y < input.Height; y++)
		{
			for (int x = 0; x < input.Width; x++)
			{
				var color = input.GetPixel(x, y);
				var red = color.R;
				var cyan = (color.G + color.B)/2;
				var newColor = Color.FromArgb(color.A, red, cyan, cyan);
				output.SetPixel(x, y, newColor);
			}
		}
		
		return output;
	}

	private static Color Bilerp(IReadOnlyList<Color> cornerColors, float u, float v) 
		=> Lerp(
			Lerp(cornerColors[0], cornerColors[1], u),
			Lerp(cornerColors[2], cornerColors[3], u),
			v
		);

	private static Color Lerp(Color a, Color b, float t) 
		=> Color.FromArgb(
			(int) (a.A + (b.A - a.A) * t),
			(int) (a.R + (b.R - a.R) * t),
			(int) (a.G + (b.G - a.G) * t),
			(int) (a.B + (b.B - a.B) * t)
		);
}
