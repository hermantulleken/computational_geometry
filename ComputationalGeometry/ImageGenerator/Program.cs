// See https://aka.ms/new-console-template for more information

using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

using ColorX;
using ImageGenerator;


/*
Color[] colors =
[
	"00ffff".AsColor(),
	"ffffff".AsColor(),
	"000000".AsColor(),
	"ff0000".AsColor()
];

var colorSpace = ImageX.GenerateImage(256, 256, colors);
colorSpace.Save("colorspace.png", ImageFormat.Png);

var inputForest = (Bitmap) Image.FromFile("forest.png");
var filteredForest = ImageX.ToRedCyan(inputForest);
filteredForest.Save("forest_red_cyan.png", ImageFormat.Png);

Vector3[] inputColors1 =
[
	"724747".AsVector()
];

Vector3[] outputColors1 =
[
	"c42f2f".AsVector()
];

var forestOut1 = ColorMapAlgorithm.MapImage(filteredForest, inputColors1, outputColors1);
var colorSpaceOut1 = ColorMapAlgorithm.MapImage(colorSpace, inputColors1, outputColors1);
forestOut1.Save("forest_out1.png", ImageFormat.Png);
colorSpaceOut1.Save("colorspace_out1.png", ImageFormat.Png);

Vector3[] inputColors2 =
[
	"724747".AsVector(),
	"97b4b4".AsVector()
];

Vector3[] outputColors2 =
[
	"c42f2f".AsVector(),
	"7adadaff".AsVector()
];

var forestOut2 = ColorMapAlgorithm.MapImage(filteredForest, inputColors2, outputColors2);
var colorSpaceOut2 = ColorMapAlgorithm.MapImage(colorSpace, inputColors2, outputColors2);
forestOut2.Save("forest_out2.png", ImageFormat.Png);
colorSpaceOut2.Save("colorspace_out2.png", ImageFormat.Png);
*/

var scene1Input = (Bitmap) Image.FromFile("bleak.png");

var inputColors3 = new[]{
	"859596ff",	
//	"23ace9ff"
}
	.Select(n => n.AsVector())
	.Concat(ColorMapAlgorithm.UnitCube[..7])
	.Concat(ColorMapAlgorithm.UnitCube[7..])
	.ToArray();

var outputColors3 = new[]{
		"ff716fff",
//		"7bd9e1ff",
}
	.Select(n => n.AsVector())
	.Concat(ColorMapAlgorithm.UnitCube[..7])
	.Concat(ColorMapAlgorithm.UnitCube[7..])
	.ToArray();

var scene1Output = ColorMapAlgorithm.MapImage(scene1Input, inputColors3, outputColors3);
scene1Output.Save("bleak_out.png", ImageFormat.Png);
