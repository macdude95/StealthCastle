using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour {

    public string displayName;

	public static PickUpController pickUp;

	private Rigidbody2D rb2d;
	private BoxCollider2D boxCol2d;


	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
		boxCol2d = GetComponent<BoxCollider2D>();
	}

	public void DropItem(Vector3 position) {
		this.transform.position = position;
	}

	private void OnTriggerStay2D(Collider2D collision) {
        if (Input.GetButtonDown("PickUpItem") && collision.gameObject.tag == "Player") {
            gameObject.SetActive(false);
        }
	}

    public string getName() { return displayName; }
}
