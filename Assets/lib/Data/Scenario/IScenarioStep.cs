using System;

namespace lib.Data.Scenario
{
	public partial class Scenario
	{
		public interface IScenarioStep
		{
			/// <summary>
			/// Default length of the step. If the step can change length this should be at least 3
			/// </summary>
			int DefaultLength { get; }
		}
	}
}