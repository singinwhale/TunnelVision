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

	public void Initialize(ObstacleInitializer initializer)
	{
		_mesh.sharedMesh.vertices = initializer.Vertices;
		_mesh.sharedMesh.triangles = initializer.Triangles;

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
