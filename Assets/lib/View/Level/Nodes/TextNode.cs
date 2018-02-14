using System;
using lib.Data.Scenario;
using lib.View.Shapers;
using UnityEngine;

namespace lib.View.Level.Nodes
{
	public class TextNode : LevelNode
	{
		public String Text { get; protected set; }

		public override void Initialize(Level level, Scenario.IScenarioStep step, float offset, float length)
		{
			Debug.Assert(step is Scenario.TextStep, "Given scenariostep is no TextStep!");
			
			_shaper = new SpiralShaper(offset,length);
			var textMesh = gameObject.AddComponent<TextMesh>();
			textMesh.text = ((Scenario.TextStep)step).Text;
			
			base.Initialize(level, step, offset, length);
		}

		public override void OnPlayerEnter()
		{
			base.OnPlayerEnter();
			
			//show text
		}
	}
}