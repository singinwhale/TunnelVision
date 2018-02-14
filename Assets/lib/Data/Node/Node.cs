using System.Xml.XPath;
using lib.Data.Task;

namespace lib.Data.Node
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