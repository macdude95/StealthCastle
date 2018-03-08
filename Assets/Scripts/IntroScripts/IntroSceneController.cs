using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* IntroSceneController.cs
 * Created by Michael Cantrell
 */

public class IntroSceneController : MonoBehaviour {

    public Animator goodKingAnimator;
    public static IntroSceneController instance;
    public GameObject lockCharacterObject;
    public Animator evilKingAnimator;
    public GameObject evilGuard1;
    public GameObject evilGuard2;
    public Collider2D PlayerCollider;
    public Collider2D moveEnemiesCollider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

	void Start () {
        // don't let the player leave until the cutscene is finished
        moveEnemiesCollider = GetComponent<BoxCollider2D>();
	}
	
	void Update () {
        if (moveEnemiesCollider.IsTouching(PlayerCollider)) {
            EnemiesLeave();
        }
	}

    private void EnemiesLeave() {
        evilKingAnimator.SetTrigger("MOVE");
        evilGuard1.GetComponent<Animator>().SetTrigger("MOVE");
        evilGuard2.GetComponent<Animator>().SetTrigger("MOVE");
        evilGuard1.GetComponent<Rigidbody2D>().velocity = new Vector2(0,-100);
        evilGuard2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -100);
    }
}
