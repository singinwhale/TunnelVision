using System.Collections;
using System.Collections.Generic;
using Assets.lib;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private LevelGenerator _level;

	[SerializeField] private float _progress;

	[SerializeField] private float _speed;
	

	private ContinuousSphericalInterpolator _sphericalInterpolator = new ContinuousSphericalInterpolator();
	public float Torque = 10;

	// Use this for initialization
	void Start ()
	{
		_level = FindObjectOfType<LevelGenerator>();
		if(!_level) Debug.Log("LevelGenerator could not be found!");

		_progress = 1;

		Vector3 tangent = _level.Spline.GetDerivative(_progress, 1);
		Vector3 normal = _level.Spline.GetNormal(_progress);
		_sphericalInterpolator.Rotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
	{
		_sphericalInterpolator.Torque = Torque;
		_progress += _speed * Time.deltaTime;
		Vector3 position = _level.Spline[_progress];

		Vector3 targetTangent = _level.Spline.GetDerivative(_progress+2, 1),
			targetNormal = _level.Spline.GetNormal(_progress+2);

		var targetRotation = Quaternion.LookRotation(targetTangent, targetNormal);
		if (Quaternion.Angle(_sphericalInterpolator.Rotation, targetRotation) > 90)
		{
			_sphericalInterpolator.TargetRotation = Quaternion.LookRotation(targetTangent, targetNormal);
		}
		else
		{
			_sphericalInterpolator.TargetRotation = targetRotation;
		}

		transform.position = position;
		transform.rotation = _sphericalInterpolator.Update(Time.deltaTime);
		//transform.rotation = Quaternion.LookRotation(targetTangent, Vector3.up);
	}

	void OnDrawGizmos()
	{
		//Gizmos.DrawLine(transform.position, _level.Spline[_progress]);
		var baseLoc = transform.position+(Vector3) (transform.localToWorldMatrix * new Vector3(0, 0, 3));

		//draw current rotation
		Vector3 axis;
		float angle;
		_sphericalInterpolator.Rotation.ToAngleAxis(out angle, out axis);


		Gizmos.color = Color.red;
		Gizmos.DrawLine(baseLoc + axis*3, baseLoc - axis*3);
		Gizmos.color = Color.green;
		Gizmos.DrawRay(baseLoc+new Vector3(0,0.04f,0), _sphericalInterpolator.Rotation * Vector3.right*3);
		// draw target rotation
		_sphericalInterpolator.TargetRotation.ToAngleAxis(out angle, out axis);
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(baseLoc + axis * 3, baseLoc - axis * 3);
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(baseLoc, _sphericalInterpolator.Rotation * Vector3.right * 3);
		

		//draw angular velocity
		Gizmos.color =Color.cyan;
		Gizmos.DrawRay(baseLoc,_sphericalInterpolator._angularVelocity);
		

		
	}
}
