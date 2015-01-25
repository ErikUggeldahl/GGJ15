using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

	// Update is called once per frame
	void Update () 
	{
		transform.LookAt(Camera.main.transform.position);
	}
}
