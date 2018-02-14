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

		private Queue<LevelNodeController> _steps = new Queue<LevelNodeController>();
		private Stack<LevelNodeController> _pastSteps = new Stack<LevelNodeController>();
		
		public View.Level.Level Level { get; private set; }
		
		public CameraController Camera {get; private set;}

		public LevelController()
		{
		}

		public void Initialize(Scenario scenario)
		{
			
			_scenario = scenario;

			var levelGameObject = new GameObject("Level", typeof(View.Level.Level));
			Level = levelGameObject.GetComponent<View.Level.Level>();

			Camera = Object.FindObjectOfType<CameraController>();
			
			//create new controllers for each step in the scenario
			foreach (var scenarioStep in _scenario.Steps)
			{
				var levelNodeController = new LevelNodeController(scenarioStep);
				_steps.Enqueue(levelNodeController);
				levelNodeController.OnFinished += OnNodeFinished;
			}

			// instantly enable the first levelcontroller
			_steps.Peek().Enable();
		}

		public void Update()
		{
			// update current node
			_steps.Peek().Update();
			//tick the entire level
			Level.Tick();
		}

		public void OnNodeFinished(LevelNodeController nodeController)
		{
			Debug.Assert(_steps.Peek() == nodeController,"Finished node is not the current one!");
			_pastSteps.Push(_steps.Dequeue());
		}

		// here we should set the player back a bit
		public void OnPlayerDeath()
		{
			
		}
	}
}