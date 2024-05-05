using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using MIConvexHull;
using System.Numerics;

public static class ColorMapAlgorithm
{
	private class Vertex : IVertex
	{
		public double[] Position { get; }
		public Vector3 VectorPosition { get;  }
		public readonly int index = -1;

		public Vertex(int index, Vector3 position)
		{
			this.index = index;
			Position = [position.X, position.Y, position.Z];
			VectorPosition = position;
		}
		
		public override string ToString() => $"[{index}] ({Position[0]}, {Position[1]}, {Position[2]})";
	}

	[SuppressMessage("ReSharper", "InconsistentNaming")]
	private class Tetrahedron : TriangulationCell<Vertex, Tetrahedron>
	{
		public int index;
		public Matrix4x4 inverseMatrix;
		public Vector3 normalABC;
		public Vector3 normalABD;
		public Vector3 normalACD;
		public Vector3 normalBCD;

		public override string ToString() 
			=> $"Vertices: {string.Join(", ", Vertices.Select(v => v.ToString()))}";

		public void CalculateFaceNormals()
		{
			// Calculate normals of the faces ABC, ABD, ACD, and BCD
			normalABC = Vector3.Cross(Vertices[1].VectorPosition - Vertices[0].VectorPosition, Vertices[2].VectorPosition - Vertices[0].VectorPosition);
			normalABD = Vector3.Cross(Vertices[1].VectorPosition - Vertices[0].VectorPosition, Vertices[3].VectorPosition - Vertices[0].VectorPosition);
			normalACD = Vector3.Cross(Vertices[2].VectorPosition - Vertices[0].VectorPosition, Vertices[3].VectorPosition - Vertices[0].VectorPosition);
			normalBCD = Vector3.Cross(Vertices[2].VectorPosition - Vertices[1].VectorPosition, Vertices[3].VectorPosition - Vertices[1].VectorPosition);
		}
	}

	private class ColorMap
	{
		private readonly ITriangulation<Vertex,Tetrahedron> delaunay;
		private readonly IReadOnlyList<Vector3> outputs;
		private readonly IReadOnlyList<Tetrahedron> mappedCells;

		public ColorMap(IEnumerable<Vector3> inputColors, IEnumerable<Vector3> outputColors)
		{
			// Define points (including the vertices of a cube and some interior points)
			List<Vector3> inputs =
			[
				//..UnitCube,
				..inputColors
			];
			
			outputs =
			[
				//..UnitCube,
				..outputColors
			];
			
			var vertices = inputs.Select((p, i) => new Vertex(i, p)).ToList();
			delaunay = Triangulation.CreateDelaunay<Vertex, Tetrahedron>(vertices);
			mappedCells = delaunay.Cells.Select(MapTetrahedron).ToList();
				
			foreach (var cell in delaunay.Cells)
			{
				Console.WriteLine($"Cell: {cell}");
			}

			return;
			
			Tetrahedron MapTetrahedron(Tetrahedron cell, int index)
			{
				cell.index = index;
				var matrix = new Matrix4x4(
					cell.Vertices[0].VectorPosition[0], cell.Vertices[0].VectorPosition[1], cell.Vertices[0].VectorPosition[2], 1,
					cell.Vertices[1].VectorPosition[0], cell.Vertices[1].VectorPosition[1], cell.Vertices[1].VectorPosition[2], 1,
					cell.Vertices[2].VectorPosition[0], cell.Vertices[2].VectorPosition[1], cell.Vertices[2].VectorPosition[2], 1,
					cell.Vertices[3].VectorPosition[0], cell.Vertices[3].VectorPosition[1], cell.Vertices[3].VectorPosition[2], 1
					);
				
				cell.inverseMatrix = InvertMatrix(matrix);
				cell.CalculateFaceNormals();
			
				return new Tetrahedron
				{
					Vertices = cell.Vertices.Select(v => new Vertex(v.index, outputs[v.index])).ToArray(),
					index =  index
				};
			}
		}

		private static Matrix4x4 InvertMatrix(Matrix4x4 matrix)
		{
			Matrix4x4.Invert(matrix, out var inverted);
			return inverted;
		}

		private static Vector4 MultiplyMatrixVector(Matrix4x4 matrix, Vector4 vector) => Vector4.Transform(vector, matrix);

		public Vector3 Map(Vector3 testPoint)
		{
			var containingTetrahedron = FindContainingTetrahedron(testPoint, delaunay.Cells);

			if (containingTetrahedron != null)
			{
				var outputTetrahedron = mappedCells[containingTetrahedron.index];
				var barycentric = CalculateBarycentricCoordinates(testPoint, containingTetrahedron);

				return CalculatePointFromBarycentric(barycentric, outputTetrahedron);
			}
			
			Console.WriteLine($"No tetrahedron contains the test point. {testPoint}");
			return Vector3.Zero;
		}

		private static Tetrahedron? FindContainingTetrahedron(Vector3 point, IEnumerable<Tetrahedron> tetrahedrons) 
			=> tetrahedrons.FirstOrDefault(tet => IsPointInsideTetrahedron(point, tet));

		private static bool IsPointInsideTetrahedron(Vector3 point, Tetrahedron tet) 
			=> IsPointInTetrahedron(
				point, 
				tet.Vertices[0].VectorPosition, 
				tet.Vertices[1].VectorPosition, 
				tet.Vertices[2].VectorPosition, 
				tet.Vertices[3].VectorPosition, tet);

		// Method to check if a point P is inside the tetrahedron formed by points A, B, C, and D
		private static bool IsPointInTetrahedron(
			Vector3 point, 
			Vector3 vertex1,
			Vector3 vertex2, 
			Vector3 vertex3, 
			Vector3 vertex4, Tetrahedron tet)
			=> // Check if point P is on the same side of each face as the point D
				SameSide(point, vertex1, vertex2, vertex3, vertex4, tet.normalABC) &&
				SameSide(point, vertex1, vertex2, vertex4, vertex3, tet.normalABD) &&
				SameSide(point, vertex1, vertex3, vertex4, vertex2, tet.normalACD) &&
				SameSide(point, vertex2, vertex3, vertex4, vertex1, tet.normalBCD);


		// Helper method to check if point P is on the same side of the plane as point D
		private static bool SameSide(Vector3 point, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal)
		{
			var ap = point - a; // Vector from A to P
			var ad = d - a; // Vector from A to D

			float dotP = Vector3.Dot(normal, ap);
			float dotD = Vector3.Dot(normal, ad);

			// Check if the dot products have the same sign
			return dotP * dotD >= 0;
		}

		private static Vector4 CalculateBarycentricCoordinates(Vector3 point, Tetrahedron tet)
		{
			var vector = new Vector4(point.X, point.Y, point.Z, 1);
			return MultiplyMatrixVector(tet.inverseMatrix, vector);
		}

		private static Vector3 CalculatePointFromBarycentric(Vector4 barycentric, Tetrahedron tet)
		{
			var v1 = tet.Vertices[0].VectorPosition * barycentric.X;
			var v2 = tet.Vertices[1].VectorPosition * barycentric.Y;
			var v3 = tet.Vertices[2].VectorPosition * barycentric.Z;
			var v4 = tet.Vertices[3].VectorPosition * barycentric.W;
			
			return v1 + v2 + v3 + v4;
		}
	}

	private const float Epsilon = 0.01f;  // This can be adjusted based on how tight you want the range to be

	public static readonly List<Vector3> UnitCube =
	[
		new Vector3(0, 0, 0), // Black
		new Vector3(1, 0, 0), // Red
		new Vector3(1, 1, 0), // Yellow
		new Vector3(0, 1, 0), // Green
		new Vector3(0, 1, 1), // Cyan
		new Vector3(0, 0, 1), // Blue
		new Vector3(1, 0, 1), // Magenta
		new Vector3(1, 1, 1), // White
	];
	
	public static Bitmap MapImage(
		Bitmap inputImage, 
		IEnumerable<Vector3>? inputColors, 
		IEnumerable<Vector3> outputColors)
	{
#pragma warning disable CA1416
		var outputImage = new Bitmap(inputImage.Width, inputImage.Height, PixelFormat.Format32bppArgb);
		var rect = new Rectangle(0, 0, inputImage.Width, inputImage.Height);
		var inputData = inputImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		var outputData = outputImage.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

		byte[] inputPixels = new byte[inputData.Stride * inputImage.Height];
		byte[] outputPixels = new byte[outputData.Stride * outputImage.Height];
		System.Runtime.InteropServices.Marshal.Copy(inputData.Scan0, inputPixels, 0, inputPixels.Length);
		int width = inputImage.Width;
		
		if (inputColors == null)
		{
			var inColors = outputColors.ToArray();
			var outColors = outputColors.ToArray();
			var distances = new float[outColors.Length];
			bool assigned = false;

			Parallel.For(0, inputImage.Height, y =>
			{
				int inputRowOffset = y * inputData.Stride;
				
				for (int x = 0; x < width; x++)
				{
					int pixelOffset = x * 4; // Assuming 4 bytes per pixel (ARGB)
					int inputPixelOffset = inputRowOffset + pixelOffset;

					byte blue = inputPixels[inputPixelOffset];
					byte green = inputPixels[inputPixelOffset + 1];
					byte red = inputPixels[inputPixelOffset + 2];

					var colorVector = new Vector3(MapColorValue(red), MapColorValue(green), MapColorValue(blue));

					if (assigned)
					{
						for (int i = 0; i < inColors.Length; i++)
						{
							float newDistance = (outColors[i] - colorVector).LengthSquared();

							if (newDistance < distances[i])
							{
								inColors[i] = colorVector;
								distances[i] = newDistance;
							}
						}
					}
					else
					{
						for (int i = 0; i < inColors.Length; i++)
						{
							inColors[i] = colorVector;
							distances[i] = (outColors[i] - inColors[i]).LengthSquared();
						}

						assigned = true;
					}
				}
			});
			
			inputColors = inColors.Select((color, distance) => distance < 0.3f ? color : outColors[distance]).ToArray();
		}
	
		var colorMap = new ColorMap(inputColors, outputColors);

		Parallel.For(0, inputImage.Height, y =>
		{
			int inputRowOffset = y * inputData.Stride;
			int outputRowOffset = y * outputData.Stride;
			for (int x = 0; x < width; x++)
			{
				int pixelOffset = x * 4; // Assuming 4 bytes per pixel (ARGB)
				int inputPixelOffset = inputRowOffset + pixelOffset;
				int outputPixelOffset = outputRowOffset + pixelOffset;

				byte blue = inputPixels[inputPixelOffset];
				byte green = inputPixels[inputPixelOffset + 1];
				byte red = inputPixels[inputPixelOffset + 2];
				byte alpha = inputPixels[inputPixelOffset + 3];

				var colorVector = new Vector3(MapColorValue(red), MapColorValue(green), MapColorValue(blue));
				var mappedColorVector = colorMap.Map(colorVector);

				outputPixels[outputPixelOffset] = (byte)(mappedColorVector.Z * 255);
				outputPixels[outputPixelOffset + 1] = (byte)(mappedColorVector.Y * 255);
				outputPixels[outputPixelOffset + 2] = (byte)(mappedColorVector.X * 255);
				outputPixels[outputPixelOffset + 3] = alpha;
			}
		});

		System.Runtime.InteropServices.Marshal.Copy(outputPixels, 0, outputData.Scan0, outputPixels.Length);

		inputImage.UnlockBits(inputData);
		outputImage.UnlockBits(outputData);
#pragma warning restore CA1416
		return outputImage;
	}
	
	private static float MapColorValue(int colorValue) => colorValue / 255f * (1 - 2 * Epsilon) + Epsilon;
}
