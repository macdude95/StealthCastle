using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour {

	public static PickUpController pickUp;
	private Rigidbody2D rb2d;
	private BoxCollider2D boxCol2d;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
		boxCol2d = GetComponent<BoxCollider2D>();
	}

	public void DropItem(Vector3 position, Vector2 direction) {
		boxCol2d.isTrigger = false;
		this.transform.position = position;
		rb2d.AddForce(direction * 20);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {
			gameObject.SetActive (false);
		} 
	}

	private void OnCollisionExit2D(Collision2D collision) {
		boxCol2d.isTrigger = true;
		rb2d.velocity = Vector2.zero;
	}
}
