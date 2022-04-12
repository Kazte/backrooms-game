using System;
using UnityEngine;

public class WeaponShopItem : ShopItem
{
    [SerializeField]
    private Weapon weapon;

    private WeaponController weaponController;

    private PlayerController playerController;

    protected override void Start()
    {
        base.Start();
        
        weaponController = FindObjectOfType<WeaponController>();
        playerController = FindObjectOfType<PlayerController>();
    }

    protected override void Buy()
    {
        if (playerController.SubMoney(price)){
            
            if (!weaponController.HasWeapon(weapon))
            {
                weaponController.AddWeapon(weapon);
                gameObject.SetActive(false);
            }
        }
    }
}