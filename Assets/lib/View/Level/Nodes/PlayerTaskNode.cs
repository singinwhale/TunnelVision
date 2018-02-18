using lib.Data.Scenario;
using lib.System.Level;
using lib.View.Shapers;

namespace lib.View.Level.Nodes
{
	public class PlayerTaskNode : LevelNode
	{
		public override void Initialize(Level level, LevelNodeController controller)
		{
			_shaper = new RandomShaper(controller.Offset, controller.Length);
			base.Initialize(level, controller);
		}
	}
}