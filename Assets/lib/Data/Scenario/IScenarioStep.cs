using System;

namespace lib.Data.Scenario
{
	public partial class Scenario
	{
		public interface IScenarioStep
		{
			int DefaultLength { get; }
		}
	}
}