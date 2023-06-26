using System.Numerics;

namespace ComputationalGeometry;

public readonly struct Segment2 : IComparable<Segment2>
{
	/// <summary>
	/// The start of the line segment.
	/// </summary>
	public readonly Vector2 Start;

	/// <summary>
	/// The end of the line segment.
	/// </summary>
	public readonly Vector2 End;

	/// <summary>
	/// The interval of the segment projected onto the X-axis.
	/// </summary>
	public Interval XInterval
		=> new Interval(Start.X, End.X);

	/// <summary>
	///	The interval of the line segment projected onto the Y-axis.
	/// </summary>
	public Interval YInterval
		=> new Interval(Start.Y, End.Y);
	
	/// <summary>
	///	The vector representing the difference between the end and start points.
	/// </summary>
	public Vector2 Delta => End - Start;

	public float Length => Delta.Length();

	public Segment2(Vector2 start, Vector2 end)
	{
		Start = start;
		End = end;
	}

	public Vector2 Lerp(float fraction) => Vector2.Lerp(Start, End, fraction);

	public int CompareTo(Segment2 other) 
		=> Start.X.CompareTo(other.Start.X);
	
	public (bool intersects, Vector2 intersection) FindIntersection(Segment2 other)
	{
		var deltaThis = Delta;
		var deltaOther = other.Delta;
		var deltaStart = other.Start - Start;
		float perpDot = deltaThis.PerpDot(deltaOther);

		if (Math.Abs(perpDot) < float.Epsilon)
		{
			return (false, Vector2.Zero);  // Lines are parallel
		}

		// How far along the intersection is on the segment. 
		float fractionAlongThis = deltaStart.PerpDot(deltaThis) / perpDot;
		float fractionAlongOther = deltaStart.PerpDot(deltaOther) / perpDot;
		
		bool intersects = Interval.Unit.Contains(fractionAlongOther) && Interval.Unit.Contains(fractionAlongThis);  // Check if intersection point is within both segments
		
		return !intersects 
			? (false, Vector2.Zero) 
			: (intersects, Lerp(fractionAlongThis));
	}
}
