 
using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    private float currentHealth;

    public event Action<DamageSource> OnDamage;
    public event Action OnHeal;
    public event Action<DamageSource> OnDeath;

    public bool CanBeHealed => currentHealth < maxHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(DamageSource damageSource)
    {
        var targetHealth = currentHealth - damageSource.Damage;
        
        if (targetHealth <= 0f)
        {
            currentHealth = 0f;
            OnDamage?.Invoke(damageSource);
            OnDeath?.Invoke(damageSource);
        }
        else
        {
            currentHealth = targetHealth;
            OnDamage?.Invoke(damageSource);
        }
    }

    public void Heal(float heal)
    {
        var targetHeal = currentHealth + heal;

        if (targetHeal > maxHealth)
        {
            currentHealth = maxHealth;
            OnHeal?.Invoke();
        }
        else
        {
            currentHealth = targetHeal;
            OnHeal?.Invoke();
        }
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
}

[Serializable]
public class DamageSource
{
    public float Damage;
    
    public Vector3 HitPoint;
    public Vector3 HitNormal;
}
