using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine;

namespace Assets.lib.Data.Config
{
	public class Config
	{


		private static Config _instance = null;

		public static Config Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new Config();
				}
				return _instance;
			}
		}

		public List<Process> Processes { get; private set; }

		public Xml.Config Global { get; private set;}

		private Config()
		{
			// read the normal config values from the config.xml file
			var text = Resources.Load<TextAsset>("config/config").text;
			var stream = new StringReader(text);
			XmlSerializer serializer = new XmlSerializer(typeof(Xml.Config));
			Global = (Xml.Config)serializer.Deserialize(stream);

			// load the processes' files
			LoadAllProcesses();
		}

		private void LoadAllProcesses()
		{
			var processesData = ReadProcessFileContents();

			Processes = new List<Process>();
			foreach (var stream in processesData)
			{
				XPathDocument doc = new XPathDocument(stream);
				var xPathNavigator = doc.CreateNavigator();
				xPathNavigator.MoveToFirstChild();
				Process currentProcess = new Process(xPathNavigator);
				Processes.Add(currentProcess);
			}
			
		}

		/// <summary>
		/// Searches for all TextAssets in the Resources/config/processes folder an returns all of them as a Stream Array.
		/// </summary>
		/// <returns>(String-)Streams that represent all TextAssets found</returns>
		private Stream[] ReadProcessFileContents()
		{
			List<Stream> processes = new List<Stream>();

			
			var textassets = Resources.LoadAll<TextAsset>("config/processes/"); 
			foreach (var textasset in textassets)
			{
				Stream str = new MemoryStream(Encoding.UTF8.GetBytes(textasset.text));

				processes.Add(str);
			}
			return processes.ToArray();
		}
	}
}

