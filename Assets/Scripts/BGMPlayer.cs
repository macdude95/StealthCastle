using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Written by Daniel Anderson
/// </summary>
public class BGMPlayer : MonoBehaviour {

    public int actionBGMTime;
    private int currentActionBGMTime;

    public static BGMPlayer instance;
    private DoubleAudioSource player;
    private AudioClip calmBGM, actionBGM;

    private bool actionBGMOn = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (actionBGMOn)
        {
            currentActionBGMTime--;
            if (currentActionBGMTime <= 0)
            {
                player.CrossFade(calmBGM, 100, 0);
                actionBGMOn = false;
            }
        }
    }

    public void ResetTimer()
    {
        currentActionBGMTime = 0;
        actionBGMOn = false;
        player.CrossFade(calmBGM, 100, 0);
    }

    public void PlayActionMusic()
    {
        if (!actionBGMOn)
        {
            actionBGMOn = true;
            player.CrossFade(actionBGM, 100, 0);
        }
        currentActionBGMTime = actionBGMTime;
    }

    public void LevelMusicChanged()
    {
        player.CrossFade(calmBGM, 100, 0);
        actionBGMOn = false;
        currentActionBGMTime = 0;
    }

    public void LoadNewMusic(AudioClip calm, AudioClip action)
    {
        calmBGM = calm;
        actionBGM = action;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            player = this.GetComponent<DoubleAudioSource>();

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
}
