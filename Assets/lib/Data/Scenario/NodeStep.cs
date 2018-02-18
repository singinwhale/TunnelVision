
using lib.Data.Node;
using UnityEngine;

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
				DefaultLength = (int)RenderSettings.fogEndDistance/5;
			}

			public int DefaultLength { get; private set; }
		}
	}
}