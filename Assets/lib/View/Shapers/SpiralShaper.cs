using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace lib.View.Shapers
{
	public class SpiralShaper : Shaper
	{
		public float SpiralingSpeed = 0.1f; // rotations per meter
		public float Radius = 10;

		public SpiralShaper(int start, int length) : base(start, length)
		{
		}

		/// <inheritdoc />
		public override void UpdateSplinePoints(IShaper previous, int length)
		{
			var stepSize = 30;//30 degrees per step
			var pointsList = new List<Vector3>();
			LastDirection = previous.LastDirection;
			for (int i = stepSize; i < length; i++)
			{
				var value = Mathf.Deg2Rad * i;
				float x = Mathf.Sin(value) * Radius;
				float y = Mathf.Cos(value) * Radius;
				float z = (i*stepSize)/SpiralingSpeed;
				
				
				var spiralPoint = new Vector3(x,y,z);
				var rotation = Quaternion.FromToRotation(Vector3.forward, previous.LastDirection);
				Vector3 point = previous.LastPoint + rotation * spiralPoint;
				pointsList.Add(point);
				LastPoint = point;
				if (pointsList.Count > 1)
				{
					LastDirection = (pointsList[pointsList.Count - 1] - pointsList[pointsList.Count - 2]).normalized;
				}

				LastNormal = previous.LastNormal;
			}

			SplinePoints = pointsList;
		}
	}
}