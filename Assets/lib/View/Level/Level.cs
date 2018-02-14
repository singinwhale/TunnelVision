using System.Collections.Generic;
using System.Linq;
using lib.Data.Scenario;
using lib.System;
using lib.View.Level.Nodes;
using UnityEngine;

namespace lib.View.Level
{
    /// <summary>
    /// The high level abstraction of the visible part of the gameworld.
    /// </summary>
    public class Level : MonoBehaviour
    {
        private SortedList<float,LevelNode> _levelNodes;
        
        public BezierSpline.BezierSpline Spline { get; private set; }

        public SortedList<float,LevelNode> LevelNodes
        {
            get { return _levelNodes; }
        }

        public LevelStyleData StyleData { get; private set; }

        public Level()
        {
            Spline = new BezierSpline.BezierSpline();
            _levelNodes = new SortedList<float, LevelNode>();
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
            foreach (var levelNode in _levelNodes)
            {
                if (levelNode.Value.CouldBeVisible())
                {
                    levelNode.Value.Tick();
                }
            }
            
            
            var progress = World.Instance.LevelController.Camera.Progress;
            var currentEntry = _levelNodes.Last(pair => pair.Key < progress);
            var currentNode = currentEntry.Value;
        }

        /// <summary>This Method instantiates a LevelNode with the given parameters.</summary>
        /// <remarks>It is called by reflection from LevelNodeController</remarks>
        // ReSharper disable once UnusedMember.Global. 
        public TLevelNodeSubclass CreateLevelNode<TLevelNodeSubclass>(Scenario.IScenarioStep step, float offset, float length) where TLevelNodeSubclass : LevelNode
        {
            var levelNodeGameObject = new GameObject();
            TLevelNodeSubclass theNode = levelNodeGameObject.AddComponent<TLevelNodeSubclass>();
            theNode.Initialize(this, step,offset,length);
            _levelNodes[offset] = theNode;
            return theNode;
        }
    }
}