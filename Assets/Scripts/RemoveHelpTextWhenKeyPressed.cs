using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* RemoveHelpTextWhenKeyPressed.cs
 * Created by Michael Cantrell
 */

public class RemoveHelpTextWhenKeyPressed : MonoBehaviour {

    public KeyCode removeWhenKeyPressed;

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(removeWhenKeyPressed)) {
            gameObject.SetActive(false);
        }
	}
}
