using UnityEngine;
using UnityEngine.U2D;

public class AR15Weapon : Weapon
{
    [SerializeField]
    private float spread = 0.01f;



    public override void Shoot()
    {
        if (currentRateOfFire > 0f || currentAmmo <= 0 || IsReloading) return;


        audioSource.PlayOneShot(shootSound);
        animator.SetTrigger("Shoot");
        audioSource.pitch = initAudioPitch + Random.Range(-pitchRange, pitchRange);
        muzzleParticle.Play();

        var range = spread;
        var ray = new Ray(camera.transform.position, camera.transform.forward + transform.right * Random.Range(-range / 2, range / 2) + transform.up * Random.Range(-range, range));


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
        recoil.RecoilFire();

        currentAmmo--;
        currentRateOfFire = rateOfFire;
        HudManager.Instance.SetAmmo(currentAmmo, maxAmmo);
    }
}