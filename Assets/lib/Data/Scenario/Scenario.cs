using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using lib.Data.Node;

namespace lib.Data.Scenario
{
	public partial class Scenario
	{
		public String ID { get; private set; }
		public String Name { get; private set; }
		public List<IScenarioStep> Steps { get; private set; }
		public Scenario(NodeGraph graph, XPathNavigator xPathNavigator)
		{
			Steps = new List<IScenarioStep>();

			ID = xPathNavigator.GetAttribute("id", "");
			Name = xPathNavigator.GetAttribute("name", "");

			xPathNavigator.MoveToChild("Script", "");
			xPathNavigator.MoveToFirstChild();
			do
			{
				if (xPathNavigator.LocalName == "Text")
				{
					Steps.Add(new TextStep(xPathNavigator.InnerXml));
				}
				else if (xPathNavigator.LocalName == "Node")
				{
					var node = graph[xPathNavigator.GetAttribute("id", "")];
					Steps.Add(new NodeStep(node));
				}
				else
				{
					throw  new XmlException("Unknown Tagname: "+xPathNavigator.LocalName);
				}
			} while (xPathNavigator.MoveToNext());
		}
		
	}
}