using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        animator.SetTrigger("ArrowHit");
        rb.velocity = Vector2.zero;
    }

    public void HitAnimComplete()
    {
        parent.StopAttacking();
        animator.ResetTrigger("ArrowHit");
        this.gameObject.SetActive(false);        
    }
}
