using System;
using UnityEngine;

public class HealthShopItem : ShopItem
{
    private PlayerController playerController;

    protected override void Start()
    {
        base.Start();
        playerController = FindObjectOfType<PlayerController>();
    }

    protected override void Buy()
    {
        if (playerController.SubMoney(price) && playerController.CanBeHealed)
        {
            playerController.Heal(20);
        }
    }
}