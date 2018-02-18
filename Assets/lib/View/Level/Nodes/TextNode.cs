using System;
using lib.Data.Scenario;
using lib.System.Level;
using lib.View.Shapers;
using UnityEngine;

namespace lib.View.Level.Nodes
{
	public class TextNode : LevelNode
	{
		public String Text { get; protected set; }

		public override void Initialize(Level level, LevelNodeController controller)
		{
			Debug.Assert(controller.Step is Scenario.TextStep, "Given scenariostep is no TextStep!");
			
			_shaper = new LinearShaper(controller.Offset, controller.Length);
			var textMesh = gameObject.AddComponent<TextMesh>();
			textMesh.text = ((Scenario.TextStep)controller.Step).Text;
			
			base.Initialize(level, controller);
		}

		public override void OnPlayerEnter()
		{
			base.OnPlayerEnter();
			
			//show text
		}
	}
}