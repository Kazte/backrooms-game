 
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class Rocket : MonoBehaviour
{
    
    [SerializeField]
    private Rigidbody rigidbody;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip explotionAudioClip;

    [SerializeField]
    private float speed;

    [SerializeField]
    private GameObject rocketModel;

    [SerializeField]
    private float explotionDistance;

    [SerializeField]
    private GameObject explotionFx;

    private Weapon weaponParent;
    
    public void Init(Weapon myWeapon)
    {
        weaponParent = myWeapon;
    }
    private void FixedUpdate()
    {
        rigidbody.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        rocketModel.SetActive(false);
        Instantiate(explotionFx, transform);
        audioSource.PlayOneShot(explotionAudioClip);

        var colliders = Physics.OverlapSphere(transform.position, explotionDistance);

        if (colliders.Length > 0)
        {
            foreach (var col in colliders)
            {
                if (col.TryGetComponent(out Health health))
                {
                    health.Damage(new DamageSource
                    {
                        Damage = weaponParent.GetDamage()
                    });
                }
            }
        }
        
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explotionDistance);
    }
}
