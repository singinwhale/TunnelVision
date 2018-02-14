using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace lib.View.BezierSpline
{
    public class BezierSpline
    {
        private List<Vector3> _pointsList = new List<Vector3>();
        private List<Vector3> _givenPoints;

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
                return _pointsList.Count - 1 /*- float.Epsilon*/;
            }
        }

        public ReadOnlyCollection<Vector3> RawPoints
        {
            get { return _pointsList.AsReadOnly(); }
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
            u = Mathf.Clamp(u, 0, Length - float.Epsilon);
            
            _curve.Points = _pointsList.GetRange(GetCurveFirstPointIndexForU(u), CurveOrder);
            
            return _curve[GetTForU(u)]; //[0,1] normalized value from u fraction
        }

        public Vector3 GetDerivative(float u, int order)
        {
            u = Mathf.Clamp(u, 0, Length - float.Epsilon);

            _curve.Points = _pointsList.GetRange(GetCurveFirstPointIndexForU(u), CurveOrder);

            return _curve.GetDerivative(GetTForU(u),order); //[0,1] normalized value from u fraction
        }

	    public Vector3 GetNormal(float u)
	    {

		    u = Mathf.Clamp(u, 0, Length - float.Epsilon);

		    _curve.Points = _pointsList.GetRange(GetCurveFirstPointIndexForU(u), CurveOrder);

			return _curve.GetNormal(GetTForU(u));
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


	    private int GetCurveFirstPointIndexForU(float u)
	    {
		    int i = (int)u;

		    i = i - i % (CurveOrder - 1); //share last point
		    if (i > _pointsList.Count - CurveOrder) return _pointsList.Count - CurveOrder;
		    return i;
	    }

		private float GetTForU(float u)
        {
            int firstPointIndex = GetCurveFirstPointIndexForU(u);
            float t = (u - firstPointIndex) / (CurveOrder - 1);
            t = Mathf.Clamp(t, 0, 1);
            return t;
        }

    }
}