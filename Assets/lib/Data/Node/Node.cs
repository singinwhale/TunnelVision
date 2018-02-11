using System;
using System.Xml.XPath;
using Assets.lib.Data.Task;

namespace Assets.lib.Data.Node
{
	partial class NodeGraph
	{
		public abstract class Node
		{

			public string Name { get; private set; }

			public string ID{ get; private set; }

			public PlayerTask Task
			{
				get; private set;
			}

			protected Node(XPathNavigator xPathNavigator)
			{
				Name = xPathNavigator.GetAttribute("name", "");
				ID = xPathNavigator.GetAttribute("id", "");

				Task = new PlayerTask(xPathNavigator.MoveToChild("PlayerTask", ""));
			}
		}
	}
}