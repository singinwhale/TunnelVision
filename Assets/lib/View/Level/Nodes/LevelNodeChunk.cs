using lib.Data.Config;
using lib.System;
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
        [SerializeField]
        private int _offset;
        [SerializeField]
        private int _length;


        public int Length
        {
            get { return _length; }
            set
            {
                _length = value; 
                _meshRenderer.material.SetFloat("Length",value);
            }
        }

        public int Offset
        {
            get { return _offset; }
            set
            {
                _offset = value; 
                _meshRenderer.material.SetFloat("Offset",value);
            }
        }

        public Material Material
        {
            get { return _meshRenderer.sharedMaterial; }
        }
        

        public void Initialize()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();

            var materialPath = Config.Instance.Global.Level.Mesh.Material.Path;
            var m = Resources.Load<Material>(materialPath);
            _meshRenderer.material = m;
        }

        public void Skin(Mesh mesh, LevelStyleData styleData)
        {
            _meshFilter.sharedMesh = mesh;
        }
        
        /// <summary>
        /// A Node can be visible if the distance to the camera is less than the fog distance
        /// </summary>
        /// <returns></returns>
        public bool CouldBeVisible()
        {
            var theCamera = World.Instance.LevelController.Camera;
            var cameraProgress = theCamera.Progress;
            var spline = World.Instance.LevelController.Level.Spline;
            
            //were inside it
            if (cameraProgress > Offset &&
                cameraProgress < Offset + Length) 
                return true;
            //not hidden by fog anymore
            if (
                (
                    spline.EstimateDistanceOnSpline(cameraProgress, Offset) <
                        RenderSettings.fogEndDistance ||
                    spline.EstimateDistanceOnSpline(cameraProgress, Offset+Length) <
                        RenderSettings.fogEndDistance
                )
            )
                return true;
            
            return false;
        }
        

    }
}