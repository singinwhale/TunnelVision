using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Assets.lib;
using Level;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private DebugGeometryGenerator _geometryGenerator;

	[SerializeField] private float _progress;

	[SerializeField] private float _speed;

	private float _progressSpeed = 1;

	public float SampleDistance = 1;

	private FileStream f;


	// Use this for initialization
	void Start ()
	{
		_geometryGenerator = FindObjectOfType<DebugGeometryGenerator>();
		if(!_geometryGenerator) Debug.Log("GeometryGenerator could not be found!");

		_progress = 1;

		Vector3 tangent = _geometryGenerator.Spline.GetDerivative(_progress, 1);
		Vector3 normal = _geometryGenerator.Spline.GetNormal(_progress);

		//f = File.Open("debug.log", FileMode.Append);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//sample progress at normal speed and then resample with a adjusted value that should provide for a more linear speed
		float progressDelta = _progressSpeed * Time.deltaTime;
		Vector3 position = _geometryGenerator.Spline[_progress+progressDelta];

		//calculate the speed which we apparantly have in the world
		float actualSpeed = ((position - transform.position).magnitude / Time.deltaTime);
		//prevent division by zero
		if (Mathf.Abs(actualSpeed) > float.Epsilon)
		{
			//the deviation from our desired speed
			float error = _speed / actualSpeed;

			//adjust our progress speed
			_progressSpeed = progressDelta * error / Time.deltaTime;
			float newSamplePoint = _progress + progressDelta * error;

			position = _geometryGenerator.Spline[newSamplePoint];
			_progress = newSamplePoint;
		}
		else
		{
			//use an arbitrary progressSpeed to save the progression from stalling
			_progressSpeed = _speed;
			_progress += _progressSpeed * Time.deltaTime;
			position = _geometryGenerator.Spline[_progress];
		}


		//sample the track before us so we can have smooth transitions in our rotation
		Vector3 targetTangent = Vector3.zero,
			targetNormal = Vector3.zero;

		int nSamples = 10;
		float sampleDistance = SampleDistance;
		for (int i = 0; i < nSamples; i++)
		{
			targetTangent += _geometryGenerator.Spline.GetDerivative(_progress + (sampleDistance * i / nSamples), 1);
			targetNormal += _geometryGenerator.Spline.GetDerivative(_progress + (sampleDistance * i / nSamples), 1);
		}
		targetTangent /= nSamples;
		targetNormal /= nSamples;

		var targetRotation = Quaternion.LookRotation(targetTangent, targetNormal);

		var velocity = ((position - transform.position) / Time.deltaTime).magnitude;
		var bytes = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, Encoding.ASCII.GetBytes((velocity+"\n").ToString()));
		//f.Write(bytes,0, bytes.Length);

		transform.position = position;
		transform.rotation = targetRotation;
		
		
		//transform.rotation = Quaternion.LookRotation(targetTangent, Vector3.up);
	}
	
}
