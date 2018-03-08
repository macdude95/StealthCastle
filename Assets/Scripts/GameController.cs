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
    public AudioClip calmBgm, actionBGM;

	public GameObject currItem;
	public static GameController instance;

    private DoubleAudioSource audioSource;

	public int score;
	private int displayedScore;
    private bool isDead = false;
    private bool actionBGMOn = false;
    public int actionBGMTime;
    private int currentActionBGMTime = -1;

    private IList<IRespawnable> respawnableObjects;

	private void Awake() {
		if (instance == null) {
			instance = this;
            audioSource = this.GetComponent<DoubleAudioSource>();
            LevelMusicChanged();
        }
		else if (instance != this) {
			Destroy(gameObject);
		}
        //DontDestroyOnLoad(gameObject); breaks too much rn

        fadeInOutImage.gameObject.SetActive(true);

        respawnableObjects = InterfaceHelper.FindObjects<IRespawnable>();
        
	}

	// Use this for initialization
	void Start () {
		score = 0;
		displayedScore = 0;
		itemText.text = "";
		pointText.text = score.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown && isDead)
        {
            StartCoroutine(FadeAndCompletion(() => {
            RespawnObjects();
            fadeInOutAnimator.SetBool("FADE", false);
                isDead = false;
            }));
            audioSource.CrossFade(calmBgm, 100, 1);
            actionBGMOn = false;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.CrossFade(actionBGM, 100, 2);
        }

        int scoreDelta = score - displayedScore ;
		if (scoreDelta != 0) {
            if (scoreDelta > 1000 && scoreDelta != 0)
            {
                displayedScore += 500;
                scoreDelta -= 500;
            }
		    
            if (scoreDelta > 250 && scoreDelta != 0)
            {
                displayedScore += 100;
                scoreDelta -= 100;

            }
            if (scoreDelta > 30 && scoreDelta != 0)
            {
                displayedScore += 10;
                scoreDelta -= 10;
            }

            if (scoreDelta != 0)
            {
                displayedScore += 1;
                scoreDelta -= 1;
            }
                
        }
		pointText.text = displayedScore.ToString ();

        if(actionBGMOn)
        {
            currentActionBGMTime--;
            if(currentActionBGMTime <= 0)
            {
                audioSource.CrossFade(calmBgm, 100, 1);
                actionBGMOn = false;
            }
        }
	}

    public void LevelMusicChanged()
    {
        audioSource.CrossFade(calmBgm, 100, 0);
    }

    public void PlayActionMusic()
    {
        if(!actionBGMOn)
        {
            actionBGMOn = true;
            audioSource.CrossFade(actionBGM, 100, .2f);
        }
        currentActionBGMTime = actionBGMTime;
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
            SceneManager.LoadScene(sceneName);
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
        foreach (IRespawnable rc in respawnableObjects)
        {
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
