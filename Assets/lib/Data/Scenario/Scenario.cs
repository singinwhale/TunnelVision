using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Assets.lib.Data.Node;

namespace Assets.lib.Data
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

				}
				else if (xPathNavigator.LocalName == "Node")
				{

				}
				else
				{
					throw  new XmlException("Unknown Tagname: "+xPathNavigator.LocalName);
				}
			} while (xPathNavigator.MoveToNext());
		}
		
	}
}