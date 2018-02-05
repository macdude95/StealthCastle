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

	private void Start() {
		playerAnim = GetComponent<Animator>();
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("Disguise") &&
			Input.GetButtonDown("Submit")) {
			gear = collision.gameObject;

			SpriteRenderer gearRenderer =
				collision.gameObject.GetComponent<SpriteRenderer>();
			gearRenderer.sprite = null;

			string animControl = GetAnimControlName();
			playerAnim.runtimeAnimatorController =
				Resources.Load<RuntimeAnimatorController>(animControl);
		}
	}

	private string GetAnimControlName() {
		/*
		 * The name of all objects that contain a "disguise" are named
		 * "Gear*****". Therefore, to get the "*****", the start index of the
		 * sbustring must be 4 letters ahead.
		 */
		string gearSubstring = gear.name.Substring(4);

		/*
		 * All animator controllers of each guard and other enemy types
		 * wearing gear usable disguises have the name "*****Animator"
		 */
		return gearSubstring + "Animator";
	}
}
