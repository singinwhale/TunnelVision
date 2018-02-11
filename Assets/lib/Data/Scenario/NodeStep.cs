using Assets.lib.Data.Node;

namespace Assets.lib.Data
{
	public partial class Scenario
	{
		public class NodeStep : IScenarioStep, NodeGraph.INodeDecorator
		{
			public NodeGraph.Node DecoratedNode { get; set; }

			public NodeStep(NodeGraph.Node decoratedObject)
			{
				DecoratedNode = decoratedObject;
			}
		}
	}
}