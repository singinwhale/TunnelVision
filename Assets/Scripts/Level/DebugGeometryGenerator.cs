using System.Collections.Generic;
using Assets.lib.Data.Config;
using Assets.lib.View.BezierSpline;
using UnityEngine;

namespace Level
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	public class DebugGeometryGenerator : MonoBehaviour
	{
		public BezierSpline Spline { get; private set; }

		public float LevelLength = 1000;

		[Range(0,30)]
		public float t;

		//Debug settings
		public bool ShowNormals = false;

		public bool ShowTangents = false;

		public bool ShowCurve = false;

		public bool ShowPoints = false;

		public bool ShowLines = false;

		public DebugGeometryGenerator()
		{
			Spline = null;
		}


		// Use this for initialization
		void Start()
		{
			Spline = new BezierSpline();
			if (Spline == null) Spline = new BezierSpline();

			List<Vector3> points = new List<Vector3>();
			for (int i = 0; i <= LevelLength; i += 20)
			{
				points.Add(/*Quaternion.AngleAxis(i / (1000.0f / 360.0f), Vector3.up) **/ new Vector3(i, Mathf.Cos(i / 25.0F) * 20, Mathf.Sin(i / 25.0F) * 10));
			}
			for (int i = (int) LevelLength; i <= LevelLength*2; i += 20)
			{
				points.Add(new Vector3(i,0,0));
			}
			for (int i = (int)LevelLength*2; i <= LevelLength*3; i += 20)
			{
				points.Add(new Vector3(i, 0, Mathf.Cos(i / 25.0F) * 20));
			}
			for (int i = (int)LevelLength * 3; i <= LevelLength * 4; i += 20)
			{
				points.Add(new Vector3(i, Mathf.Cos(i / 25.0F) * 20,0));
			}
			Spline.Points = points;

			UpdateMesh();
			foreach (var obstacleController in GameObject.FindObjectsOfType<ObstacleController>())
			{
				if(UnityEngine.Application.isEditor) DestroyImmediate(obstacleController.gameObject);
				else Destroy(obstacleController.gameObject,0);
			}
			foreach (var textMesh in GameObject.FindObjectsOfType<TextMesh>())
			{
				if (UnityEngine.Application.isEditor) DestroyImmediate(textMesh.gameObject);
				else Destroy(textMesh.gameObject, 0);
			}
			GenerateObstacles();

			var x = Config.Instance.Processes;
		
		}



		void OnDrawGizmos()
		{
			if(ShowLines)
				for (int i = 0; i < Spline.Points.Count - 1; i++)
				{
					float f = (float)i / (float)Spline.Points.Count;
					Gizmos.color = new Color(f, 1 - f, 1 - f);
					Gizmos.DrawLine(Spline.Points[i], Spline.Points[i + 1]);
				}

			if(ShowPoints)
				for (int i = 0; i < Spline.Points.Count; i++)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawWireCube(Spline.Points[i], Vector3.one);
				}

			int resolution = 10 ;
        
			if(ShowNormals||ShowCurve || ShowTangents)
				for (int i = 0; i < Spline.Points.Count* resolution - 1; i++)
				{
					float f = (float)i / ((float)Spline.Points.Count * resolution);
					Gizmos.color = new Color(1 - f, f, 1 - f);
					var u1 = (float)i/ resolution;
					var val1 = Spline[u1];
					var t1 = Spline.GetDerivative(u1, 1);
					var u2 = (float)(i + 1)/ resolution;
					var val2 = Spline[u2];
					//var t2 = _spline.GetDerivative(u2, 1);

					//var n1 = Vector3.Cross(Vector3.Cross(t1, t2),t1)/100;;
					var n1 = Spline.GetNormal(u1)/30;
					if (ShowCurve)
						Gizmos.DrawLine(val1, val2);
					Gizmos.color = new Color(59.0f/255.0f,62.0f / 255.0f,193.0f / 255.0f);
					if(ShowNormals)
						Gizmos.DrawLine(val1,val1+n1);
					Gizmos.color = Color.yellow;
					if(ShowTangents)
						Gizmos.DrawLine(val1,val1+t1);
				}
        

			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(Spline[t],Vector3.one*0.1f);
		}

		public void UpdateMesh()
		{
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Vector2> UVs = new List<Vector2>();


			var MeshResolutionParallel = Config.Instance.Global.Level.Mesh.Resolution.Parallel;
			var MeshResolutionNormal = Config.Instance.Global.Level.Mesh.Resolution.Normal;
			var Radius = Config.Instance.Global.Level.Mesh.Radius;
			//we have to save our old normal so we can use that as a second vector for our normal calculation
			Vector3 oldNormal = Vector3.zero;
			for (int i = 0; i < Spline.Points.Count - 1; i++)
			{
				for (int x = 0; x < MeshResolutionParallel; x++)
				{
					float u = i + ((float) x / MeshResolutionParallel);
					Vector3 center = Spline.Evaluate(u);
					Vector3 tangent = Spline.GetDerivative(u, 1);
					//Vector3 normal = Vector3.Cross(Vector3.Cross(tangent,_spline.GetDerivative(u+1.0f/MeshResolutionParallel,1)),tangent);
					Vector3 normal = Spline.GetNormal(u).normalized;

					if (oldNormal == Vector3.zero) oldNormal = normal;
					else
					{
						Plane p = new Plane(tangent,Vector3.zero);

						Vector3 newNormal = Vector3.RotateTowards(oldNormal, normal, Mathf.Deg2Rad * 10.0f, 0);
						newNormal = Vector3.RotateTowards(newNormal, p.ClosestPointOnPlane(oldNormal), Mathf.Deg2Rad* 10.0f, 0);
						if (newNormal != normal && Vector3.Cross(Vector3.Cross(tangent, oldNormal), Vector3.Cross(tangent, newNormal)) == Vector3.zero) // check if all vectors are planar
						{
							newNormal = Quaternion.AngleAxis(10, tangent) * oldNormal; // just rotate because they oppose each other so it does not matter how we rotate
						}
						oldNormal = newNormal;
						normal = newNormal;
					}

					//Generate tube segment. We need to overlap the first and the last vertices because we need UV 0 and 1 on the same spot
					for (int n = 0; n <= MeshResolutionNormal; n++)
					{
						//rotate the normal arount the center
						Vector3 vert = center + Quaternion.AngleAxis(n * 360.0f / MeshResolutionNormal, tangent) * normal.normalized * Radius;
						vertices.Add(vert);
						float xUV = (float)(i * MeshResolutionParallel + x) / (float)(Spline.Points.Count * MeshResolutionParallel);
						float yUV = (float) n / MeshResolutionNormal;
						UVs.Add(new Vector2(xUV, yUV));

					}
					//connect tube segment to last one
					if (i != 0 || x > 0)//do not do this for the very first segment
					{
						//calculate stride per segment
						int segmentFirstIndex = (i * MeshResolutionParallel + x) * (MeshResolutionNormal+1);
						int lastSegmentFirstIndex = segmentFirstIndex - (MeshResolutionNormal + 1);

						for (int n = 1; n <= MeshResolutionNormal+1; n++)
						{
							//generate double faced quad between tube segments
							//using modulo to connect last vertex to first
							triangles.AddRange(MakeQuad(
									lastSegmentFirstIndex+n% (MeshResolutionNormal + 1),
									lastSegmentFirstIndex+(n-1)% (MeshResolutionNormal + 1),
									segmentFirstIndex+(n-1)% (MeshResolutionNormal + 1),
									segmentFirstIndex+n% (MeshResolutionNormal + 1)
								)
							);
						}

					}
				}

			}

			GetComponent<MeshFilter>().sharedMesh.vertices = vertices.ToArray();
			GetComponent<MeshFilter>().sharedMesh.triangles = triangles.ToArray();
			GetComponent<MeshFilter>().sharedMesh.uv = UVs.ToArray();
		}


		private void GenerateObstacles()
		{
			for (float u = 0; u < Spline.Length; u += Config.Instance.Global.Level.Obstacles.Density)
			{
				//set up the initializer 
				var initializer = new ObstacleController.ObstacleInitializer();
				initializer.Text = "Test 1234 holdriö";
				initializer.Color = new Color(1,85/255.0f,50/255.0f);
				initializer.Material = Resources.Load<Material>("Materials/ObstacleMaterial");
				//initializer.Material = this.GetComponent<MeshRenderer>().sharedMaterial;
				GenerateObstacleMesh(ref initializer,u,Random.value*360);
				initializer.Forward = Spline.GetDerivative(u, 1);
				Vector3 normal = Spline.GetNormal(u);//Vector3.Cross(_spline.GetDerivative(u, 2), _spline.GetDerivative(u, 1)));
				initializer.Up = Quaternion.AngleAxis(90,initializer.Forward) * normal;
				var obstacle = ObstacleController.Instantiate(initializer);
				obstacle.transform.position = Spline[u];
			}
		}

		private void GenerateObstacleMesh(ref ObstacleController.ObstacleInitializer obstacle, float u, float angle)
		{
			Vector3 tangent = Spline.GetDerivative(u, 1).normalized;

			Vector3 normal = Spline.GetNormal(u).normalized;

			//make only half circle obstacles for now
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
		
		
			for (float x = 0; x < 180; x += 180.0f / Config.Instance.Global.Level.Mesh.Resolution.Normal)
			{
				float currAngle = angle + x;
				var rotation = Quaternion.AngleAxis(currAngle, tangent);
				//create vertex on the half circle
				Vector3 vertex = rotation * (normal * Config.Instance.Global.Level.Mesh.Radius);
				vertices.Add(vertex);
				//add a second translated one so the obstacle has thickness. Assumes the spline is straight for that short bit
				vertex += tangent;
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
}
