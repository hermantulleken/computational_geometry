// // Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using ColorX;

public static class Program
{
#pragma warning disable CA1416
	static void Main(string[] _)
	{
		string inputImagePath = "test2.png";
		string outputImagePath = "5out.png";

		using var inputImage = new Bitmap(inputImagePath);
		
		var stopwatch = new Stopwatch();

		List<Vector3>? inputColors = null;//[new Vector3(0.5f, 0.5f, 0.5f)];
		List<Vector3> outputColors = [
			"ff716fff".AsVector(),   // Red
			"f9a332ff".AsVector(), // Orange
			"f9ee34ff".AsVector(),   // Yellow
			"7bd9e1ff".AsVector(),   // Green
			"0297feff".AsVector(),   // Blue
			"30287aff".AsVector()    // Purple
		];

		
		stopwatch.Start();
		var outputImage = ColorMapAlgorithm.MapImage(inputImage, inputColors, outputColors);
		stopwatch.Stop();
		Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
		outputImage.Save(outputImagePath, ImageFormat.Png);
		Console.WriteLine($"Processed image saved to {outputImagePath}");
	}
#pragma warning restore CA1416
}
