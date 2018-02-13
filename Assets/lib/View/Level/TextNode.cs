using System;
using Assets.lib.System.Level;

namespace Assets.lib.View.Level
{
	public class TextNode : LevelNode
	{
		public String Text { get; protected set; }

		public new void Initialize(LevelController level, float offset, float length)
		{
			base.Initialize(level, offset,length);
		}
	}
}