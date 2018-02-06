using UnityEngine;
using System.Collections;

public enum gameSounds{
	footstep
}

public class SoundController : MonoBehaviour {

	public AudioClip footStep;

	public static SoundController instance;

	// Use this for initialization
	void Start () {
		instance = this;

	}

	public static void PlaySound(gameSounds currentSound){
		switch(currentSound){
		case gameSounds.footstep:{
				instance.GetComponent<AudioSource>().PlayOneShot(instance.footStep);
				break;
			}
		}

	}
}