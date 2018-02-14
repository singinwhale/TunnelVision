using System;
using lib.Data.Node;
using lib.System.Level.Tasks;
using lib.View.Level.Nodes;

namespace lib.Data.Scenario
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