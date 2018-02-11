using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Assets.lib.Data.Task;

namespace Assets.lib.Data.Node
{
	partial class NodeGraph
	{
		public class ProcessNode : Node
		{


			public List<Node> _nextNodes = new List<Node>();

			public List<Node> NextNodes{get { return _nextNodes; }}

			public ProcessNode(XPathNavigator xPathNavigator) : base(xPathNavigator.Clone())
			{
				//can't initialize nextNodes here because not all nodes have been created yet.
			}
		}
	}
}