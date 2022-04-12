 
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private string interactText;
    
    public string InteractText => interactText;

    public virtual void OnInteract(PlayerController playerController)
    {
        
    }

    
}
