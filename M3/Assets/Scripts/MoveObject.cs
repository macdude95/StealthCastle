using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {

	public float translationSpeed = 1.0f;

	public bool left = false;
	public bool right = false;
	public bool up = false;
	public bool down = false;

	public bool oscillate = false;
	public float oscillateTime = 0;

	public bool respawn = false;
	public float respawnTime = 0;
	private Vector3 spawnPoint;

	void Start() {
		if (oscillate) {
			InvokeRepeating("SwitchDirection", oscillateTime, oscillateTime);
		}
		if (respawn) {
			spawnPoint = transform.position;
			InvokeRepeating("Respawn", respawnTime, respawnTime);
		}
	}

	void Update() {
		if (left) {
			gameObject.transform.Translate(translationSpeed * Time.deltaTime, 0, 0);
		}
		if (right) {
			gameObject.transform.Translate(-translationSpeed * Time.deltaTime, 0, 0);
		}
		if (up) {
			gameObject.transform.Translate(0, -translationSpeed * Time.deltaTime, 0);
		}
		if (down) {
			gameObject.transform.Translate(0, translationSpeed * Time.deltaTime, 0);
		}
	}

	private void SwitchDirection() {
		left = !left;
		right = !right;
		up = !up;
		down = !down;
	}

	private void Respawn() {
		transform.position = spawnPoint;
	}
}
