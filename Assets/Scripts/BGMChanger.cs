using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
