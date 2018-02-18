using lib.Data.Config;
using Level;
using UnityEngine;

namespace lib.System.Player
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private float _progress;

		[SerializeField] private float _speed;

		private float _progressSpeed = 1;

		public float SampleDistance = 1;
	
		public float Progress
		{
			get { return _progress; }
		}


		// Use this for initialization
		void Start ()
		{
			_progress = 1;
			_progressSpeed = Config.Instance.Global.Player.Camera.Speed;
			_speed = Config.Instance.Global.Player.Camera.Speed;
			
		}
	
		// Update is called once per frame
		void Update ()
		{
			//sample progress at normal speed and then resample with a adjusted value that should provide for a more linear speed
			float progressDelta = _progressSpeed * Time.deltaTime;
			Vector3 position = World.Instance.LevelController.Level.Spline[_progress+progressDelta];

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

				position = World.Instance.LevelController.Level.Spline[newSamplePoint];
				_progress = newSamplePoint;
			}
			else
			{
				//use an arbitrary progressSpeed to save the progression from stalling
				_progressSpeed = _speed;
				_progress += _progressSpeed * Time.deltaTime;
				position = World.Instance.LevelController.Level.Spline[_progress];
			}


			//sample the track before us so we can have smooth transitions in our rotation
			Vector3 targetTangent = Vector3.zero,
				targetNormal = Vector3.zero;

			int nSamples = 10;
			float sampleDistance = SampleDistance;
			for (int i = 0; i < nSamples; i++)
			{
				targetTangent += World.Instance.LevelController.Level.Spline.GetDerivative(_progress + (sampleDistance * i / nSamples), 1);
				targetNormal += World.Instance.LevelController.Level.Spline.GetDerivative(_progress + (sampleDistance * i / nSamples), 1);
			}
			targetTangent /= nSamples;
			targetNormal /= nSamples;

			var targetRotation = Quaternion.LookRotation(targetTangent, targetNormal);


			transform.position = position;
			transform.rotation = targetRotation;
		}
	
	}
}
