using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapActionController : MonoBehaviour {

    public static GameObject instance;

	// Use this for initialization
	void Start () {
        instance = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateTrap()
    {
        this.transform.Rotate(new Vector3(0,0,45));
    }
}
