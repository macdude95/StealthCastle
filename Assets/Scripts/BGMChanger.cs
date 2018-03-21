using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Written by Daniel Anderson
/// </summary>
public class BGMChanger : MonoBehaviour {

    public AudioClip calmBGM, actionBGM;

    //load the new music files
    void Start() {
        if (calmBGM != null && actionBGM != null)
        {
            BGMPlayer.instance.LoadNewMusic(calmBGM, actionBGM);
            BGMPlayer.instance.LevelMusicChanged();
        }
	}
}
