using System;
using lib.System.Level.Tasks;
using lib.View.Level.Nodes;

namespace lib.Data.Scenario
{
	partial class Scenario
	{

		public class TextStep : IScenarioStep
		{
			public String Text { get; set; }

			public TextStep(string text)
			{
				Text = text;
				DefaultLength = 4; // text.Length / 10 * 20;
			}

			public int DefaultLength { get; private set; }
		}
	}
}