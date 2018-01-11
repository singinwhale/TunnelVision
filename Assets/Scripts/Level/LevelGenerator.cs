using System.Collections;
using System.Collections.Generic;
using Assets.lib;
using UnityEngine;
using MathNet.Numerics.Interpolation;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class LevelGenerator : MonoBehaviour
{
    private BezierSpline _spline = null;

    [Range(1,100)]
    public int MeshResolutionParallel = 3;
    [Range(3, 64)]
    public int MeshResolutionNormal = 16;

    public float Radius = 3;

    [Range(0,30)]
    public float t;

    //Debug settings
    public bool ShowNormals = false;

    public bool ShowCurve = false;

    public bool ShowPoints = false;

    public bool ShowLines = false;
    

    // Use this for initialization
    void Start()
    {
        _spline = new BezierSpline();
    }
    
    // Update is called once per frame
    void Update ()
    {
        if(_spline == null) _spline = new BezierSpline();

        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i <= 360; i += 20)
        {
            points.Add(new Vector3(i,Mathf.Cos(i/30)*10,Mathf.Sin(i/30)*10));
        }
        _spline.Points = points;
        UpdateMesh();
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
        
        if(ShowNormals||ShowCurve)
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
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawLine(val1,val1+t1);
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
}
