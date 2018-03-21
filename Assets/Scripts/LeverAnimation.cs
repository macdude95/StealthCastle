using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Created by: Alex Hua
 * Lever class to manage the toggle of the lever.
 * Never used in our actual game since there
 * wasn't enough time but for future puzzles, etc.*/
public class LeverAnimation : MonoBehaviour {

    public static LeverAnimation instanceLever;

    private Animator lever_anim;
    private bool leverUsed = false;

	// Use this for initialization
	void Start () {
        lever_anim = GetComponent<Animator>();
        instanceLever = this;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (leverUsed == true)
        {
            lever_anim.SetTrigger("LeverClicked");
            lever_anim.ResetTrigger("LeverIdle");
        }
        else
        {
            lever_anim.SetTrigger("LeverIdle");
            lever_anim.ResetTrigger("LeverClicked");
        }
	}

    /*Function to change the animation status of lever*/
    public void ChangeLeverAnimation()
    {
        if (leverUsed == false)
            leverUsed = true;
        else
            leverUsed = false;
    }
}
