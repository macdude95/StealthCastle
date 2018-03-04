using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour {

	public string SceneName;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag ("Player")) {
			GameObject.Find ("Player").SetActive(false);	
			GameController.instance.LoadNewLevel (SceneName);
		}
	}
}
		