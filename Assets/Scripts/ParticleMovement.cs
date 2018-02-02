using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMovement : MonoBehaviour {

	private float initializationTime;
	public float howLongUntilVanish = 2;
	public float speed = 5;

	void Start () {
		initializationTime = Time.timeSinceLevelLoad;
		GetComponent<Rigidbody2D> ().velocity *= speed;
	}
	
	void Update() {
		float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;
		if (timeSinceInitialization > this.howLongUntilVanish) {
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Enemy") {
			Destroy (this.gameObject);
		}

	}

}
