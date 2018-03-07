using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* GoodKingController.cs
 * Created by Michael Cantrell
 */

public class GoodKingController : MonoBehaviour {

    public AudioClip scream;

    private void DidDie() {
        IntroSceneController.instance.NextDialogue();
        GetComponent<AudioSource>().PlayOneShot(scream);
    }

    private void DidStartWalking()
    {
        IntroSceneController.instance.NextDialogue();
    }
}
