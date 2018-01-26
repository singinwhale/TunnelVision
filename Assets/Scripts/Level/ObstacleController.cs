using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ObstacleController : MonoBehaviour
{


	private MeshCollider _collider;
	private MeshRenderer _renderer;
	private MeshFilter _mesh;

	// Use this for initialization
	void Start ()
	{
		_collider = GetComponent<MeshCollider>();
		_renderer = GetComponent<MeshRenderer>();
		_mesh = GetComponent<MeshFilter>();
	}
	
	// Update is called once per frame
	void Update () {
		
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

		go.hideFlags = HideFlags.DontSave;

		oc.Initialize(initializer);

		return go;
	}

	public void Initialize(ObstacleInitializer initializer)
	{
		var mesh = new Mesh();
		mesh.vertices = initializer.Vertices;
		mesh.triangles = initializer.Triangles;
		_mesh.sharedMesh = mesh;

		_collider.sharedMesh = _mesh.sharedMesh;

		_renderer.sharedMaterial = initializer.Material;
		_renderer.sharedMaterial.color = initializer.Color;
		

		//instantiate new TextMesh GameObject
		var obj = new GameObject(initializer.Text, typeof(MeshRenderer), typeof(TextMesh));
		obj.transform.parent = transform;
		obj.transform.position = Vector3.zero;;

		var textMesh = obj.GetComponent<TextMesh>();
		textMesh.text = initializer.Text;
		textMesh.anchor = TextAnchor.MiddleCenter;
	}

	public struct ObstacleInitializer
	{
		public Vector3[] Vertices;
		public int[] Triangles;
		public String Text;
		public Color Color;
		public Material Material;
	}
	
}
