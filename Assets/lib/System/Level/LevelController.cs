using System.Collections.Generic;
using Assets.lib.Data;

namespace Assets.lib.System.Level
{
	/// <summary>
	/// This Controller takes care of managing an entire level. Or rather one concrete scenario in a process.
	/// A Level consists of a series of LevelNodes.
	/// </summary>
	public class LevelController
	{
		private Scenario _scenario = null;

		private Queue<LevelNodeController> _steps = new Queue<LevelNodeController>();

		public LevelController(Scenario scenario)
		{
			_scenario = scenario;

			//create new controllers for each step in the scenario
			foreach (var scenarioStep in _scenario.Steps)
			{
				var levelNodeController = new LevelNodeController(scenarioStep);
				_steps.Enqueue(levelNodeController);
			}
		}
	}
}