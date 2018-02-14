using UnityEngine;

namespace lib.View.Shapers
{
	/// <summary>
	/// Shapers define the key points of the level geometry.
	/// </summary>
	public interface IShaper
	{
		float Start { get; set; }
		float Length { get; set; }
		
		Vector3 LastPoint { get; set; } 
		Vector3 LastDirection { get; set; } 
		Vector3 LastNormal{ get; set; }
		
		/// <summary>
		/// Must only be called ONCE!
		/// </summary>
		/// <param name="spline">The shared spline that defines the level</param>
		/// <param name="previous">The previous shaper which provides data about the endpoint of the previous chunk</param>
		/// <param name="length">The length of the corresponding chunk</param>
		void AddShapePointsToSpline(ref BezierSpline.BezierSpline spline, IShaper previous, float length);

		Mesh GetMesh(BezierSpline.BezierSpline spline, IShaper previus, float offset, float length);
	}
}