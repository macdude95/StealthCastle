using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*Created By: Alex Hua
 * Class to tag onto a box collider to act as a finish trigger.
 * Loads next scene whenever the "Player" touches the boxcollider.*/
public class FinishLevel : MonoBehaviour {

	public string SceneName;    

    /*Function to change scenes to the wanted scene*/
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag ("Player")) {
			GameObject.Find("Player").SetActive(false);
			GameController.instance.LoadNewLevel (SceneName);
		}
	}
}
		