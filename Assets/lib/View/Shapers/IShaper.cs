using System.Collections.Generic;
using lib.View.Level.Nodes;
using UnityEngine;

namespace lib.View.Shapers
{
	/// <summary>
	/// Shapers define the key points of the level geometry.
	/// </summary>
	public interface IShaper
	{
		/// <summary>
		/// The offset on the spline 
		/// </summary>
		int Start { get; set; }
		
		/// <summary>
		/// The length on the spline. Effectively the number of points 
		/// </summary>
		int Length { get; set; }
		
		/// <summary>
		/// The collection of points which make up this shaper's share of the spline
		/// </summary>
		List<Vector3> SplinePoints { get; }
		
		/// <summary>
		/// 
		/// </summary>
		Vector3 LastPoint { get;}
		
		/// <summary>
		/// The last direction in which the spline was built. Changes depending on whether the Mesh has been built, or not
		/// </summary>
		Vector3 LastDirection { get; } 
		/// <summary>
		/// The last Normal of the shaper. Only valid as soon as the Mesh has been built once.
		/// </summary>
		Vector3 LastNormal{ get; }

		/// <summary>
		/// Sets the SplinePoints property to fitting values.
		/// </summary>
		/// <param name="previous">The previous shaper which provides data about the endpoint of the previous chunk</param>
		/// <param name="length">The length of the corresponding chunk</param>
		void UpdateSplinePoints(IShaper previous, int length);

		/// <summary>
		/// Calculates MeshData based on the given data. This is independant of the SplinePoints propoerty but influences
		/// LastPoint, LastDirection and LastNormal
		/// </summary>
		/// <param name="spline"></param>
		/// <param name="previous"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		LevelNodeChunk.MeshData GetMesh(BezierSpline.BezierSpline spline, IShaper previous, int offset, int length);
	}
}