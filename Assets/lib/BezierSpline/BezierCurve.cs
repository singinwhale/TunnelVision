

using System;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.lib
{
    public class BezierCurve
    {

        private List<Vector3> pointsList;

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

        public Vector3 GetTangent(float t)
        {
            return GetDerivative(t, 1);
        }

        public Vector3 GetNormal(float t)
        {
            return GetDerivative(t, 2);
        }

        public Vector3 GetDerivative(float t, int order)
        {
            // make sure t is in range [0,1]
            Assert.IsFalse(t < 0.0f || 1.0f < t);

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
        private static Matrix<double> GetCasteljauMatrix(int numPoints)
        {
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
    }
}