using System.Numerics;

namespace ComputationalGeometry;

public static class Geometry
{
	public const double Epsilon = 0.001;

	/// <summary> Returns all the intersections in the given list of segments. </summary>
	public static IEnumerable<(Vector2 intersection, Segment2 segment1, Segment2 segment2)> FindIntersections_BruteForce(IEnumerable<Segment2> intersections) 
		=>  from pair in intersections.DistinctPairs()
			let segment1 = pair.item1
			let segment2 = pair.item2 
			let intersection = segment1.FindIntersection(segment2)
			where intersection.intersects
			select (intersection.intersection, segment1, segment2);
}
