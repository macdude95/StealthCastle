using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Created by: Brian Egana
 * The script uses the animator of the player game object to switch runtime
 * animation controllers, making it so that the player appears to disguise
 * themselves with the gear that they find.
 */
public class EquipDisguise : MonoBehaviour {

	private GameObject gear;
	private Animator playerAnim;
	private AudioSource disguiseSound;
    private RuntimeAnimatorController updatedAnimator;


    private void Start() {
		playerAnim = GetComponent<Animator>();
		disguiseSound = GetComponent<AudioSource>();
	}

	public void PlayDisguiseSound() {
		disguiseSound.Play();
	}

    public void SetAnimControlToGuard()
    {
        playerAnim.runtimeAnimatorController = updatedAnimator;
    }

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("Disguise")) {
			gear = collision.gameObject;
            updatedAnimator = collision.GetComponent<DisguiseInformationContainer>().animator;
			playerAnim.SetBool("IS_CHANGING", true);
            collision.gameObject.SetActive(false);
		}
	}

}
