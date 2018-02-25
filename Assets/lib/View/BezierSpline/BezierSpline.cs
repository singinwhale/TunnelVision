using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace lib.View.BezierSpline
{
    public class BezierSpline
    {
        private List<Vector3> _pointsList = new List<Vector3>();
        private List<Vector3> _givenPoints = new List<Vector3>();

        private readonly BezierCurve _curve = new BezierCurve();

        public const int CurveOrder = 4;


        public List<Vector3> Points
        {
            get
            {
                return _givenPoints;
            }

            set
            {
                _pointsList = CalculateSharedPoints(value);
                _givenPoints = value;
            }
        }


        public float Length
        {
            get
            {
                return _givenPoints.Count - 1 /*- float.Epsilon*/;
            }
        }

        public List<Vector3> RawPoints
        {
            get { return _pointsList; }
        }

        public Vector3 this[float u]
        {
            get
            {
                return Evaluate(u);
            }
        }

        public Vector3 Evaluate(float u)
        {
            u = ToInternalU(u);
            _curve.Points = _pointsList.GetRange(GetCurveFirstPointIndexForU(u), CurveOrder);
            
            return _curve[GetTForU(u)]; //[0,1] normalized value from u fraction
        }

        public Vector3 GetDerivative(float u, int order)
        {   
            u = ToInternalU(u);
            _curve.Points = _pointsList.GetRange(GetCurveFirstPointIndexForU(u), CurveOrder);

            return _curve.GetDerivative(GetTForU(u),order); //[0,1] normalized value from u fraction
        }
        
	    public Vector3 GetNormal(float u)
	    {
	        u = ToInternalU(u);
		    _curve.Points = _pointsList.GetRange(GetCurveFirstPointIndexForU(u), CurveOrder);

			return _curve.GetNormal(GetTForU(u));
	    }

        /// <summary>
        /// Tries to estimate the distance between two points on the spline along the spline.
        /// </summary>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="optimistic">Whether the estimate should be optimistic or not.
        /// Optimistic distance is guaranteed to be shorter than the actual distance.
        /// Pessimistic is guaranteed to be longer than the actual distance</param>
        /// <returns></returns>
        [Pure]
        public float EstimateDistanceOnSpline(float from, float to, bool optimistic = false)
        {
            float estimate = 0;
            // make sure from is less than to
            if (from > to)
            {
                float temp = to;
                to = from;
                from = temp;
            }
            if (Mathf.Abs(to - from) <= 1)
            {
                return (Evaluate(to) - Evaluate(from)).magnitude;
            }
            else if (!optimistic || to - from < CurveOrder)
            {   
                for (int i = (int)Mathf.Max(1,from + 1); i < Mathf.Min(RawPoints.Count,to); i++)
                {
                    estimate += (RawPoints[i] - RawPoints[i - 1]).magnitude;
                }
            }
            else
            {
                int stepsize = CurveOrder - 1;
                for (int i = (int) from + stepsize; i <= to; i++)
                {
                    estimate += (Evaluate(i) - Evaluate(i - stepsize)).magnitude;
                }
            }

            return estimate;
        }

        public BezierSpline Clone()
        {
            var newSpline = new BezierSpline();
            newSpline._pointsList = _pointsList;
            newSpline._givenPoints = _givenPoints;
            newSpline._curve.Points = _curve.Points;
            return newSpline;
        }
        
		private static List<Vector3> CalculateSharedPoints(List<Vector3> rawPoints)
        {
			List<Vector3>  curvablePoints = new List<Vector3>();

            //add first batch
            curvablePoints.Add(rawPoints[0]);
            
            for(int i = 1; i < rawPoints.Count - 1; i++)
            {
                curvablePoints.Add(rawPoints[i]);
                if (i % (CurveOrder - 2) == 0)
                {
                    //add shared point at half distance
                    curvablePoints.Add(rawPoints[i]+(rawPoints[i+1]- rawPoints[i])/ 2);
                }
            }

            //duplicate last point until a full curve is reached
            int nFillPoints = (CurveOrder - 1) - (curvablePoints.Count - 1) % (CurveOrder - 1); // a full path has (n-1)%(o-1) = 0
            for (int i = 0; i < nFillPoints; i++)
            {
                curvablePoints.Add(rawPoints[rawPoints.Count - 1]);
            }

	        return curvablePoints;
        }

        [Pure]
        private float ToInternalU(float u)
        {
            Mathf.Clamp(u, 0, Length - float.Epsilon);
            //convert from givenPoints u to rawPoints u;
            return u * ((float)_pointsList.Count / _givenPoints.Count);
        }

        [Pure]
	    private int GetCurveFirstPointIndexForU(float u)
	    {
		    int i = (int)u;

		    i = i - i % (CurveOrder - 1); //share last point
		    if (i > _pointsList.Count - CurveOrder) return _pointsList.Count - CurveOrder;
		    return i;
	    }

        [Pure]
		private float GetTForU(float u)
        {
            int firstPointIndex = GetCurveFirstPointIndexForU(u);
            float t = (u - firstPointIndex) / (CurveOrder - 1);
            t = Mathf.Clamp(t, 0, 1);
            return t;
        }

        
    }
}