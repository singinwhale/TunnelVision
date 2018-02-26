
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lib.System;
using lib.System.Level;
using lib.View.Shapers;
using UnityEngine;
using UnityEngine.Assertions;

namespace lib.View.Level.Nodes
{
	/// <summary>
	/// A part of a level. A level node consists of multiple chunks and is lengthened as needed, pushing back subsequent
	/// nodes.
	/// </summary>
	public  abstract class LevelNode : MonoBehaviour
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

		private int _loadedDistance = 0;

		private const int ChunkLength = 1;

		private static Thread _lastThread = null;
		
		public LevelNodeController Controller { get; set; }
		
		/// <summary>
		/// The Point u on the spline where the node starts
		/// </summary>
		public int Offset
		{
			get { return Controller.Offset; }
		}

		/// <summary>
		/// The length on the spline until the node ends
		/// </summary>
		public int Length
		{
			get { return Controller.Length; }
		}

		/// <summary>
		/// The Class that defines the appearance of this node. Should be set by child classes
		/// </summary>
		public IShaper Shaper
		{
			get { return _shaper; }
		}


		public virtual void Initialize(Level level, LevelNodeController controller)
		{
			_level = level;
			_chunks = new List<LevelNodeChunk>();
			Controller = controller;
		}


		public virtual void OnPlayerEnter()
		{
		}

		public virtual void Tick()
		{
			var progress = World.Instance.LevelController.Camera.Progress;

			//generate the geometry
			if (Length > _loadedDistance && (_chunks.Count == 0 || _chunks.Last().CouldBeVisible()))
			{
				for (int i = _loadedDistance; i < Length; i+= ChunkLength)
				{
					_chunks.Add(LoadChunk(Offset + i,ChunkLength));
				}
				_loadedDistance = Length;
			}
			
			foreach (var chunk in _chunks)
			{
				chunk.gameObject.SetActive(chunk.CouldBeVisible());
			}
		}

		public void OnPreviousNodeChangedLength(LevelNodeController controller)
		{
			Invalidate();
		}

		public void Invalidate()
		{
			for (int i = 0; i < _chunks.Count; i++)
			{
				Destroy(_chunks[i].gameObject);
			}
			_chunks = new List<LevelNodeChunk>();
			_loadedDistance = 0;
		}

		public bool CouldBeVisible()
		{
			var progress = World.Instance.LevelController.Camera.Progress;
			if (progress > Offset && progress < Offset + Length) return true;
			if (_level.Spline.EstimateDistanceOnSpline(progress, Offset) < RenderSettings.fogEndDistance) return true;
			if (_level.Spline.EstimateDistanceOnSpline(progress, Offset+Length) < RenderSettings.fogEndDistance) return true;
			return false;
		}
		
		protected virtual LevelNodeChunk LoadChunk(int offset, int length)
		{
			GameObject chunkGameObject = new GameObject("Level Node Chunk");
			var levelNodeChunk = chunkGameObject.AddComponent<LevelNodeChunk>();
			levelNodeChunk.Initialize();

			// load chunk in seperate thread
			//make a copies so the thread always has the same values as we have here
			var previousThread = _lastThread; 
			var bezierSpline = _level.Spline.Clone();
			var previousShaper = GetPreviousShaper(offset);
			var meshloaderThread = new Thread(
				() =>
				{
					if (previousThread != null)
					{
						//Debug.Log(Thread.CurrentThread.ManagedThreadId+" Waiting for  "+ previousThread.ManagedThreadId);						
						previousThread.Join();
						Thread.Sleep(10);
					}
					
					//Debug.Log("Started "+ Thread.CurrentThread.ManagedThreadId);
					levelNodeChunk.NewMeshData = Shaper.GetMesh(bezierSpline, previousShaper, offset, length);
					levelNodeChunk.NewMeshDataIsReady = true;
			

					//Debug.Log("Finished "+Thread.CurrentThread.ManagedThreadId);
				} 	
			);
			meshloaderThread.Start();
			_lastThread = meshloaderThread;
			
			//levelNodeChunk.NewMeshData = Shaper.GetMesh(_level.Spline, GetPreviousShaper(offset), offset, length);
			//levelNodeChunk.NewMeshDataIsReady = true;
			
			
			
			levelNodeChunk.Length = length;
			levelNodeChunk.Offset = offset;
			
			//make the gameObject a child of this
			levelNodeChunk.transform.parent = transform;
			levelNodeChunk.transform.position = Vector3.zero;
			return levelNodeChunk;
		}


		private IShaper GetPreviousShaper(int offset)
		{
			return Offset == offset ? Controller.Previous.LevelNode.Shaper : Shaper;
		}
	}
}