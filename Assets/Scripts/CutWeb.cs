using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutWeb : MonoBehaviour {

	// Use this for initialization
	void Start () { 

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player") {
			PlayerController player = collision.gameObject.GetComponent<PlayerController>();
			if (player.gadget01 == true) {
				gameObject.SetActive (false);
				Destroy (gameObject);
			}

		} 
	}
}
