using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Pickup : MonoBehaviour
{
    protected AudioSource audioSource;

    [SerializeField]
    protected AudioClip pickupAudioClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void OnPickUp(PlayerController playerController)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickUp(other.GetComponent<PlayerController>());
        }
    }
}
