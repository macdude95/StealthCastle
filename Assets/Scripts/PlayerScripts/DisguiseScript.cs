using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Created by Brian Egana
 * The script uses the animator of the player game object to switch runtime
 * animation controllers, making it so that the player appears to disguise
 * themselves with the gear that they find.
 */
public class DisguiseScript : MonoBehaviour {

	private GameObject disguise;
	private Animator playerAnim;
	private AudioSource disguiseSound;

	private RuntimeAnimatorController originalAnimator;
	private RuntimeAnimatorController updatedAnimator;

    private void Start() {
		playerAnim = GetComponent<Animator>();
		disguiseSound = GetComponent<AudioSource>();

		originalAnimator = playerAnim.runtimeAnimatorController;
	}

	public void PlayDisguiseSound() {
		disguiseSound.Play();
	}

	public void SetAnimControlToOrig() {
		playerAnim.runtimeAnimatorController = originalAnimator;
	}

    public void SetAnimControlToGuard() {
        playerAnim.runtimeAnimatorController = updatedAnimator;
    }

    public void DonDisguise(GameObject item) {
        updatedAnimator = item.GetComponent<DisguiseInfoContainer>().animator;
        playerAnim.SetBool("IS_CHANGING", true);
    }
}
