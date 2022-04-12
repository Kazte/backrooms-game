 
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    protected int price;

    [SerializeField]
    private TextMeshProUGUI priceText;

    protected Button buyButton;

    protected virtual void Start()
    {
        priceText.text = $"${price}";

        buyButton = GetComponentInChildren<Button>();
        
        buyButton.onClick.AddListener(Buy);   
    }

    protected virtual void Buy()
    {
        
    }
}
