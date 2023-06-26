using System.Numerics;

namespace ComputationalGeometry;

public static class Vector2Extensions
{
	public static float Dot(this Vector2 @this, Vector2 other)
		=> @this.X * other.X + @this.Y * other.Y;
	public static float PerpDot(this Vector2 @this, Vector2 other)
		=> @this.X * other.Y - @this.Y * other.X;
	
	public static Vector2 ProjectOnto(this Vector2 @this, Vector2 other)
		=> other * (@this.Dot(other) / other.LengthSquared());
	
	public static Vector2 RejectFrom(this Vector2 @this, Vector2 other)
		=> @this - @this.ProjectOnto(other);

	public static Vector2 Rotate90(this Vector2 @this)
		=> new Vector2(-@this.Y, @this.X);
	
	public static Vector2 Rotate(this Vector2 @this, float angle)
	{
		float cos = MathF.Cos(angle);
		float sin = MathF.Sin(angle);
		
		return new Vector2(
			@this.X * cos - @this.Y * sin,
			@this.X * sin + @this.Y * cos);
	}
	
	public static Vector2 FromAngle(float length, float angle)
		=> new Vector2(
			length * MathF.Cos(angle),
			length * MathF.Sin(angle));

}
