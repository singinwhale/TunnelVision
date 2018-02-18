using System;
using System.Collections.Generic;
using System.Diagnostics;
using lib.Data.Config;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace lib.View.Shapers
{
    public abstract class Shaper : IShaper
    {
        public int Start { get; set; }
        public int Length { get; set; }
	    
	    public List<Vector3> SplinePoints { get; protected set; }
        public Vector3 LastPoint { get; protected set; }
        public Vector3 LastDirection { get; protected set; }
        public Vector3 LastNormal { get; protected set; }

	    protected Shaper(int start, int length)
	    {
		    Start = start;
		    Length = length;
	    }
        
        public abstract void UpdateSplinePoints(IShaper previous, int length);

	    public Mesh GetMesh(BezierSpline.BezierSpline spline, IShaper previous, int offset, int length)
	    {
		    Debug.Assert(offset+length <= spline.Length,"Trying to generate mesh that is longer than the spline it is based on! "+(offset+length)+" > "+spline.Length);
	        
	        var mesh = new Mesh();
	        
            List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Vector2> UVs = new List<Vector2>();

			var meshResolutionParallel = Config.Instance.Global.Level.Mesh.Resolution.Parallel;
			var meshResolutionNormal = Config.Instance.Global.Level.Mesh.Resolution.Normal;
			var radius = Config.Instance.Global.Level.Mesh.Radius;
			//we have to save our old normal so we can use that as a second vector for our normal calculation
			Vector3 oldNormal = previous.LastNormal;
			for (int i = 0; i < length; i++)
			{
				for (int x = 0; x <= meshResolutionParallel; x++)
				{
					float u = offset + i + ((float) x / meshResolutionParallel);
					Vector3 center = spline.Evaluate(u);
					Vector3 tangent = spline.GetDerivative(u, 1);
					Vector3 normal = spline.GetNormal(u).normalized;

					//for the first ring we have to use the last shapers values
					if (i == 0 && x == 0)
					{
						oldNormal = previous.LastNormal;
						normal = previous.LastNormal;
						center = previous.LastPoint;
						tangent = previous.LastDirection;
					}
					else
					{
						// project normal onto tangent plane so the rotation stays somewhat within that plane
						Plane p = new Plane(tangent,Vector3.zero);

						// rotate toward target normal
						Vector3 newNormal = Vector3.RotateTowards(oldNormal, normal, Mathf.Deg2Rad * 10.0f, 0);
						//then rotate to plane
						newNormal = Vector3.RotateTowards(newNormal, p.ClosestPointOnPlane(oldNormal), Mathf.Deg2Rad* 10.0f, 0);
						
						// check if all vectors are planar. We don't want that
						if (newNormal != normal && Vector3.Cross(Vector3.Cross(tangent, oldNormal), Vector3.Cross(tangent, newNormal)) == Vector3.zero) 
						{
							newNormal = Quaternion.AngleAxis(10, tangent) * oldNormal; // rotate the normal out of the same plane as tangent and targetnormal
						}
						oldNormal = newNormal;
						normal = newNormal;
					}

					LastPoint = center;
					LastDirection = tangent;
					LastNormal = normal;

					Debug.Assert(Vector3.Cross(normal,tangent) != Vector3.zero, "Tangent and Normal are parallel. something went wrong when calculating the normal.");
					Debug.Assert(tangent != Vector3.zero, "Tangent is zero! Do you have a duplicate pont in the spline?");
					if (Vector3.Cross(normal, tangent) == Vector3.zero)
					{
						Vector3.Cross(normal, tangent);
					}
					
					//Generate tube segment. We need to overlap the first and the last vertices because we need UV 0 and 1 on the same spot
					for (int n = 0; n <= meshResolutionNormal; n++)
					{
						//rotate the normal arount the center
						Vector3 vert = center + Quaternion.AngleAxis(n * 360.0f / meshResolutionNormal, tangent) * normal.normalized * radius;
						vertices.Add(vert);
						float xUV = (float)(i * meshResolutionParallel + x) / (spline.Points.Count * meshResolutionParallel);
						float yUV = (float) n / meshResolutionNormal;
						UVs.Add(new Vector2(xUV, yUV));

					}
					//connect tube segment to last one
					if (i != 0 || x > 0)//do not do this for the very first segment
					{
						//calculate stride per segment
						int segmentFirstIndex = (i * meshResolutionParallel + x) * (meshResolutionNormal+1);
						int lastSegmentFirstIndex = segmentFirstIndex - (meshResolutionNormal + 1);

						for (int n = 1; n <= meshResolutionNormal+1; n++)
						{
							//generate double faced quad between tube segments
							//using modulo to connect last vertex to first
							triangles.AddRange(MakeQuad(
									lastSegmentFirstIndex+n% (meshResolutionNormal + 1),
									lastSegmentFirstIndex+(n-1)% (meshResolutionNormal + 1),
									segmentFirstIndex+(n-1)% (meshResolutionNormal + 1),
									segmentFirstIndex+n% (meshResolutionNormal + 1)
								)
							);
						}

					}
				}

			}

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = UVs.ToArray();

	        return mesh;
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