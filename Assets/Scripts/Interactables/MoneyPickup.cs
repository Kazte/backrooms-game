using System;
using UnityEngine;

public class MoneyPickup : Pickup
{
    [SerializeField]
    private int money;

    protected override void OnPickUp(PlayerController playerController)
    {
        playerController.AddMoney(money);
        
        audioSource.PlayOneShot(pickupAudioClip);
        
        Destroy(gameObject);
    }
}