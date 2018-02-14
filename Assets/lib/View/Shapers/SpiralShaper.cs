using UnityEngine;

namespace lib.View.Shapers
{
	public class SpiralShaper : Shaper
	{
		public float SpiralingSpeed = 0.5f; // rotations per meter
		public float Radius = 50;

		public SpiralShaper(float start, float length) : base(start, length)
		{
		}

		public override void AddShapePointsToSpline(ref BezierSpline.BezierSpline spline, IShaper previous, float length)
		{
			var pointsList = spline.Points;
			float nRotations = SpiralingSpeed * length;
			var stepSize = 360 / 30;//30 degrees per step
			for (float i = 0; i < nRotations * stepSize; i+= stepSize)
			{
				var value = Mathf.Deg2Rad * i * SpiralingSpeed;
				float x = Mathf.Sin(value) * Radius;
				float y = (i <= 90 ? Mathf.Sin(value) : 1) * Mathf.Cos(value) * Radius;
				float z = (i/stepSize)/SpiralingSpeed;
				
				var spiralPoint = new Vector3(x,y,z);
				var rotation = Quaternion.FromToRotation(Vector3.forward, previous.LastDirection);
				Vector3 point = rotation * spiralPoint;
				pointsList.Add(point);
			}

			spline.Points = pointsList;
		}
	}
}