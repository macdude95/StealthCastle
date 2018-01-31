using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour {

    Vector3 target;
    public float speed = 0.01f;
    Vector2 velocity;

    public void Start()
    {
        target = this.transform.position;
    }

    public void Update()
    {
        if(this.transform.position != target)
        {
           this.transform.position = Vector2.MoveTowards(this.transform.position, target, speed);         
        }
    }

    public void MoveToSeenPosition(Vector3 coord)
    {
        target = coord;
        var dir = target - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
    }

}
