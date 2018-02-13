using UnityEngine;

namespace Assets.lib.View.Shapers
{
	public class SpiralShaper : IShaper
	{
		public float Start { get; set; }
		public float Length { get; set; }
		public Vector3[] GetChunk(Vector3 lastPoint, Vector3 direction, float offset, float length)
		{
			throw new global::System.NotImplementedException();
		}
	}
}