using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public Image itemDisplay;
	public Image scoreDisplay;
	public Text itemText;
	public Text pointText;

	public GameObject currItem;
	public static GameController instance;

	public int score;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
		//DontDestroyOnLoad(gameObject); breaks too much rn
	}

	// Use this for initialization
	void Start () {
		score = 0;
		itemText.text = "";
		pointText.text = score.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetPlayerItem(GameObject item) {
		itemDisplay.enabled = true;
		itemDisplay.sprite = item.GetComponent<SpriteRenderer>().sprite;
		currItem = item;

		itemText.text = currItem.GetComponent<PickUpController>().GetName();
	}

	public void DisplayScore() {
		pointText.text = score.ToString ();
	}

	public string GetItemName() {
		return currItem == null ? "none" : currItem.name;
	}

	public void ResetScene() {
		string sceneName = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(sceneName);
	}
}
