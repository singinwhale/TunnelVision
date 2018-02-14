using UnityEngine;

namespace lib.View.Shapers
{
    public class DefaultShaper : Shaper
    {
        public DefaultShaper() : base(0,0)
        {
            LastPoint = Vector3.zero;
            LastDirection = Vector3.forward;
            LastNormal = Vector3.up;
        }
        
        public override void AddShapePointsToSpline(ref BezierSpline.BezierSpline spline, IShaper previous, float length)
        {
            spline.Points.Add(Vector3.zero);
        }
    }
}