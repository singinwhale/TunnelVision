using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Interpolation;

public class LevelGenerator : MonoBehaviour
{
    private CubicSpline splineXY;
    private CubicSpline splineXZ;

    [Range(-10,10)]
    public double t;
    // Use this for initialization
    void Start()
    {
        double[] x = {1, 3, 0};
        double[] y = {0, 0, -3};
        double[] z = {0, 3, 0};
        splineXY = CubicSpline.InterpolateNatural(x, y);
        splineXZ = CubicSpline.InterpolateNatural(x, z);
        

        splineXZ.Interpolate(1.5);
        splineXY.Interpolate(1.5);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    Vector3 res;
	    res.x = (float)t;
	    res.y = (float)splineXZ.Interpolate(t);
	    res.z = (float)splineXZ.Interpolate(t);
		Debug.DrawLine(Vector3.zero, res);
	}
}
