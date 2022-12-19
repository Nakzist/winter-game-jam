using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private Animator _animator;

    [SerializeField] private Rigidbody2D playerRigidbody2D;
    [SerializeField] private Transform groundCheckerTransform;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyMask;

    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _jumpPower = 5;
    [SerializeField] private int attackDamage = 15;

    [SerializeField] private float attackSpeed = 1;

    private float horizontalInput;
    private bool jumpKeyWasPressed;

    private float lastAttack = 0;
    private int charge = 0;
    

    private bool canDash = true;
    private bool isDashing = false;
    
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

        
        
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        horizontalInput = Input.GetAxisRaw("Horizontal");

        
        
        if (Input.GetButtonDown("Fire1"))
        {
            if (Time.time >= lastAttack + 1 / attackSpeed)
            {
                lastAttack = Time.time;
                _animator.SetTrigger("Attack");
                Attack();
            }
        }

        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(horizontalInput * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            _animator.SetBool("isWalking", true);
        }
        else
        {
            _animator.SetBool("isWalking", false);

        }
        

        if (health <= 0)
        {
            Debug.Log("player died");
            Die();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Physics2D.OverlapCircleAll(groundCheckerTransform.position, 0.1f, groundMask).Length == 0)
        {
            return;
        }

        if (Input.GetButtonDown("Jump"))
        {
            _animator.SetTrigger("Jump");
            jumpKeyWasPressed = true;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = GetComponent<Rigidbody2D>().gravityScale;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        yield return new WaitForSeconds(dashTime);
        GetComponent<Rigidbody2D>().gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        
        
        playerRigidbody2D.velocity = new Vector2(horizontalInput * _moveSpeed, playerRigidbody2D.velocity.y);
        if (Physics2D.OverlapCircleAll(groundCheckerTransform.position, 0.1f, groundMask).Length == 0)
        {
            return;
        }

        if (jumpKeyWasPressed)
        {
            playerRigidbody2D.velocity += Vector2.up * _jumpPower;
            jumpKeyWasPressed = false;
        }
    }

    private void Attack()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyMask);

        foreach (Collider2D enemy in enemyColliders)
        {
            Debug.Log(enemy.gameObject.layer);
        }

        foreach (Collider2D enemy in enemyColliders)
        {
            if (enemy.gameObject.layer == 7)
            {
            enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
                
            }
            else
            {
            enemy.GetComponent<BossController>().TakeDamage(attackDamage);
                
            }

        }

    }

    public void TakeDamage(int damage)
    {
        Debug.Log("player took damage");
        health -= damage;
    }

    void Die()
    {
        //_animator.SetBool("isDead", true);
        enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<BoxCollider2D>().enabled = false;
        SceneManager.LoadScene("LoseScene");
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            Die();
        }
    }
}