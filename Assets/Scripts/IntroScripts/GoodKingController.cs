using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* GoodKingController.cs
 * Created by Michael Cantrell
 */

public class GoodKingController : MonoBehaviour {

    private void DidFinishWalking() {
        IntroSceneController.instance.NextDialogue();
    }

    private void DidStartWalking()
    {
        IntroSceneController.instance.NextDialogue();
    }
}
