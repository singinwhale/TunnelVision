using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace lib.Data.Node
{
	public partial class NodeGraph
	{
		private Dictionary<String, Node> _nodes = new Dictionary<string, Node>();

		private Node _startNode = null;

		private String _startNodeID = String.Empty;

		private String _endNodeID = String.Empty;

		public NodeGraph(XPathNavigator xPathNavigator)
		{
			_startNodeID = xPathNavigator.GetAttribute("startNode", "");
			_endNodeID = xPathNavigator.GetAttribute("endNode", "");

			//Create all the nodes first
			xPathNavigator.MoveToChild("Node", "");
			do
			{
				var nodeID = xPathNavigator.GetAttribute("id","");
				if (nodeID == _endNodeID)
				{
					_nodes[nodeID] = new EndNode(xPathNavigator.Clone());
				}
				else
				{
					_nodes[nodeID] = new ProcessNode(xPathNavigator.Clone());
					if (nodeID == _startNodeID)
					{
						_startNode = _nodes[nodeID];
					}
				}
			} while (xPathNavigator.MoveToNext("Node", ""));

			// now set all the connections between them
			xPathNavigator.MoveToFirst();
			do
			{
				var nodeID = xPathNavigator.GetAttribute("id", "");
				var currentNode = _nodes[nodeID];
				if (!(currentNode is EndNode))
				{
					ProcessNode processNode = (ProcessNode) currentNode;
					if (xPathNavigator.MoveToChild("NextNode", ""))
					{
						do
						{
							var id = xPathNavigator.GetAttribute("id", "");
							Node nextNode = null;
							try
							{
								 nextNode = _nodes[id];
							}
							catch (ArgumentNullException e)
							{
								throw new XmlException("NextNode with id=["+id+"] could not be found.",e);
							}
							processNode.NextNodes.Add(nextNode);
						} while (xPathNavigator.MoveToNext("NextNode", ""));
					}
					else
					{
						throw new XmlException("Node[id=" + currentNode.ID +
						                       "] has no next node but is not defined as endNode in the NodeGraph.");
					}
				}
				xPathNavigator.MoveToParent();
			} while (xPathNavigator.MoveToNext("Node",""));
		}
		
		public Node this[String nodeID]
		{
			get { return _nodes[nodeID]; }
		}
	}
}