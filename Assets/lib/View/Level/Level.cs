using System.Collections.Generic;
using System.Linq;
using lib.Data.Scenario;
using lib.System;
using lib.System.Level;
using lib.View.Level.Nodes;
using lib.View.Shapers;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace lib.View.Level
{
    /// <summary>
    /// The high level abstraction of the visible part of the gameworld.
    /// </summary>
    public class Level : MonoBehaviour
    {
        public BezierSpline.BezierSpline Spline { get; private set; }

        public LevelStyleData StyleData { get; private set; }

        [Header("Debug Settings")]
        //Debug settings
        public bool ShowNormals = false;

        public bool ShowTangents = false;

        public bool ShowCurve = false;

        public bool ShowPoints = false;
        
        public bool ShowRawPoints = false;

        public bool ShowLines = false;
        
        
        //Methods
        //--------------------------------------------
        
        public Level()
        {
            Spline = new BezierSpline.BezierSpline();
        }

        // initialize self
        void Awake()
        {
        }

        // communicate with other gameObjects
        void Start()
        {
            //load first node which hopefully already exists
            
        }

        /// <summary>
        /// Tick is independant of Unity's Update method so we have more control
        /// </summary>
        public void Tick()
        {
            var progress = World.Instance.LevelController.Camera.Progress;
        }

        public void OnNodeChangedLength(LevelNodeController nodeController)
        {
            
            //rebuild the parts of the spline points which are necessary
            var listNode = World.Instance.LevelController.CurrentListNode.List.Find(nodeController);
            Debug.Assert(listNode != null, "node is not in List!");
            
            //count the unchanged points
            var iterator = listNode.Previous;
            int accumulator = 0;
            while (iterator != null)
            {
                accumulator += iterator.Value.LevelNode.Shaper.SplinePoints.Count;
                iterator = iterator.Previous;
            }
            var currentPoints = Spline.Points;
            //take the unchanged points
            var unchangedPoints = currentPoints.GetRange(0, accumulator - 1);
            
            // now we build the new pointslist
            var newPoints = new List<Vector3>();
            newPoints.AddRange(unchangedPoints); 
            iterator = listNode;
            while (iterator != null)
            {
                // it does not matter if previous is null because the only shaper that should logically be able to receive
                // a null value at this point is the default shaper and it doesn't care
               iterator.Value.LevelNode.Shaper.UpdateSplinePoints(
                   previous: iterator.Previous == null? null : iterator.Previous.Value.LevelNode.Shaper, 
                   length: iterator.Value.Length
               );

                newPoints.AddRange(iterator.Value.LevelNode.Shaper.SplinePoints);
                iterator = iterator.Next;
            }

            Spline.Points = newPoints;
        }

        public void RebuildSplinePoints()
        {
            var points = new List<Vector3>();
            var iterator = World.Instance.LevelController.Nodes.First;
            IShaper previous = null;
            while (iterator != null)
            {
                iterator.Value.LevelNode.Shaper.UpdateSplinePoints(previous, iterator.Value.Length);
                points.AddRange(iterator.Value.LevelNode.Shaper.SplinePoints);
                previous = iterator.Value.LevelNode.Shaper;
                iterator = iterator.Next;
            }

            Spline.Points = points;
        }
        
        /// <summary>This Method instantiates a LevelNode with the given parameters.</summary>
        /// <remarks>It is called by reflection from LevelNodeController</remarks> 
        // ReSharper disable once UnusedMember.Global
        public TLevelNodeSubclass CreateLevelNode<TLevelNodeSubclass>(LevelNodeController controller) where TLevelNodeSubclass : LevelNode
        {
            var levelNodeGameObject = new GameObject();
            TLevelNodeSubclass theNode = levelNodeGameObject.AddComponent<TLevelNodeSubclass>();
            theNode.Initialize(this, controller);
            return theNode;
        }
        
        void OnDrawGizmos()
        {
            if (Spline == null) return;
            if(ShowLines)
                for (int i = 0; i < Spline.RawPoints.Count - 1; i++)
                {
                    float f = (float)i / (float)Spline.Length;
                    Gizmos.color = new Color(f, 1 - f, 1 - f);
                    Gizmos.DrawLine(Spline.RawPoints[i], Spline.RawPoints[i + 1]);
                }

            if(ShowPoints)
                for (int i = 0; i < Spline.Points.Count; i++)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireCube(Spline.Points[i], Vector3.one);
                }
            
            if(ShowRawPoints)
                for (int i = 0; i < Spline.RawPoints.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(Spline.RawPoints[i], Vector3.one);
                }

            int resolution = 10 ;
        
            if(ShowNormals||ShowCurve || ShowTangents)
                for (int i = 0; i < Spline.Length* resolution - 1; i++)
                {
                    float f = (float)i / ((float)Spline.Length * resolution);
                    Gizmos.color = new Color(1 - f, f, 1 - f);
                    var u1 = (float)i/ resolution;
                    var val1 = Spline[u1];
                    var t1 = Spline.GetDerivative(u1, 1);
                    var u2 = (float)(i + 1)/ resolution;
                    var val2 = Spline[u2];
                    //var t2 = _spline.GetDerivative(u2, 1);

                    //var n1 = Vector3.Cross(Vector3.Cross(t1, t2),t1)/100;;
                    var n1 = Spline.GetNormal(u1)/30;
                    if (ShowCurve)
                        Gizmos.DrawLine(val1, val2);
                    Gizmos.color = new Color(59.0f/255.0f,62.0f / 255.0f,193.0f / 255.0f);
                    if(ShowNormals)
                        Gizmos.DrawLine(val1,val1+n1);
                    Gizmos.color = Color.yellow;
                    if(ShowTangents)
                        Gizmos.DrawLine(val1,val1+t1);
                }
        }
    }
}