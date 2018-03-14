using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace lib.View.Shapers
{
    /// <summary>
    /// Pseudo-Randomly shapes spline sections in a repeatable way.
    /// </summary>
    public class RandomShaper : Shaper
    {
        /// <summary>Space between generated spline points</summary>
        public float Spacing = 50;
        /// <summary>Maximum difference of angle between directions of point placement</summary>
        private float outerAngle = 30;
        
        /// <summary>State of the RNG when this object was constructed</summary>
        private Random.State _RNGState;
        
        public RandomShaper(int start, int length) : base(start, length)
        {
            SplinePoints = new List<Vector3>();
            _RNGState = Random.state;
            Random.Range(0, 1);//retrieve a random value so the state is changed for other calls.
        }

        /// <inheritdoc />
        public override void UpdateSplinePoints(IShaper previous, int length)
        {
            SplinePoints = new List<Vector3>();
            
            LastPoint = previous.LastPoint;
            LastDirection = previous.LastDirection;   
            

            //buffer the current state of the RNG so we can reset it later to prevent side effects
            var ranBuffer = Random.state;
            Random.state = _RNGState;
            for (int i = 0; i < Length; i++)
            {
                float val1 = Random.Range(-1,1) * (90 - outerAngle);
                float val2 = Random.Range(-1,1) * (90 - outerAngle);
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