using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

	public float rotationFrequency = 1.0f;
	public float secondsToRotate = 20.0f;
	public bool clockwise = true;

	private float rotationDirection = -1;

	public bool oscillate = false;
	public float oscillateTime = 0;

	void Start() {
		if (oscillate) {
			InvokeRepeating("SwitchDirection", oscillateTime, oscillateTime);
		}
	}

	void Update () {
		if (clockwise) {
			rotationDirection = -1;
		}
		else {
			rotationDirection = 1;
		}
		gameObject.transform.Rotate(0, 0, secondsToRotate * rotationFrequency
			* Time.deltaTime * rotationDirection);
	}

	private void SwitchDirection() {
		clockwise = !clockwise;
	}
}
