using UnityEngine;

namespace Assets.lib.View.Shapers
{
	/// <summary>
	/// Shapers define the key points of the level geometry.
	/// </summary>
	public interface IShaper
	{
		float Start { get; set; }
		float Length { get; set; }
		
		
		Vector3[] GetChunk(Vector3 lastPoint, Vector3 direction, float offset, float length);
	}
}