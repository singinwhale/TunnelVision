using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Assets.lib.Data.Node;

namespace Assets.lib.Data
{
	/// <summary>
	/// A Data class that represents a QM-Process. Gives Information about this process like:
	///		- Node Graph
	///		- Questions
	///		- Scenarios
	///		- Color
	///		- Seed
	///		etc.
	/// </summary>
	public class Process
	{
		public string Name { get; private set; }

		public string ID { get; private set; }

		public NodeGraph Graph { get; private set; }

		public List<Scenario> Scenarios { get; private set; }

		public Process(XPathNavigator xPathNavigator)
		{
			Name = xPathNavigator.GetAttribute("name", "");
			ID = xPathNavigator.GetAttribute("id", "");

			// read nodegraph
			if (xPathNavigator.MoveToChild("NodeGraph", ""))
			{
				Graph = new NodeGraph(xPathNavigator.Clone());
			}
			else
			{
				throw new XmlException("Tag 'NodeGraph' could not be found!");
			}

			// Read all scenarios
			Scenarios = new List<Scenario>();
			if (!xPathNavigator.MoveToNext("Scenario", ""))
			{
				throw new XmlException("Tag 'Scenario' could not be found!");
			}
			do
			{
				Scenarios.Add(new Scenario(Graph, xPathNavigator.Clone()));
			} while (xPathNavigator.MoveToNext("Scenario", ""));
		}
	}
}