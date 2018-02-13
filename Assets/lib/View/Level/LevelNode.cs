using Assets.lib.Data.Config;
using Assets.lib.System.Level;
using Assets.lib.View.Shapers;
using UnityEngine;

namespace Assets.lib.View.Level
{
	public abstract class LevelNode : MonoBehaviour
	{
		protected IShaper _shaper = null;
		protected LevelController _levelController = null;
		
		protected MeshFilter _meshFilter;
		protected MeshRenderer _meshRenderer;

		public float Offset { get; protected set; }
		public float Length{ get; protected set; }

		public void Initialize(LevelController level, float offset, float length)
		{
			_levelController = level;
			Offset = offset;
			Length = length;
			
			_meshFilter = gameObject.AddComponent<MeshFilter>();
			_meshRenderer = gameObject.AddComponent<MeshRenderer>();
			
			Material m = Resources.Load<Material>(Config.Instance.Global.Level.Material.Path);
			_meshRenderer.sharedMaterial = m;
			

		}

		protected void SyncMaterialProperties()
		{
			Material m = _meshRenderer.sharedMaterial;

			m.SetFloat("_offset", Offset);
			m.SetFloat("_length", Length);
		}

		public void OnPlayerEnter()
		{
			
		}
	}
}