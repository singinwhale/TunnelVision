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

		Vector3 targetTangent = _level.Spline.GetDerivative(_progress, 1),
			targetNormal = _level.Spline.GetNormal(_progress);

		_sphericalInterpolator.TargetRotation = Quaternion.LookRotation(targetTangent, targetNormal);

		transform.position = position;
		transform.rotation = _sphericalInterpolator.Update(Time.deltaTime);
		//transform.rotation = Quaternion.LookRotation(targetTangent, targetNormal);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		//Gizmos.DrawLine(transform.position, _level.Spline[_progress]);
	}
}
