using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomButton : Selectable, IPointerClickHandler, ISubmitHandler, ISerializationCallbackReceiver
{
    [SerializeField]
    private string buttonText;

    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private Button.ButtonClickedEvent onClick = new Button.ButtonClickedEvent();

    [SerializeField]
    private GameObject selector;



    private TextMeshProUGUI buttonTextMeshProUGUI;

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        onClick.Invoke();
    }
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        selector.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        selector.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Press();
    }
    public void OnSubmit(BaseEventData eventData)
    {
        Debug.Log(eventData);
    }
    public void OnBeforeSerialize()
    {
        buttonTextMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        buttonTextMeshProUGUI.text = buttonText;
    }
    public void OnAfterDeserialize()
    {
    }

    #if UNITY_EDITOR
    [MenuItem("GameObject/UI/Custom Button")]
    public static void AddCustomButton()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Custom Button"), Selection.activeGameObject.transform, false);
        obj.gameObject.name = "Custom Button";
    }
    #endif
}