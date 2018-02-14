/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */

using System.Xml.Serialization;

namespace lib.Data.Xml
{
	[XmlRoot(ElementName = "Resolution")]
	public class Resolution
	{
		[XmlAttribute(AttributeName = "parallel")]
		public int Parallel { get; set; }
		[XmlAttribute(AttributeName = "normal")]
		public int Normal { get; set; }
	}

	[XmlRoot(ElementName = "Mesh")]
	public class Mesh
	{
		[XmlElement(ElementName = "Resolution")]
		public Resolution Resolution { get; set; }
		[XmlElement(ElementName = "Radius")]
		public float Radius { get; set; }
	}

	[XmlRoot(ElementName = "Obstacles")]
	public class Obstacles
	{
		[XmlElement(ElementName = "Density")]
		public float Density { get; set; }
	}

	[XmlRoot(ElementName = "Material")]
	public class Material
	{
		[XmlAttribute(AttributeName = "path")]
		public string Path { get; set; }
	}

	[XmlRoot(ElementName = "Level")]
	public class Level
	{
		[XmlElement(ElementName = "Mesh")]
		public Mesh Mesh { get; set; }
		[XmlElement(ElementName = "Obstacles")]
		public Obstacles Obstacles { get; set; }

		[XmlElement(ElementName = "Material")]
		public Material Material { get; set; }
	}

	[XmlRoot(ElementName = "Camera")]
	public class Camera
	{
		[XmlElement(ElementName = "FieldOfView")]
		public float FieldOfView { get; set; }
		[XmlElement(ElementName = "SampleDstance")]
		public float SampleDstance { get; set; }

		[XmlElement(ElementName = "Speed")]
		public float Speed { get; set; }
	}

	[XmlRoot(ElementName = "Pawn")]
	public class Pawn
	{
		[XmlElement(ElementName = "Speed")]
		public float Speed { get; set; }
	}

	[XmlRoot(ElementName = "Player")]
	public class Player
	{
		[XmlElement(ElementName = "Camera")]
		public Camera Camera { get; set; }
		[XmlElement(ElementName = "Pawn")]
		public Pawn Pawn { get; set; }
	}

	[XmlRoot(ElementName = "Config")]
	public class Config
	{
		[XmlElement(ElementName = "Level")]
		public Level Level { get; set; }
		[XmlElement(ElementName = "Player")]
		public Player Player { get; set; }
	}

}
