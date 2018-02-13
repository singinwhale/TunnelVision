using System;
using System.Collections.Generic;
using System.Reflection;
using Assets.lib.Data;
using Assets.lib.View.Level;

namespace Assets.lib.System.Level
{
	public class LevelNodeController
	{
		private Scenario.IScenarioStep _step;
		private LevelNode _levelNode;

		public static Dictionary<Type, Type> ScenarioStepToLevelNodeTypeDictionary = new Dictionary<Type, Type>
		{
			{typeof(Scenario.TextStep), typeof(TextNode)},
			{typeof(Scenario.NodeStep), typeof(PlayerTaskNode)}
		};

		public LevelNodeController(Scenario.IScenarioStep step)
		{
			_step = step;

			//instantiate the corresponding LevelNode for the step
			_levelNode = (LevelNode) Activator.CreateInstance( 
					ScenarioStepToLevelNodeTypeDictionary[step.GetType()],
					BindingFlags.Public|BindingFlags.CreateInstance,
					step
				);
		}
	}
}