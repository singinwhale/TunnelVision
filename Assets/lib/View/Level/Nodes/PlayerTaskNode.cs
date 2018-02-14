using lib.Data.Scenario;
using lib.View.Shapers;

namespace lib.View.Level.Nodes
{
	public class PlayerTaskNode : LevelNode
	{
		public override void Initialize(Level level, Scenario.IScenarioStep step, float offset, float length)
		{
			_shaper = new SpiralShaper(offset,length);
			base.Initialize(level, step,offset,length);
		}
	}
}