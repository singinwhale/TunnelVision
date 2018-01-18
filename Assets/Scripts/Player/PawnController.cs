using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnController : MonoBehaviour
{

	public float Speed;

	public float Range;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Horizontal") != 0.0f)
		{
			transform.Translate(Speed*Input.GetAxis("Horizontal")*Time.deltaTime,0,0,Space.Self);
		}
		if (Input.GetAxis("Vertical") != 0.0f)
		{
			transform.Translate(0, Speed * Input.GetAxis("Vertical") * Time.deltaTime, 0, Space.Self);
		}
		var clamped = Vector3.ClampMagnitude(new Vector3(transform.localPosition.x, transform.localPosition.y,0), Range);
		clamped.z = transform.localPosition.z;
		transform.localPosition = clamped;

	}

	void OnDrawGizmos()
	{
		var vec = new Vector3(0,0, transform.localPosition.z);
		
		Gizmos.DrawWireSphere(GetComponentsInParent<Transform>()[1].position + (Vector3)(transform.localToWorldMatrix * vec), Range);
	}
}
