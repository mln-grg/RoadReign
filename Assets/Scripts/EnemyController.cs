﻿using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Vehicles enemy;
    [SerializeField] private float knockbackforce;
    private Rigidbody rb;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private bool playerInAttackRange;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float attackRange;
        
    [SerializeField] private Rigidbody shell;
    [SerializeField] private Transform fireTransform;
    [SerializeField] private float LaunchForce = 15f;
    
    private ParticleSystem explosionParticles;
    private PlayerHealth ph;
    private EnemyDamage enemydamage;
    private float enemyHealth;
    public float Health { get { return enemyHealth; } }
    private float enemySpeed;
    private float enemyTurnSpeed;

    private NavMeshAgent agent;
    private Transform player;
    private bool isDead = false;
    public bool IsDead { get { return isDead; } }
    
    private bool alreadyAttacked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>().GetComponent<Transform>();
        ph = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
        agent = GetComponent<NavMeshAgent>();
        explosionParticles = Instantiate(enemy.explosionPrefab).GetComponent<ParticleSystem>();
        explosionParticles.gameObject.SetActive(false);
        enemydamage = GetComponent<EnemyDamage>();
    }
    private void Start()
    {
        enemyHealth = enemy.Health;
        enemySpeed = enemy.acceleration;
        enemyTurnSpeed = enemy.turnSpeed;
        
    }

    private void Update()
    {
   
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
       
        if (playerInAttackRange) 
            AttackPlayer();
    
        if (ph.IsDead)
        {
            StartCoroutine(GameOver());
        }
        if (!isDead && !ph.IsDead)
        {
            agent.SetDestination(player.position);
        }
            
    }
    private void ChasePlayer()
    {
        //agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        transform.LookAt(player);
        if (!alreadyAttacked)
        {
            StartCoroutine(Shoot());
        }
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        //KnockBack();
        enemydamage.SetHealthUI();
        if (enemyHealth <= 0)
        {
            StartCoroutine(OnDeath());
        }
    }

    public void KnockBack()
    {
        rb.AddForce(transform.forward * -1 * agent.speed * knockbackforce);
    }
    IEnumerator OnDeath()
    {
        enemydamage.SetHealthUI();
        isDead = true;
        Destroy(agent);
        explosionParticles.transform.position = transform.position;
        explosionParticles.gameObject.SetActive(true);        
        yield return new WaitForSeconds(8f);
        Destroy(gameObject);
    }
    IEnumerator GameOver()
    {
        Destroy(agent);
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    IEnumerator Shoot()
    {
        alreadyAttacked = true;
        Rigidbody shellInstance = Instantiate(shell, fireTransform.position, fireTransform.rotation) as Rigidbody;
        shellInstance.velocity = LaunchForce * fireTransform.forward;
        
        yield return new WaitForSeconds(timeBetweenAttacks);

        alreadyAttacked = false;
    }
    private void OnDrawGizmos()
    {
        //  Gizmos.DrawSphere(transform.position, attackRange);
    }
}
