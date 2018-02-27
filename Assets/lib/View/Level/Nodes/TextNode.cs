using System;
using lib.Data.Scenario;
using lib.System;
using lib.System.Level;
using lib.View.Shapers;
using UnityEngine;

namespace lib.View.Level.Nodes
{
	public class TextNode : LevelNode
	{
		public override void Initialize(Level level, LevelNodeController controller)
		{
			Debug.Assert(controller.Step is Scenario.TextStep, "Given scenariostep is no TextStep!");
			
			_shaper = new LinearShaper(controller.Offset, controller.Length);
			//var textMesh = gameObject.AddComponent<TextMesh>();
			//textMesh.text = ((Scenario.TextStep)controller.Step).Text;
			
			base.Initialize(level, controller);
		}

		public override void Tick()
		{
			//this is for debugging purposes only
			if (World.Instance.LevelController.Camera.Progress > Offset + Length-3 && Length < 6)
			{
				Controller.Length++;
			}
			base.Tick();
		}
	}
}