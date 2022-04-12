using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Health health;

    [SerializeField]
    private GameObject hitParticle;

    [SerializeField]
    private GameObject hitCollider;

    [SerializeField]
    private AudioClip[] attackAudioClip;

    [SerializeField]
    private AudioClip[] deadAudioClip;

    [SerializeField]
    private Collider collider;

    [SerializeField]
    private GameObject moneyPrefab;

    private Animator animator;

    private AudioSource audioSource;

    private NavMeshAgent navMeshAgent;

    private Transform player;

    public event Action<Enemy> OnDeath;

    private bool isWalking;

    private bool isAttacking;

    private bool isDead;

    private bool canMove = true;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        health.OnDamage += Health_OnDamage;
        health.OnDeath += Health_OnDeath;

        HudManager.Instance.OnPause += Hud_OnPause;
    }
    private void Hud_OnPause(bool b)
    {
        if (b)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
    }

    public void Init(Vector3 position, Action<Enemy> killAction)
    {
        navMeshAgent.speed = Random.Range(4.5f, 5.5f);
        transform.position = position;
        health.Heal(999999);
        this.killAction = killAction;
        
        
        animator.SetBool("Dead", false);

        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.velocity = Vector3.zero;
        collider.enabled = true;


        canMove = true;
        isDead = false;
        isAttacking = false;
    }


    private void Start()
    {
        navMeshAgent.speed = Random.Range(4.5f, 5.5f);
    }

    private void Update()
    {
        if (!isDead)
        {
            var distance = Vector3.Distance(transform.position, player.position);

            if (distance >= 1.5f)
            {
                if (canMove)
                {
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(player.position);
                    isWalking = true;
                }
            }
            else
            {
                navMeshAgent.isStopped = true;
                isWalking = false;


                if (!isAttacking)
                {
                    var lookAt = player.position;
                    lookAt.y = 0f;
                    navMeshAgent.velocity = Vector3.zero;

                    transform.LookAt(lookAt);
                }
                StartAttack();
            }
        }

        animator.SetBool("Walking", isWalking);
        animator.SetBool("Attacking", isAttacking);
    }

    private void StartAttack()
    {
        canMove = false;
        isAttacking = true;
        animator.applyRootMotion = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
        canMove = true;
        SetHitCollider(0);
        animator.applyRootMotion = false;
    }

    // EVENTS
    private void Health_OnDamage(DamageSource damageSource)
    {
        Instantiate(hitParticle, damageSource.HitPoint, Quaternion.LookRotation(damageSource.HitNormal));
    }

    private void Health_OnDeath(DamageSource damageSource)
    {
        SetHitCollider(0);
        HudManager.Instance.AddScore(10);
        audioSource.PlayOneShot(deadAudioClip[Random.Range(0, deadAudioClip.Length)]);
        animator.SetBool("Dead", true);

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.enabled = false;
        collider.enabled = false;


        canMove = false;
        isDead = true;
        isAttacking = false;
        Instantiate(moneyPrefab, transform.position, quaternion.identity);


        OnDeath?.Invoke(this);
        Invoke("OnDead", 5f);
    }

    public event Action<Enemy> killAction;

    private void OnDead()
    {
        HudManager.Instance.OnPause -= Hud_OnPause;

        EndAttack();
        
        killAction?.Invoke(this);
    }

    public void SetHitCollider(int set) { hitCollider.SetActive(set == 1 ? true : false); }

    public void PlayAttackSound() => audioSource.PlayOneShot(attackAudioClip[Random.Range(0, attackAudioClip.Length)]);

    private void OnDrawGizmosSelected()
    {
        var corners = navMeshAgent.path.corners;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            if (i == 0)
            {
                Gizmos.DrawLine(transform.position, corners[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }
    }
}