using System.Collections.Generic;
using System.Linq;
using lib.Data.Scenario;
using lib.Data.Xml;
using lib.System;
using lib.View.Shapers;
using UnityEngine;

namespace lib.View.Level.Nodes
{
	/// <summary>
	/// A part of a level. A level node consists of multiple chunks and is lengthened as needed, pushing back subsequent
	/// nodes.
	/// </summary>
	public abstract class LevelNode : MonoBehaviour
	{
		/// <summary>
		/// The Class that defines the appearance of this node. Should be set by child classes
		/// </summary>
		protected IShaper _shaper = null;
		
		/// <summary>
		/// Reference to the LevelController of the level that this Node is part of
		/// </summary>
		protected Level _level = null;
		
		/// <summary>
		/// The chunks that are part of this node
		/// </summary>
		protected List<LevelNodeChunk> _chunks;

		private float _offset;
		private float _length;

		private float _loadedDistance = 0;
		
		/// <summary>
		/// The Point u on the spline where the node starts
		/// </summary>
		public float Offset
		{
			get { return _offset; }
			protected set
			{
				_offset = value;
				_shaper.Start = value;
			}
		}

		/// <summary>
		/// The length on the spline until the node ends
		/// </summary>
		public float Length
		{
			get { return _length; }
			protected set
			{
				_length = value;
				_shaper.Length = value;
			}
		}

		public virtual void Initialize(Level level, Scenario.IScenarioStep step, float offset, float length)
		{
			_level = level;
			Offset = offset;
			Length = length;
			_chunks = new List<LevelNodeChunk>();
		}

		public virtual void OnPlayerEnter()
		{
		}

		public virtual void Tick()
		{
			var progress = World.Instance.LevelController.Camera.Progress;
			
			//generate the geometry
			if (_length > _loadedDistance)
			{
				LoadChunk(_offset + _loadedDistance, _length - _loadedDistance);
			}
			
			//make sure the game objects are visible if necessary
			gameObject.SetActive(CouldBeVisible());
		}

		/// <summary>
		/// A Node can be visible if the distance to the camera is less than the fog distance
		/// </summary>
		/// <returns></returns>
		public bool CouldBeVisible()
		{
			if ((World.Instance.LevelController.Camera.transform.position - World.Instance.Level.Spline[_offset]).magnitude <
			    RenderSettings.fogEndDistance) 
				return true;
			if (World.Instance.LevelController.Camera.Progress > _offset ||
			    World.Instance.LevelController.Camera.Progress < _offset + _length) 
				return true;
			return false;
		}

		protected void LoadChunk(float offset, float length)
		{
			GameObject chunkGameObject = new GameObject("Level Node Chunk");
			var levelNodeChunk = chunkGameObject.AddComponent<LevelNodeChunk>();
			levelNodeChunk.Initialize();

			IShaper previous;
			var myIndex = _level.LevelNodes.IndexOfKey(_offset);
			if (myIndex > 0)
			{
				var previousNode = _level.LevelNodes.ElementAt(myIndex - 1).Value;
				previous = previousNode._shaper;
			}
			else // if we are the first
			{
				previous = new DefaultShaper();
			}

			var mesh = _shaper.GetMesh(_level.Spline, previous, offset, length);
			
			levelNodeChunk.Skin(mesh,_level.StyleData);
			
			//make the gameObject a child of this
			levelNodeChunk.transform.parent = transform;
		}

		
	}
}