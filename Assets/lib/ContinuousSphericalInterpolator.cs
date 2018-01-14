using UnityEngine;

namespace Assets.lib
{
	public class ContinuousSphericalInterpolator
	{
		private Vector3 _angularVelocity = Vector3.zero;
		public float Torque = 0;


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

			//calculate target roation axis
			Vector3 targetRotationAxis = Vector3.Cross(currentAxis, targetAxis);

			//accelerate towards target rotation
			_angularVelocity += (targetRotationAxis -_angularVelocity.normalized) * Torque * deltaTime;
			//detect overshoot
			//if()

			// do the rotating
			Vector3 rotationAxis = _angularVelocity.normalized;
			float rotationSpeed = _angularVelocity.magnitude;
			Rotation = Quaternion.AngleAxis(rotationSpeed * deltaTime, rotationAxis) * Rotation;

			_angularVelocity = rotationAxis.normalized * rotationSpeed;

			return Rotation;
		}
	}
}