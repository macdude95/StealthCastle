using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutWeb : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {
			PlayerController player = collision.gameObject.GetComponent<PlayerController>();
			if (GameController.instance.getItemName().Equals("WebCutter")) {
				gameObject.SetActive (false);
			}
		} 
	}
}
