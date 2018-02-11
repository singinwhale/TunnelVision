using Assets.lib.View.Shapers;
using UnityEngine;

namespace Assets.lib.View.Level
{
	public class LevelNode : MonoBehaviour
	{
		protected IShaper _shaper = null;

		public LevelNode()
		{
			gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
		}
	}
}