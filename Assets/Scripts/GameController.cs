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
    public Text restartLevelText;
    public Image fadeInOutImage;
    public Animator fadeInOutAnimator;

	public GameObject currItem;
	private GameObject startItem;
	public static GameController instance;

    private bool isDead = false;
    private bool actionBGMOn = false;
    public int actionBGMTime;
    private int currentActionBGMTime = -1;

    private static IList<IRespawnable> respawnableObjects;

	private void Awake() {
		if (instance == null) {
			instance = this;
        }
		else if (instance != this) {
			Destroy(gameObject);
		}
        DontDestroyOnLoad(gameObject);

        fadeInOutImage.gameObject.SetActive(true);
        respawnableObjects = InterfaceHelper.FindObjects<IRespawnable>();
    }

	// Use this for initialization
	void Start () {
		itemText.text = "";
		pointText.text = ScoreScript.instance.score.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown && isDead) {
            StartCoroutine(FadeAndCompletion(() => {
				RespawnObjects();
				if (startItem != null) {
					startItem.SetActive(false);
					SetPlayerItem(startItem);
				}
				fadeInOutAnimator.SetBool("FADE", false);
                isDead = false;
            }));
            BGMPlayer.instance.ResetTimer();

        }
		int scoreDelta = ScoreScript.instance.score - ScoreScript.instance.displayedScore;
		if (scoreDelta != 0) {
            if (scoreDelta > 1000 && scoreDelta != 0) {
				ScoreScript.instance.displayedScore += 500;
                scoreDelta -= 500;
            }
            if (scoreDelta > 250 && scoreDelta != 0) {
				ScoreScript.instance.displayedScore += 100;
                scoreDelta -= 100;
            }
            if (scoreDelta > 30 && scoreDelta != 0) {
				ScoreScript.instance.displayedScore += 10;
                scoreDelta -= 10;
            }
            if (scoreDelta != 0) {
				ScoreScript.instance.displayedScore += 1;
                scoreDelta -= 1;
            }
        }
		pointText.text = ScoreScript.instance.displayedScore.ToString ();
	}

    public void PlayerDied() {
        restartLevelText.gameObject.SetActive(true);
        isDead = true;
        Input.ResetInputAxes();
    }

    /* LoadNewLevel
    * Created by Michael Cantrell
    * Takes in a string that is the name of the scene to load
    * then performs a fade before loading the new scene with SceneManager
    */
    public void LoadNewLevel(string sceneName) {
        StartCoroutine(FadeAndCompletion(() =>
        {
			if (startItem != currItem) {
				Destroy(startItem);
			}
			if (currItem != null) {
				DontDestroyOnLoad(currItem);
			}
            SceneManager.LoadScene(sceneName);

			startItem = currItem;
			fadeInOutAnimator.SetBool("FADE", false);
			respawnableObjects = InterfaceHelper.FindObjects<IRespawnable>();
		}));
    }

    /* FadeAndCompletion
    * Created by Michael Cantrell
    * A coroutine that fades the scene to black and then
    * performs a specified action after the fade is complete
    */
    private IEnumerator FadeAndCompletion(System.Action onFadeComplete) {
        fadeInOutAnimator.SetBool("FADE", true);
        yield return new WaitUntil(() => Mathf.Approximately(fadeInOutImage.color.a,1));
        onFadeComplete();
    }

    /* RespawnObjects
    * Created by Michael Cantrell
    * Takes all of the objects in the scene
    * of type "IRespawnable" and calls their "Respawn" methods
    * in order to reset the scene.
    */
    private void RespawnObjects() {
        SetPlayerItem(null);
        foreach (IRespawnable rc in respawnableObjects) {
            rc.Respawn();
        }
        restartLevelText.gameObject.SetActive(false);
    }

	public void SetPlayerItem(GameObject item) {
		currItem = item;
		if (currItem != null) {
			itemDisplay.enabled = true;
			itemDisplay.sprite = item.GetComponent<SpriteRenderer>().sprite;
			itemText.text = currItem.GetComponent<PickUpController>().GetName();
		}
		else {
			itemDisplay.enabled = false;
			itemText.text = "";
		}
	}

	/* DisplayScore
	 * Created by Mitchell Keller
	 * Updates the score for the player
	 */

	/*public void DisplayScore() {
		pointText.text = displayedScore.ToString ();
	}*/

	public string GetItemName() {
		return currItem == null ? "none" : currItem.name;
	}

}
