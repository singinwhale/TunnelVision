using System;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.SocialPlatforms;

namespace Assets.lib
{
    public class BezierSpline
    {
        private List<Vector3> _pointsList;

        private BezierCurve _curve;

        public const int CurveOrder = 4;


        public BezierSpline()
        {
            _curve = new BezierCurve();
        }

        public List<Vector3> Points
        {
            get
            {
                return _pointsList;
            }

            set
            {
                CalculateSharedPoints(value, out _pointsList);
            }
        }

        public float Length
        {
            get
            {
                return _pointsList.Count - 1 - float.Epsilon;
            }
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
            u = Mathf.Clamp(u, 0, Length);
            
            _curve.Points = _pointsList.GetRange(GetCurveFirstPointIndexForU(u), CurveOrder);
            
            return _curve[GetTForU(u)]; //[0,1] normalized value from u fraction
        }

        public Vector3 GetDerivative(float u, int order)
        {
            u = Mathf.Clamp(u, 0, Length);

            _curve.Points = _pointsList.GetRange(GetCurveFirstPointIndexForU(u), CurveOrder);

            return _curve.GetDerivative(GetTForU(u),order); //[0,1] normalized value from u fraction
        }




        public int GetCurveFirstPointIndexForU(float u)
        {
            int i = (int)u; 
            
            i = i - i % (CurveOrder - 1); //share last point
            if (i > _pointsList.Count - CurveOrder) return _pointsList.Count - CurveOrder;
            return i; 
        }
        
        private static void CalculateSharedPoints(List<Vector3> rawPoints, out List<Vector3> curvablePoints)
        {
            curvablePoints = new List<Vector3>(); //one more point needed per curve

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
        }

        public float GetTForU(float u)
        {
            int firstPointIndex = GetCurveFirstPointIndexForU(u);
            float t = (u - firstPointIndex) / (CurveOrder - 1);
            t = Mathf.Clamp(t, 0, 1);
            return t;
        }

        public Vector3 GetNormal(float u)
        {
            int firstPointIndex = GetCurveFirstPointIndexForU(u);
            _curve.Points = _pointsList.GetRange(firstPointIndex, CurveOrder);
            return _curve.GetNormal((u - firstPointIndex) / CurveOrder); //[0,1] normalized value from u fraction
        }

    }
}