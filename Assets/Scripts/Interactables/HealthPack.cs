 
using UnityEngine;

public class HealthPack : Pickup
{

    [SerializeField]
    private int heal;

    
    protected override void OnPickUp(PlayerController playerController)
    {
        if (playerController.CanBeHealed)
        {
            playerController.Heal(heal);
            Destroy(gameObject);
        }
    }
}
