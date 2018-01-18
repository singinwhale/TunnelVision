using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Assets.lib;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private LevelGenerator _level;

	[SerializeField] private float _progress;

	[SerializeField] private float _speed;

	//private FileStream f;


	// Use this for initialization
	void Start ()
	{
		_level = FindObjectOfType<LevelGenerator>();
		if(!_level) Debug.Log("LevelGenerator could not be found!");

		_progress = 1;

		Vector3 tangent = _level.Spline.GetDerivative(_progress, 1);
		Vector3 normal = _level.Spline.GetNormal(_progress);

		//f = File.Open("debug.log", FileMode.Append);
	}
	
	// Update is called once per frame
	void Update ()
	{
		_progress += _speed * Time.deltaTime;
		Vector3 position = _level.Spline[_progress];

		Vector3 targetTangent = Vector3.zero,
			targetNormal = Vector3.zero;

		int nSamples = 10;
		float sampleDistance = 2.0f;
		for (int i = 0; i < nSamples; i++)
		{
			targetTangent += _level.Spline.GetDerivative(_progress + (sampleDistance * i / nSamples), 1);
			targetNormal += _level.Spline.GetDerivative(_progress + (sampleDistance * i / nSamples), 1);
		}
		targetTangent /= nSamples;
		targetNormal /= nSamples;

		var targetRotation = Quaternion.LookRotation(targetTangent, targetNormal);

		var velocity = ((position - transform.position) / Time.deltaTime).magnitude;
		//var bytes = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, Encoding.ASCII.GetBytes((velocity+"\n").ToString()));
		//f.Write(bytes,0, bytes.Length);
		//Debug.Log(velocity);

		transform.position = position;
		transform.rotation = targetRotation;

		_speed = 1 + _progress / _level.Spline.Length * 3;
		GetComponentInChildren<Camera>().fieldOfView = 90 + 30 * _progress / _level.Spline.Length;
		
		//transform.rotation = Quaternion.LookRotation(targetTangent, Vector3.up);
	}
	
}
