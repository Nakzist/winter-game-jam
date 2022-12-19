using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class BossController : MonoBehaviour
{
    [SerializeField] private int health = 30;
    [SerializeField] private Animator _animator;
    [SerializeField] private float playerDistance;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private Transform player;


    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float enemySpeed;

    private float playerToEnemy;
    private float lastAttack = 0;
    private float attackSpeed = 1;
    

    private bool canDash = true;
    private bool isDashing = false;
    
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    private Vector2 dashBoxPoint;
    private float nextAttack;
    private bool canAttack;

    private void Update()
    {
        canAttack = Time.time >= lastAttack + 1 / attackSpeed;
        if (isDashing)
        {
            return;
        }
        
        if (Mathf.Abs(player.position.x - transform.position.x) <= 3)
        {
            BossAttack();
        }

        if (Vector2.Distance(player.position, transform.position) <= 2 && canAttack && canDash)
        {
            lastAttack = Time.time;
            StartCoroutine(Dash());
        }
        if (Physics2D.OverlapCircle(transform.position, 5, playerMask))
        {
            
            // if (transform.position.x < player.position.x)
            // {
            //     GetComponent<Rigidbody2D>().velocity = new Vector2( enemySpeed, GetComponent<Rigidbody2D>().velocity.y);
            // }
            // else if (transform.position.x > player.position.x)
            // {
            //     GetComponent<Rigidbody2D>().velocity = new Vector2( -enemySpeed, GetComponent<Rigidbody2D>().velocity.y);
            // }
            
            LookAtPlayer();
        }
        // else
        // {
        //     GetComponent<Animator>().SetBool("isWalking", false);
        // }
        if (GetComponent<Transform>().position.y <= -10)
        {
            Destroy(gameObject);
        }

    }

    public void LookAtPlayer()
    {
        if (transform.position.x <= player.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x),transform.localScale.y,transform.localScale.z);
        }else if (transform.position.x > player.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),transform.localScale.y,transform.localScale.z);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("enemy took damage");
        health -= damage;
        //_animator.SetTrigger("Hurt");

        if (health <= 0)
        {
            Debug.Log("enemy died");
            Die();
        }
    }

    void Die()
    {
        SceneManager.LoadScene("WinScene");
        //_animator.SetBool("isDead", true);
    }

    private void BossAttack()
    {
        if (transform.localScale.x >= 0)
        {
            dashBoxPoint = new Vector2(transform.position.x + transform.localScale.x * dashSpeed * dashTime/2 * Time.deltaTime, transform.position.y);
        }
        else
        {
            dashBoxPoint = new Vector2(transform.position.x - transform.localScale.x * dashSpeed * dashTime/2 * Time.deltaTime, transform.position.y);
        }
        
        bool isInArea = Physics2D.OverlapBox(dashBoxPoint,new Vector2(transform.localScale.x * dashSpeed * dashTime * Time.deltaTime, transform.localScale.y * 5),0,playerMask);
        bool canAttack = Time.time >= nextAttack;
        if ( isInArea && canAttack)
        {
            nextAttack = Time.time + dashCooldown;
            player.GetComponent<PlayerController>().TakeDamage(attackDamage);
        }
        
    }
    
    private IEnumerator Dash()
    {
        _animator.SetBool("Attack", true);
        canDash = false;
        isDashing = true;
        BossAttack();
        Debug.Log("boss attack");
        float originalGravity = GetComponent<Rigidbody2D>().gravityScale;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = new Vector2(-transform.localScale.x * dashSpeed, 0f);
        yield return new WaitForSeconds(dashTime);
        GetComponent<Rigidbody2D>().gravityScale = originalGravity;
        isDashing = false;
        _animator.SetBool("Attack", false);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Delete()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
