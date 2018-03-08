using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* GoodKingController.cs
 * Created by Michael Cantrell
 */

public class GoodKingController : MonoBehaviour {

    public AudioClip scream;
    public Collider2D PlayerCollider;
    public Collider2D playScreamCollider;

	private void Update()
	{
        if (playScreamCollider.IsTouching(PlayerCollider))
        {
            GetComponent<AudioSource>().PlayOneShot(scream);
            playScreamCollider.gameObject.SetActive(false);
        }
	}
}
