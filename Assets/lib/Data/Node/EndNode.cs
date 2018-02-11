using System.Xml.XPath;

namespace Assets.lib.Data.Node
{
	partial class NodeGraph
	{
		public class EndNode : Node
		{
			public EndNode(XPathNavigator xPathNavigator) : base(xPathNavigator)
			{
			}
		}
	}
}