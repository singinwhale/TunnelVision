using System;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using UnityEngine.Assertions;

namespace lib.View.BezierSpline
{
    public class BezierCurve
    {

        private List<Vector3> pointsList;

        /// <summary> Stores the Casteljau Matrizes for every curve Order. Filled lazyly. </summary>
        private Dictionary<int, Matrix<double>> _matrixCache = new Dictionary<int, Matrix<double>>();

        public List<Vector3> Points
        {
            get
            {
                return pointsList;
            }

            set
            {
                pointsList = value;
            }
        }

        public Vector3 this[float t]
        {
            get { return Evaluate(t); }
        }

        public Vector3 Evaluate(float t)
        {
            return GetDerivative(t, 0);
        }
		

        public Vector3 GetDerivative(float t, int order)
        {
            // make sure t is in range [0,1]
            Assert.IsFalse(t < 0.0f || 1.0f < t,"t is not in Range [0,1]: t = " + t);

            Matrix<double> casteljau = GetCasteljauMatrix(pointsList.Count);
            Matrix<double> points = GetPointsMatrix();

            //tVector = ((n-o)!*t^(n-o), (n-o-1)!*t^(n-o-2), ... 1);
            Vector<double> tVector = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(pointsList.Count);
            for (int i = 0; i < pointsList.Count; i++)
            {
                int exponent = pointsList.Count - i - 1;//count down - 1
                double derivationFactor;
                if (order == 0) // no derivation
                {
                    derivationFactor = 1;
                }
                else if (exponent <= order - 1) // constants and below are thrown away
                {
                    tVector[i] = 0;
                    continue;
                }
                else
                {
                    derivationFactor = SpecialFunctions.Factorial(exponent) / SpecialFunctions.Factorial(exponent-order);
                }
                tVector[i] =  derivationFactor * Mathf.Pow(t, exponent - order);
            }

            Vector<double> result = points * casteljau * tVector; 

            Vector3 ret;
            ret.x = (float)result[0];
            ret.y = (float)result[1];
            ret.z = (float)result[2];
            return ret;
        }
        

        /// <summary>
        /// Returns the Matrix representing the Casteljau Algorithm for solving Bernstein functions
        /// </summary>
        /// <param name="numPoints">Number of points in this bezier curve</param>
        /// <returns>Returns a sparse matrix of the order numPoints X numPoints</returns>
        private Matrix<double> GetCasteljauMatrix(int numPoints)
        {
            if (_matrixCache.ContainsKey(numPoints)) return _matrixCache[numPoints];
            Matrix<double> m = new MathNet.Numerics.LinearAlgebra.Double.SparseMatrix(numPoints, numPoints);

            int n = numPoints-1;
            for (int i = 0; i <= n; i++)
            {
                for (int k = 0; k <= n - i; k++)
                {
                    // m_ij = -1^(i+k+n)*(n-1 choose k)*(n choose i)
                    m[i, k] = Math.Pow(-1, i + k + n) * 
                        SpecialFunctions.Binomial(n - i, k) *
                        SpecialFunctions.Binomial(n, i);
                }
            }

            _matrixCache[numPoints] = m;
            return m;
        }

        private Matrix<double> GetPointsMatrix()
        {
            Matrix<double> m = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(3,pointsList.Count);
            for (int i = 0; i < pointsList.Count; i++)
            {
                m[0, i] = pointsList[i].x;
                m[1, i] = pointsList[i].y;
                m[2, i] = pointsList[i].z;
            }
            return m;
        }

	    public Vector3 GetNormal(float t)
	    {
			var tangent = GetDerivative(t, 1);
		    var curvature = GetDerivative(t, 2);

		    //detect edge cases for straight lines
		    if (Vector3.Cross(tangent, curvature) == Vector3.zero)
		    {
			    if (tangent.normalized == Vector3.up)
			    {
				    curvature = Vector3.right;
			    }
			    else
			    {
				    curvature = Vector3.up;
			    }
		    }
		    var cross = Vector3.Cross(tangent, curvature);
		    return Quaternion.AngleAxis(-90, tangent) * cross;
		}
    }
}