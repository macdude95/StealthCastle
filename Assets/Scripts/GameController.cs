using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Image itemDisplay;
	public Text itemText;

	public GameObject currItem;
	public static GameController instance;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		itemText.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetPlayerItem(GameObject item) {
		itemDisplay.enabled = true;
		itemDisplay.sprite = item.GetComponent<SpriteRenderer>().sprite;
		currItem = item;

		itemText.text = currItem.name;
	}

	public string getItemName() {
		return currItem == null ? "none" : currItem.name;
	}
}
