using System.Collections;
using System.Collections.Generic;
using Assets.lib;
using UnityEngine;
using MathNet.Numerics.Interpolation;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour
{
    private BezierCurve _curve;
    private BezierSpline _spline;

    private List<Vector3> points;

    [Range(-10,10)]
    public double t;
    
    // Use this for initialization
    void Start()
    {
        _curve = new BezierCurve();
        _spline = new BezierSpline();

        
    }
    
    // Update is called once per frame
    void Update ()
    {

        if (_curve == null) _curve = new BezierCurve();
        if(_spline == null) _spline = new BezierSpline();
        //_curve.Points = points;

        points = new List<Vector3>(10);
        for (int i = 0; i < 110; i++)
        {
            int sign = (i % 2) == 0?-1:1;
            points.Add(new Vector3(20 * sign, i * -sign, i* 10));
        }


        _spline.Points = points;
    }

    void OnDrawGizmos()
    {
        if (_curve == null) _curve = new BezierCurve();


        //for (int i = 0; i < points.Count - 1; i++)
        //{
        //    float f = (float)i / (float)points.Count;
        //    Gizmos.color = new Color(f, 1 - f, 1 - f);
        //    Gizmos.DrawLine(points[i], points[i + 1]);
        //}

        //for (int i = 0; i < _spline.Points.Count; i++)
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireCube(_spline.Points[i],Vector3.one);
        //}

        int resolution = 10 ;
        for (int i = 0; i < _spline.Points.Count* resolution - 1; i++)
        {
            float f = (float)i / ((float)points.Count* resolution);
            Gizmos.color = new Color(1 - f, f, 1 - f);
            var u1 = (float)i/ resolution;
            var val1 = _spline[u1];
            var u2 = (float)(i + 1)/ resolution;
            var val2 = _spline[u2];
            Gizmos.DrawLine(val1, val2);
            //drawString("" + u1 + "," + _spline.GetTForU(u1), val1,Color.blue);
            //float step = 0.1f;
            //for (float j = i+step; j <= i+1; j += step)
            //{
            //    Gizmos.color = Color.green;
            //    Vector3 normal = _spline.GetNormal(j).normalized;
            //    Gizmos.DrawLine(_spline[j - step], _spline[j - step] + normal);
            //}

        }

    }

    static public void drawString(string text, Vector3 worldPos, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();

        var restoreColor = GUI.color;

        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

        if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
        {
            GUI.color = restoreColor;
            UnityEditor.Handles.EndGUI();
            return;
        }

        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        GUI.color = restoreColor;
        UnityEditor.Handles.EndGUI();
    }
}
