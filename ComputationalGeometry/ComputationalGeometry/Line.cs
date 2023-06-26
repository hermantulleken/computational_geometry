using System.Numerics;

namespace ComputationalGeometry;

public static class Line2
{
	public static Segment2 FromPoints(Vector2 point0, Vector2 point1)
	{
		return new Segment2(point0, point1);
	}

	public static Segment2 FromPointAndDirection(Vector2 point, Vector2 direction)
	{
		var other = point + direction;
		return FromPoints(point, other);
	}

	public static Segment2 FromPointAndNormal(Vector2 point, Vector2 normal)
	{
		var other = point + normal.Rotate90();
		return FromPoints(point, other);
	}

	public static Segment2 FromPointAndAngle(Vector2 point, float angle)
	{
		var other = point + Vector2Extensions.FromAngle(1, angle);
		return FromPoints(point, other);
	}
}
