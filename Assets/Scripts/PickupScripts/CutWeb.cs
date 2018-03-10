using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutWeb : MonoBehaviour, IRespawnable {

	/* 
	 * Created by Mitchell Keller
	 * If the player is holding the gadget to cut webs,
	 * then remove it from the scene.
	 */

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {
			if (GameController.instance.GetItemName().Equals("WebCutter")) {
				gameObject.SetActive (false);
			}
		} 
	}

	public void Respawn() {
		gameObject.SetActive (true);	
	}
}
