using lib.System.Level;
using lib.View.Shapers;

namespace lib.View.Level.Nodes
{
    public class DefaultNode : LevelNode
    {
        public override void Initialize(Level level, LevelNodeController controller)
        {
            _shaper = new DefaultShaper();
            base.Initialize(level, controller);
        }

        protected override LevelNodeChunk LoadChunk(int offset, int length)
        {
            // do absolutely nothing here :)
            return null;
        }

        public override void Tick()
        {
            // do absolutely nothing here :)
        }
    }
}