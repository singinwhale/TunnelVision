using System.Collections.Generic;
using UnityEngine;

namespace lib.View.Shapers
{
	/// <summary>
	/// Shapers define the key points of the level geometry.
	/// </summary>
	public interface IShaper
	{
		int Start { get; set; }
		int Length { get; set; }
		
		List<Vector3> SplinePoints { get; }
		
		Vector3 LastPoint { get;} 
		Vector3 LastDirection { get; } 
		Vector3 LastNormal{ get; }

		/// <summary>
		/// Sets the SplinePoints property to fitting values.
		/// </summary>
		/// <param name="previous">The previous shaper which provides data about the endpoint of the previous chunk</param>
		/// <param name="length">The length of the corresponding chunk</param>
		void UpdateSplinePoints(IShaper previous, int length);

		Mesh GetMesh(BezierSpline.BezierSpline spline, IShaper previus, int offset, int length);
	}
}