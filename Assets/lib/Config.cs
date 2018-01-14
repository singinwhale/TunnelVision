using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.XPath;
using UnityEditor;
using UnityEngine;

namespace Assets.lib
{
	public class Config
	{
		private static Config _instance = null;
		private static Config Instance
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

		public void LoadAll()
		{
			var processesData = GetProcessData();

			foreach (var stream in processesData)
			{
				XPathDocument doc = new XPathDocument(stream);
				
			}
			
		}

		/// <summary>
		/// Searches for all TextAssets in the Assets/config/processes folder an returns all of them as a Stream Array.
		/// </summary>
		/// <returns>(String-)Streams that represent all TextAssets found</returns>
		private Stream[] GetProcessData()
		{
			List<Stream> processes = new List<Stream>();

			var guids = AssetDatabase.FindAssets("t:TextAsset", new []{"Assets/config/processes"});
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

				Stream str = new MemoryStream(Encoding.UTF8.GetBytes(asset.text));

				processes.Add(str);
			}
			return processes.ToArray();
		}
	}
}