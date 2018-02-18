using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace lib.View.Shapers
{
    public class RandomShaper : Shaper
    {
        public float Spacing = 50;
        private float outerAngle = 30;
        private Random.State _RNGState;
        
        public RandomShaper(int start, int length) : base(start, length)
        {
            SplinePoints = new List<Vector3>();
            _RNGState = Random.state;
            Random.Range(0, 1);//retrieve a random value so the state is changed for consecutive accesses
        }

        public override void UpdateSplinePoints(IShaper previous, int length)
        {
            SplinePoints = new List<Vector3>();
            if(SplinePoints.Count ==0)
            {
                LastPoint = previous.LastPoint;
                LastDirection = previous.LastDirection;   
            }

            //buffer the current state of the RNG so we can reset it later
            var ranBuffer = Random.state;
            Random.state = _RNGState;
            for (int i = 0; i < Length; i++)
            {
                float val1 = Mathf.Cos(Random.Range(0,Mathf.PI)) * (90 - outerAngle);
                float val2 = Mathf.Cos(Random.Range(0,Mathf.PI)) * (90 - outerAngle);
                var rotation = Quaternion.Euler(val1,val2,0) * Quaternion.FromToRotation(Vector3.forward, LastDirection);
                var thePoint = LastPoint + rotation * Vector3.forward * Spacing;

                SplinePoints.Add(thePoint);
                LastPoint = thePoint;
                LastDirection = rotation * Vector3.forward;   
            }

            Random.state = ranBuffer;
        }
    }
}