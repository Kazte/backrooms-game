using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum WeaponType
{
    Semiautomatic,
    Automatic
}

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    protected Camera camera;

    protected AudioSource audioSource;

    protected Animator animator;

    [SerializeField]
    protected string weaponName;

    [SerializeField]
    private RecoilData recoilData;

    [SerializeField]
    protected LayerMask layers;

    [SerializeField]
    protected int maxAmmo;

    protected int currentAmmo;

    [SerializeField]
    protected float rateOfFire;

    protected float currentRateOfFire;

    [SerializeField]
    protected AudioClip shootSound;

    [SerializeField]
    protected AudioClip reloadSound;

    [SerializeField]
    protected DamageSource damageSource;

    [SerializeField]
    protected Transform muzzleTransform;

    [SerializeField]
    protected ParticleSystem muzzleParticle;

    public bool IsReloading;

    public WeaponType weaponType;

    [SerializeField]
    protected float pitchRange = 0.1f;

    protected Recoil recoil;

    protected float initAudioPitch;

    protected virtual void Awake()
    {
        camera = Camera.main;

        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        recoil = GetComponentInParent<Recoil>();
    }

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;

        HudManager.Instance.SetAmmo(currentAmmo, maxAmmo);
        HudManager.Instance.SetWeaponName(weaponName);
    }

    public virtual void Init()
    {
        currentRateOfFire = 0f;
        IsReloading = false;

        HudManager.Instance.SetAmmo(currentAmmo, maxAmmo);
        HudManager.Instance.SetWeaponName(weaponName);

        recoil.SetRecoilData(recoilData);

        initAudioPitch = audioSource.pitch;
    }

    protected virtual void Update()
    {
        if (currentRateOfFire > 0f)
        {
            currentRateOfFire -= Time.deltaTime;
        }
        else
        {
            currentRateOfFire = 0f;
        }
    }

    public virtual void TryShoot()
    {
    }

    public virtual void Shoot()
    {
        if (currentRateOfFire > 0f || currentAmmo <= 0 || IsReloading) return;

        var ray = new Ray(camera.transform.position, camera.transform.forward);

        audioSource.PlayOneShot(shootSound);
        audioSource.pitch = initAudioPitch + Random.Range(-pitchRange, pitchRange);
        animator.SetTrigger("Shoot");
        muzzleParticle.Play();

        if (Physics.Raycast(ray, out var hit, layers))
        {
            var createHole = true;
            if (hit.collider.gameObject.TryGetComponent<Health>(out var health))
            {
                damageSource.HitPoint = hit.point;
                damageSource.HitNormal = hit.normal;

                health.Damage(damageSource);
                createHole = false;
            }

            var trail = Instantiate(bulletTrail, muzzleTransform.position, Quaternion.identity);

            StartCoroutine(SpawnTrail(trail, hit, createHole));
        }
        currentAmmo--;
        currentRateOfFire = rateOfFire;

        recoil.RecoilFire();

        HudManager.Instance.SetAmmo(currentAmmo, maxAmmo);
    }

    [SerializeField]
    protected TrailRenderer bulletTrail;

    [SerializeField]
    protected ParticleSystem bulletHoleParticle;


    protected IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit, bool createHole)
    {
        var time = 0f;
        var startPosition = trail.transform.position;

        while (time < 1f)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / bulletTrail.time;

            yield return null;
        }
        trail.transform.position = hit.point;

        // if (createHole) Instantiate(bulletHoleParticle, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }

    public virtual void Reload()
    {
        if (IsReloading || currentAmmo == maxAmmo) return;

        IsReloading = true;
        animator.SetBool("Reload", true);
        audioSource.PlayOneShot(reloadSound);
    }

    public virtual void EndReload()
    {
        currentAmmo = maxAmmo;
        animator.SetBool("Reload", false);
        IsReloading = false;

        HudManager.Instance.SetAmmo(currentAmmo, maxAmmo);
    }
    public float GetDamage() => damageSource.Damage;
}