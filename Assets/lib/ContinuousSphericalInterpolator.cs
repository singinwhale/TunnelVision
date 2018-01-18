using UnityEngine;

namespace Assets.lib
{
	public class ContinuousSphericalInterpolator
	{
		public Vector3 _angularVelocity = Vector3.zero;
		public float Torque = 0;
		private Matrix4x4 _inertiaTensor = new Matrix4x4();

		private Quaternion _rotation = Quaternion.LookRotation(Vector3.right,Vector3.up);
		public Quaternion Rotation
		{
			get { return _rotation; }
			set { _rotation = value; Debug.Log("Set Rotation to "+value);}
		}

		public Quaternion TargetRotation;

		public Quaternion Update(float deltaTime)
		{
			//split data into usable format
			Vector3 currentAxis;
			float currentAngle;
			Rotation.ToAngleAxis(out currentAngle, out currentAxis);

			Vector3 targetAxis;
			float targetAngle;
			TargetRotation.ToAngleAxis(out targetAngle, out targetAxis);

			//calculate torque
			Vector3 torque = Vector3.Cross(currentAxis, targetAxis).normalized * Torque;
			
			//prevent overshoot
			// t = alpha/((omega dot a)*|omega|)
			float timeToTarget = Vector3.Angle(_angularVelocity,targetAxis)/(Vector3.Dot(_angularVelocity.normalized, targetAxis.normalized) * _angularVelocity.magnitude);
			//if (timeToTarget <= torque.magnitude * 0.9 * 0.5 * timeToTarget)
			//{
			//	torque = -torque;
			//}
			//apply torque to omega
			_angularVelocity += torque * deltaTime;
			var rotationDelta = Quaternion.AngleAxis(torque.magnitude * deltaTime, torque.normalized);
			//apply omega to rotation
			Rotation = rotationDelta * Rotation;
			

			//Rotation = Quaternion.Slerp(Rotation, TargetRotation, 0.1f);

			
			return Rotation;
		}
		
	}
}