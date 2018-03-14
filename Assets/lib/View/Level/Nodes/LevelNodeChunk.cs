using System.Collections.Generic;
using lib.Data.Config;
using lib.System;
using MathNet.Numerics.Optimization;
using UnityEngine;

namespace lib.View.Level.Nodes
{
    /// <summary>
    /// Manages the Geometry of a part of a levelNode.
    /// The Geometry is not genereated here. it is only the one displaying and storing it
    /// </summary>
    public class LevelNodeChunk : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        [SerializeField]
        private int _offset;
        [SerializeField]
        private int _length;

        /// <summary>
        /// Mesh that should replace the current one.
        /// </summary>
        /// <remarks>This variable is set from a different thread!</remarks>
        public MeshData NewMeshData;
        public volatile bool NewMeshDataIsReady = false;

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
            get { return _meshRenderer.material; }
        }
        

        public void Initialize()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();

            var materialPath = Config.Instance.Global.Level.Mesh.Material.Path;
            var m = Resources.Load<Material>(materialPath);
            _meshRenderer.material = m;
        }

        public void Update()
        {
            if (NewMeshDataIsReady)
            {
                Skin(NewMeshData, World.Instance.Level.StyleData);
                NewMeshDataIsReady = false;
            }
        }

        public void Skin(MeshData meshData, LevelStyleData styleData)
        {
            var meshFilterSharedMesh = new Mesh();
            meshFilterSharedMesh.vertices = meshData.Vertices.ToArray();
            meshFilterSharedMesh.triangles = meshData.Triangles.ToArray();
            meshFilterSharedMesh.uv = meshData.UVs.ToArray();
            _meshFilter.sharedMesh = meshFilterSharedMesh;
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

        public struct MeshData
        {
            public List<Vector3> Vertices;
            public List<int> Triangles;
            public List<Vector2> UVs;
        }

    }
}