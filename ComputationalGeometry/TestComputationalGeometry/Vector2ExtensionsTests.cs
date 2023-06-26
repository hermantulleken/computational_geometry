namespace TestComputationalGeometry;

[TestFixture]
public class Vector2ExtensionsTests
{
	#region Dot
	[Test]
	public void Dot_WithBothZeroVectors_ReturnsZero()
	{
		var vector1 = new Vector2(0, 0);
		var vector2 = new Vector2(0, 0);

		float result = vector1.Dot(vector2);

		Assert.That(result, Is.EqualTo(0));
	}

	[Test]
	public void Dot_WithOrthogonalVectors_ReturnsZero()
	{
		var vector1 = new Vector2(1, 0);
		var vector2 = new Vector2(0, 1);

		float result = vector1.Dot(vector2);

		Assert.That(result, Is.EqualTo(0));
	}

	[Test]
	public void Dot_WithSameVectors_ReturnsSquaredMagnitude()
	{
		var vector1 = new Vector2(3, 4); // Vector with magnitude 5 (3-4-5 right triangle)
		var vector2 = new Vector2(3, 4); 

		float result = vector1.Dot(vector2);

		Assert.That(result, Is.EqualTo(25)); // 5^2 = 25
	}

	[Test]
	public void Dot_WithDifferentVectors_ReturnsDotProduct()
	{
		var vector1 = new Vector2(1, 2);
		var vector2 = new Vector2(3, 4);

		float result = vector1.Dot(vector2);

		Assert.That(result, Is.EqualTo(11)); // 1*3 + 2*4 = 11
	}
	#endregion
	
	#region PerpDot
	[Test]
	public void PerpDot_WithBothZeroVectors_ReturnsZero()
	{
		var vector1 = new Vector2(0, 0);
		var vector2 = new Vector2(0, 0);

		float result = vector1.PerpDot(vector2);

		Assert.That(result, Is.EqualTo(0));
	}

	[Test]
	public void PerpDot_WithOrthogonalVectors_ReturnsMagnitudeProduct()
	{
		var vector1 = new Vector2(1, 0);
		var vector2 = new Vector2(0, 1);

		float result = vector1.PerpDot(vector2);

		Assert.That(result, Is.EqualTo(1)); // product of magnitudes, because the vectors are orthogonal
	}

	[Test]
	public void PerpDot_WithParallelVectors_ReturnsZero()
	{
		var vector1 = new Vector2(1, 2);
		var vector2 = new Vector2(2, 4); // vector2 is parallel to vector1

		float result = vector1.PerpDot(vector2);

		Assert.That(result, Is.EqualTo(0));
	}

	[Test]
	public void PerpDot_WithDifferentVectors_ReturnsPerpDotProduct()
	{
		var vector1 = new Vector2(1, 2);
		var vector2 = new Vector2(3, 4);

		float result = vector1.PerpDot(vector2);

		Assert.That(result, Is.EqualTo(-2)); // 1*4 - 2*3 = -2
	}
	#endregion
}
