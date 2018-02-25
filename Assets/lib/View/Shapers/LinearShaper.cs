using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace lib.View.Shapers
{
    public class LinearShaper : Shaper
    {
        public float Spacing = 50.0f;
        
        public LinearShaper(int start, int length) : base(start, length)
        {
        }

        public override void UpdateSplinePoints(IShaper previous, int length)
        {
            LastPoint = previous.LastPoint;
            SplinePoints = new List<Vector3>(length);
            for (int i = 1; i <= length; i++)
            {
                SplinePoints.Add(previous.LastPoint + previous.LastDirection.normalized * i * Spacing);
            }

            LastPoint = SplinePoints.Last();
            LastDirection = previous.LastDirection;
            LastNormal = previous.LastNormal;
        }
    }
}