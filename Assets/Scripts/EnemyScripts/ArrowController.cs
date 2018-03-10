using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ArrowController.cs
 * Created by Daniel Anderson
 */

public class ArrowController : MonoBehaviour {

    private RangedEnemyController parent;
    private Animator animator;
    private Rigidbody2D rb;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
    }

    public void SetParent(RangedEnemyController controller)
    {
        parent = controller;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        HitTarget();
    }

    public void HitTarget()
    {
        parent.ArrowImpact();
        animator.SetTrigger("ArrowHit");
        rb.velocity = Vector2.zero;
    }

    public void HitAnimComplete()
    {
        parent.ArrowFinished();
        animator.ResetTrigger("ArrowHit");
        this.gameObject.SetActive(false);        
    }
}
