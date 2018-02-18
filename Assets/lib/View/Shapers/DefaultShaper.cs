using System.Collections.Generic;
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
            SplinePoints = new List<Vector3>();
            SplinePoints.Add(Vector3.zero);
        }
        
        public override void UpdateSplinePoints(IShaper previous, int length)
        {
            //SplinePoints is never changed so we don't have to do anything here
        }
    }
}