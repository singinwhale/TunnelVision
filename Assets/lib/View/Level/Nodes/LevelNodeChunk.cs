using lib.Data.Config;
using UnityEngine;

namespace lib.View.Level.Nodes
{
    /// <summary>
    /// Manages the Geometry of a part of a levelNode.
    /// The Geometry is not genereated here. it is only the one displaying it
    /// </summary>
    public class LevelNodeChunk : MonoBehaviour
    {
        protected MeshFilter _meshFilter;
        protected MeshRenderer _meshRenderer;

        
        public Material Material
        {
            get { return _meshRenderer.sharedMaterial; }
        }
        

        public void Initialize()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
			
            var m = Resources.Load<Material>(Config.Instance.Global.Level.Material.Path);
            _meshRenderer.sharedMaterial = m;
        }

        public void Skin(Mesh mesh, LevelStyleData styleData)
        {
            _meshFilter.sharedMesh = mesh;
        }
        
        

    }
}