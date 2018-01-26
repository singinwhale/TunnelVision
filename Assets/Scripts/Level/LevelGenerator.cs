using System.Collections;
using System.Collections.Generic;
using Assets.lib;
using Boo.Lang.Environments;
using UnityEngine;
using MathNet.Numerics.Interpolation;
using UnityEditor;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class LevelGenerator : MonoBehaviour
{
    private BezierSpline _spline = null;

	public BezierSpline Spline { get { return _spline; } }

	[Range(1,100)]
    public int MeshResolutionParallel = 3;
    [Range(3, 64)]
    public int MeshResolutionNormal = 16;

    public float Radius = 3;

	public float ObstacleDensity = 2;

    [Range(0,30)]
    public float t;

    //Debug settings
    public bool ShowNormals = false;

	public bool ShowTangents = false;

	public bool ShowCurve = false;

    public bool ShowPoints = false;

    public bool ShowLines = false;
	
	
    // Use this for initialization
    void Start()
    {
        _spline = new BezierSpline();
	    if (_spline == null) _spline = new BezierSpline();

	    List<Vector3> points = new List<Vector3>();
	    for (int i = 0; i <= 2000; i += 20)
	    {
		    points.Add(/*Quaternion.AngleAxis(i / (1000.0f / 360.0f), Vector3.up) **/ new Vector3(i, Mathf.Cos(i / 25.0F) * 10, Mathf.Sin(i / 25.0F) * 10));
	    }
	    _spline.Points = points;

	    UpdateMesh();
		foreach (var obstacleController in GameObject.FindObjectsOfType<ObstacleController>())
		{
			if(UnityEngine.Application.isEditor) DestroyImmediate(obstacleController.gameObject);
			else Destroy(obstacleController.gameObject,0);
		}
	    GenerateObstacles();
    }


	// Update is called once per frame
    void Update ()
    {
        
    }

    void OnDrawGizmos()
    {
        if(ShowLines)
        for (int i = 0; i < _spline.Points.Count - 1; i++)
        {
            float f = (float)i / (float)_spline.Points.Count;
            Gizmos.color = new Color(f, 1 - f, 1 - f);
            Gizmos.DrawLine(_spline.Points[i], _spline.Points[i + 1]);
        }

        if(ShowPoints)
        for (int i = 0; i < _spline.Points.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_spline.Points[i], Vector3.one);
        }

        int resolution = 10 ;
        
        if(ShowNormals||ShowCurve || ShowTangents)
        for (int i = 0; i < _spline.Points.Count* resolution - 1; i++)
        {
            float f = (float)i / ((float)_spline.Points.Count * resolution);
            Gizmos.color = new Color(1 - f, f, 1 - f);
            var u1 = (float)i/ resolution;
            var val1 = _spline[u1];
            var t1 = _spline.GetDerivative(u1, 1);
            var u2 = (float)(i + 1)/ resolution;
            var val2 = _spline[u2];
            var t2 = _spline.GetDerivative(u2, 1);

            var n1 = Vector3.Cross(Vector3.Cross(t1, t2),t1);
            if(ShowCurve)
            Gizmos.DrawLine(val1, val2);
            Gizmos.color = Color.cyan;
            if(ShowNormals)
            Gizmos.DrawLine(val1,val1+n1);
            Gizmos.color = Color.yellow;
			if(ShowTangents)
            Gizmos.DrawLine(val1,val1+t1);
        }
        

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_spline[t],Vector3.one*0.1f);
    }

    public void UpdateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 oldNormal = Vector3.zero;
        for (int i = 0; i < _spline.Points.Count - 1; i++)
        {
            for (int x = 0; x < MeshResolutionParallel; x++)
            {
                float u = i + ((float) x / MeshResolutionParallel);
                Vector3 center = _spline.Evaluate(u);
                Vector3 tangent = _spline.GetDerivative(u, 1);
                Vector3 normal = Vector3.Cross(Vector3.Cross(tangent,_spline.GetDerivative(u+1.0f/MeshResolutionParallel,1)),tangent);

                if (oldNormal == Vector3.zero) oldNormal = normal;
                else
                {
                    normal = Vector3.RotateTowards(oldNormal, normal, Mathf.PI * 2 * 10.0f / 360.0f, 0);
                    oldNormal = normal;
                }

                //Generate tube segment
                for (int n = 0; n < MeshResolutionNormal; n++)
                {
                    //rotate the normal arount the center
                    Vector3 vert = center + Quaternion.AngleAxis(n * 360.0f / MeshResolutionNormal, tangent) * normal.normalized * Radius;
                    vertices.Add(vert);
                }
                //connect tube segment to last one
                if (i != 0 || x > 0)//do not do this for the very first segment
                {
                    //calculate stride per segment
                    int segmentFirstIndex = (i * MeshResolutionParallel + x) * MeshResolutionNormal;
                    int lastSegmentFirstIndex = segmentFirstIndex - MeshResolutionNormal;

                    for (int n = 1; n <= MeshResolutionNormal; n++)
                    {
                        //generate double faced quad between tube segments
                        //using modulo to connect last vertex to first
                        triangles.AddRange(MakeQuad(
                            lastSegmentFirstIndex+n% MeshResolutionNormal,
                            lastSegmentFirstIndex+(n-1)%MeshResolutionNormal,
                            segmentFirstIndex+(n-1)% MeshResolutionNormal,
                            segmentFirstIndex+n% MeshResolutionNormal
                            )
                        );
                    }

                }
            }

        }

        GetComponent<MeshFilter>().sharedMesh.vertices = vertices.ToArray();
        GetComponent<MeshFilter>().sharedMesh.triangles = triangles.ToArray();
    }


	private void GenerateObstacles()
	{
		for (float u = 0; u < _spline.Length; u += ObstacleDensity)
		{
			var initializer = new ObstacleController.ObstacleInitializer();
			initializer.Text = "Test";
			initializer.Color = Color.red;
			//initializer.Material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Meshes/Materials/ObstacleMaterial.mat");
			initializer.Material = this.GetComponent<MeshRenderer>().sharedMaterial;
			SetObstacleMesh(ref initializer,u,Random.value*360);
			var obstacle = ObstacleController.Instantiate(initializer);
			obstacle.transform.position = _spline[u];
		}
	}

	private void SetObstacleMesh(ref ObstacleController.ObstacleInitializer obstacle, float u, float angle)
	{
		Vector3 tangent = _spline.GetDerivative(u, 1);
		Vector3 normal = _spline.GetNormal(u);

		//make only half circle obstacles for now
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		
		
		for (float x = 0; x < Mathf.PI; x += Mathf.PI / MeshResolutionNormal)
		{
			float currAngle = Mathf.Deg2Rad * angle + x;
			var rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * currAngle, tangent.normalized);
			//create vertex on the half circle
			Vector3 vertex = rotation * normal.normalized * Radius *2;
			vertices.Add(vertex);
			//add a second translated one so the obstacle has thickness. Assumes the spline is straight
			vertex += tangent.normalized;
			vertices.Add(vertex);

			//connect the vertices on the outside of the circle
			if (vertices.Count % 2 == 0 && vertices.Count > 2)
			{
				int index = vertices.Count - 1;
				triangles.AddRange(MakeQuad(index, index - 1, index - 3, index - 2));
			}
		}

		//close flat inside
		int lastIndex = vertices.Count - 1;
		triangles.AddRange(MakeQuad(0,1,lastIndex,lastIndex-1));

		//close sides
		for (int i = 2; i < vertices.Count-3; i += 2)
		{
			triangles.AddRange(MakeTri(0,i,i+2));
			triangles.AddRange(MakeTri(1,i+1,i+3));
		}


		obstacle.Triangles = triangles.ToArray();
		obstacle.Vertices = vertices.ToArray();
	}


    private static int[] MakeQuad(int a, int b, int c, int d)
    {
        int[] ret =
        {
            a, c, d,
            a, b, c,
            a, d, c,
            a, c, b
        };
        return ret;
    }

	private static int[] MakeTri(int a, int b, int c)
	{
		int[] ret =
		{
			a, b, c,
			a, c, b
		};
		return ret;
	}
}
