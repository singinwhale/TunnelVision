using System.Collections.Generic;
using lib.Data.Scenario;
using lib.System.Player;
using UnityEngine;

namespace lib.System.Level
{
	/// <summary>
	/// This Controller takes care of managing an entire level. Or rather one concrete scenario in a process.
	/// </summary>
	public class LevelController
	{
		private Scenario _scenario = null;

		private LinkedList<LevelNodeController> _nodes = new LinkedList<LevelNodeController>();

		private LinkedListNode<LevelNodeController> _currentListNode;
		
		public View.Level.Level Level { get; private set; }
		
		public CameraController Camera {get; private set;}
		
		public LinkedListNode<LevelNodeController> CurrentListNode
		{
			get { return _currentListNode; }
		}
		
		public LevelNodeController CurrentNode
		{
			get { return _currentListNode.Value; }
		}
		
		public LinkedList<LevelNodeController> Nodes
		{
			get { return _nodes; }
		}
		
		

		public LevelController()
		{
		}

		public void Initialize(Scenario scenario)
		{
			
			_scenario = scenario;

			var levelGameObject = new GameObject("Level", typeof(View.Level.Level));
			Level = levelGameObject.GetComponent<View.Level.Level>();

			Camera = Object.FindObjectOfType<CameraController>();

			LevelNodeController previous = new LevelNodeController();
			_nodes.AddLast(previous);
			//create new controllers for each step in the scenario
			foreach (var scenarioStep in _scenario.Steps)
			{
				var levelNodeController = new LevelNodeController(previous, scenarioStep);
				_nodes.AddLast(levelNodeController);
				levelNodeController.OnFinished += OnNodeFinished;
				levelNodeController.OnLengthChanged += Level.OnNodeChangedLength;
				previous = levelNodeController;
			}
			_currentListNode = _nodes.First;
			
			Level.RebuildSplinePoints();
			
			// instantly enable the first levelcontroller
			Debug.Assert(_currentListNode != null, "No Nodes in List! Are there any steps loaded?");
			CurrentNode.Enable();
		}

		public void Update()
		{
			// update current node
			//CurrentNode.Update();
			foreach (var controller in Nodes)
			{
				controller.Update();
			}
			//tick the entire level
			Level.Tick();
		}

		public void OnNodeFinished(LevelNodeController nodeController)
		{
			Debug.Assert(CurrentNode == nodeController,"Finished node is not the current one!");
			_currentListNode = _currentListNode.Next;
		}

		// here we should set the player back a bit
		public void OnPlayerDeath()
		{
			
		}
	}
}