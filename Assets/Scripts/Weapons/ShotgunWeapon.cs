using UnityEngine;

public class ShotgunWeapon : Weapon
{
    [SerializeField]
    private int bullets;


    [SerializeField]
    private AudioClip enterBulletAudioClip;

    [SerializeField]
    private AudioClip slideAudioClip;


    protected override void Start()
    {
        base.Start();

        damageSource.Damage /= bullets;
    }

    public override void Shoot()
    {
        if (currentRateOfFire > 0f || currentAmmo <= 0) return;

        if (IsReloading)
        {
            EndReload();
        }


        audioSource.PlayOneShot(shootSound);
        animator.SetTrigger("Shoot");
        audioSource.pitch = initAudioPitch + Random.Range(-pitchRange, pitchRange);
        muzzleParticle.Play();

        var range = 0.08f;

        for (int i = 0; i <= bullets; i++)
        {
            var ray = new Ray(camera.transform.position, camera.transform.forward + transform.right * Random.Range(-range, range) + transform.up * Random.Range(-range, range));

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
        }
        recoil.RecoilFire();

        currentAmmo--;
        currentRateOfFire = rateOfFire;
        HudManager.Instance.SetAmmo(currentAmmo, maxAmmo);
    }

    public override void Reload()
    {
        if (IsReloading || currentAmmo == maxAmmo) return;

        IsReloading = true;
        animator.SetBool("Reload", true);
    }

    public override void EndReload()
    {
        animator.SetBool("Reload", false);
        IsReloading = false;

        HudManager.Instance.SetAmmo(currentAmmo, maxAmmo);
    }

    public void PlayEnterBulletSound()
    {
        audioSource.PlayOneShot(enterBulletAudioClip);

        if (currentAmmo < maxAmmo)
        {
            currentAmmo++;
        }

        if (currentAmmo == maxAmmo)
        {
            EndReload();
        }

        HudManager.Instance.SetAmmo(currentAmmo, maxAmmo);
    }

    public void PlayerSlideSound()
    {
        audioSource.PlayOneShot(slideAudioClip);
    }
}