namespace ComputationalGeometry;

public readonly struct Interval 
{
	public static readonly Interval Unit = new Interval(0, 1);

	public readonly float Start;
	public readonly float End;
	
	public float Length => End - Start;

	/// <summary>
	/// Gets the sign of the interval based on the comparison between the start and end values.
	/// </summary>
	/// <remarks>
	/// The sign indicates the order of the start and end values of the interval.
	/// A positive sign indicates that the start value is greater than the end value.
	/// A negative sign indicates that the start value is less than the end value.
	/// A sign of zero indicates that the start and end values are equal.
	/// </remarks>
	public int Sign => Start.CompareTo(End);

	public Interval(float start, float end)
	{
		Start = start;
		End = end;
	}

	public bool Intersects(Interval other)
		=> Contains(other.Start) || Contains(other.End);

	public bool Contains(float x)
		=> Start <= x && x <= End;
	
	public bool Contains(Interval other)
		=> Start <= other.Start && other.End <= End;

	public Interval Intersection(Interval other) => new Interval(Math.Max(Start, other.Start), Math.Min(End, other.End));

	public Interval ConvexHull(Interval other) => new Interval(Math.Min(Start, other.Start), Math.Max(End, other.End));

	public override string ToString() => $"[{Start}..{End}]";

	public Interval Scale(float factor)
		=> new Interval(Start * factor, End * factor);

	public Interval Translate(float offset)
		=> new Interval(Start + offset, End + offset);
}
