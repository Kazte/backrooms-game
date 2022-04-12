using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private Weapon currentWeapon;

    [SerializeField]
    private List<Weapon> weaponsList;

    private bool changinWeapon;

    private int currentWeaponIndex;

    private InputManager inputManager;
    
    private PlayerController playerController;

    [SerializeField]
    private Transform weaponSocket;

    private void Awake()
    {
        foreach (var weapon in weaponsList)
        {
            weapon.gameObject.SetActive(false);
        }
        
        playerController = GetComponentInParent<PlayerController>();

        inputManager = InputManager.Instance;

        inputManager.OnShoot += type =>
        {
            if (HudManager.Instance.IsPause) return;
            if (playerController.Freeze) return;

            switch (currentWeapon.weaponType)
            {
                case WeaponType.Semiautomatic:
                {
                    if (type == KeyInputType.Down)
                    {
                        // Debug.Log("Shooting with: " + currentWeapon.name);
                        Shoot();
                    }
                    break;
                }
                case WeaponType.Automatic:
                {
                    if (type == KeyInputType.Pressed)
                    {
                        // Debug.Log("Shooting with: " + currentWeapon.name);
                        Shoot();
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        };

        inputManager.OnReload += b =>
        {
            if (playerController.Freeze) return;
            if (HudManager.Instance.IsPause) return;
            
            Reload();
        };

        inputManager.OnWeaponSwitch += switchDirection =>
        {
            if (playerController.Freeze) return;
            if (HudManager.Instance.IsPause) return;
            
            if (!currentWeapon.IsReloading)
            {
                if (switchDirection > 0f)
                {
                    if (!changinWeapon)
                    {
                        currentWeaponIndex++;
                        if (currentWeaponIndex >= weaponsList.Count)
                        {
                            currentWeaponIndex = 0;
                        }
                    }

                    SwitchToWeapon(weaponsList[currentWeaponIndex]);
                    changinWeapon = true;
                }
                else if (switchDirection < 0f)
                {
                    if (!changinWeapon)
                    {
                        currentWeaponIndex--;
                        if (currentWeaponIndex < 0)
                        {
                            currentWeaponIndex = weaponsList.Count - 1;
                        }
                    }

                    SwitchToWeapon(weaponsList[currentWeaponIndex]);
                    changinWeapon = true;
                }
                else
                {
                    changinWeapon = false;
                }
            }
        };
    }

    private void Start()
    {
        SwitchToWeapon(weaponsList[0]);
    }

    private void Update()
    {
        
    }

    private void SwitchToWeapon(Weapon weapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }


        currentWeapon = weapon;
        currentWeapon.gameObject.SetActive(true);
        currentWeapon.Init();
    }

    private void Shoot()
    {
        currentWeapon.Shoot();
    }

    private void Reload()
    {
        currentWeapon.Reload();
    }
    public bool HasWeapon(Weapon weapon)
    {
        return weaponsList.Contains(weapon);
    }

    public Weapon AddWeapon(Weapon weaponToAdd)
    {
        var w = Instantiate(weaponToAdd, weaponSocket);
        
        SwitchToWeapon(w);
        
        weaponsList.Add(w);

        return w;
    }
}