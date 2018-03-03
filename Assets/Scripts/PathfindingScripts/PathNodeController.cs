using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* PathNodeController.cs
 * Created by Daniel Anderson
 */

public class PathNodeController : MonoBehaviour {

    public GameObject nextNode;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        Debug.DrawLine(this.transform.position, nextNode.transform.position, Color.cyan);
    }

    public GameObject getNextNode() { return nextNode; }
}
