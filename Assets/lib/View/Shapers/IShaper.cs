using UnityEngine;

namespace Assets.lib.View.Shapers
{
	/// <summary>
	/// Shapers define the key points of the level geometry.
	/// </summary>
	public interface IShaper
	{
		Vector3[] GetNextChunk(Vector3 lastPoint, Vector3 direction, float length);
	}
}