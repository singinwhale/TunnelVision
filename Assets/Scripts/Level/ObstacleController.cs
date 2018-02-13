using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ObstacleController : MonoBehaviour
{
	private ObstacleInitializer _initializer;

	private MeshCollider _collider;
	private MeshRenderer _renderer;
	private MeshFilter _mesh;

	private GameObject _textGameObject;

	// Use this for initialization
	void Start ()
	{
		_collider = GetComponent<MeshCollider>();
		_renderer = GetComponent<MeshRenderer>();
		_mesh = GetComponent<MeshFilter>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		var camera = FindObjectOfType<Camera>();
		//_textGameObject.transform.rotation = Quaternion.LookRotation(_initializer.Forward, camera.transform.up);
	}

	public static GameObject Instantiate(ObstacleInitializer initializer)
	{
		var go = new GameObject("Obstacle");
		var collider = go.AddComponent<MeshCollider>();
		var meshfilter = go.AddComponent<MeshFilter>();
		var meshrenderer = go.AddComponent<MeshRenderer>();
		var oc = go.AddComponent<ObstacleController>();
		oc._collider = collider;
		oc._mesh = meshfilter;
		oc._renderer = meshrenderer;

		oc.Initialize(initializer);

		return go;
	}

	public void Initialize(ObstacleInitializer initializer)
	{
		_initializer = initializer;

		var mesh = new Mesh();
		mesh.vertices = initializer.Vertices;
		mesh.triangles = initializer.Triangles;
		_mesh.sharedMesh = mesh;

		_collider.sharedMesh = _mesh.sharedMesh;

		_renderer.sharedMaterial = initializer.Material;
		_renderer.sharedMaterial.color = initializer.Color;


		return;
		//BEGIN instantiate new TextMesh GameObject
		var obj = new GameObject(initializer.Text, typeof(MeshRenderer), typeof(TextMesh));
		obj.transform.parent = transform;

		//calculate average vertex position
		Vector3 acc = Vector3.zero;
		foreach (var vertex in initializer.Vertices) acc += vertex;
		acc /= initializer.Vertices.Length;
		obj.transform.localPosition = acc;

		obj.transform.rotation = Quaternion.LookRotation(-initializer.Forward, initializer.Up);
		

		var textMesh = obj.GetComponent<TextMesh>();
		textMesh.text = initializer.Text;
		textMesh.anchor = TextAnchor.MiddleCenter;
		float resolution = 100;
		textMesh.fontSize = (int)resolution;
		textMesh.transform.localScale = Vector3.one / resolution * 10;
		_textGameObject = obj;
		//END textmesh gameobject instantiation
	}

	public struct ObstacleInitializer
	{
		public Vector3 Forward;
		public Vector3 Up;
		public Vector3[] Vertices;
		public int[] Triangles;
		public String Text;
		public Color Color;
		public Material Material;
	}
	
}
