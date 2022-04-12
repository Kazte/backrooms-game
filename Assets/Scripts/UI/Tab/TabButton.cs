using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISerializationCallbackReceiver
{
    [SerializeField]
    private string textButton;

    [SerializeField]
    public TabGroup TabGroup;

    [SerializeField]
    public GameObject Selector;


    public UnityEvent OnTabSelected;

    public UnityEvent OnTabDeselected;

    private TextMeshProUGUI text;

    private void Start()
    {
        Selector.SetActive(false);


        TabGroup = GetComponentInParent<TabGroup>();
        TabGroup.Subscribe(this);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        TabGroup.OnTabEnter(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TabGroup.OnTabExit(this);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        TabGroup.OnTabSelected(this);
    }
    public void OnBeforeSerialize()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = textButton;
        Selector = GetComponentInChildren<Image>(true).gameObject;
    }
    public void OnAfterDeserialize()
    {
    }

    public void Select()
    {
        OnTabSelected?.Invoke();
    }

    public void Deselected()
    {
        OnTabDeselected?.Invoke();
    }
}