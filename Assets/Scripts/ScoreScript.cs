using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreScript : MonoBehaviour {
	public int score;
	public int displayedScore;

	public static ScoreScript instance;

	// Use this for initialization
	void Start () {
		score = 0;
		displayedScore = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Awake () {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad (this);
	}
}
