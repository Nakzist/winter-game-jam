using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int health = 30;
    [SerializeField] private Animator _animator;
    [SerializeField] private float playerDistance;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform player;


    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float enemySpeed;

    private float playerToEnemy;
    private float lastAttack = 0;
    private float attackSpeed = 1;
    

    private void Update()
    {
        
        if (playerToEnemy <= playerDistance && playerToEnemy >= playerDistance )
        {
            EnemyAttack();
            return;
        }
        

        if (Vector2.Distance(player.position, transform.position) <= 2)
        {
            if (Time.time > lastAttack + 1 / attackSpeed)
            {
                lastAttack = Time.time;
                _animator.SetTrigger("Attack");
                return;
            }
        }
        if (Physics2D.OverlapCircle(attackPoint.position, 5, playerMask))
        {
            // GetComponent<Animator>().SetBool("isWalking", true);
            // Vector2 target = new Vector2(player.position.x, transform.position.y);
            // Vector2 newPos = Vector2.MoveTowards(transform.position, target, enemySpeed * Time.deltaTime);
            // GetComponent<Rigidbody2D>().MovePosition(newPos);

            if (transform.position.x < player.position.x)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2( enemySpeed, GetComponent<Rigidbody2D>().velocity.y);
            }
            else if (transform.position.x > player.position.x)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2( -enemySpeed, GetComponent<Rigidbody2D>().velocity.y);
            }
            
            LookAtPlayer();
        }
        else
        {
            GetComponent<Animator>().SetBool("isWalking", false);
        }
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
        _animator.SetTrigger("Hurt");

        if (health <= 0)
        {
            Debug.Log("enemy died");
            Die();
        }
    }

    void Die()
    {
            _animator.SetBool("isDead", true);
    }

    private void EnemyAttack()
    {
        Collider2D[] playerCollider = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);

        foreach (Collider2D player in playerCollider)
        {
            Debug.Log("enemy gave damage");
            player.GetComponent<PlayerController>().TakeDamage(attackDamage);
        }
    }

    public void Delete()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
