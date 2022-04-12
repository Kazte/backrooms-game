using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> TabButtons;

    public List<GameObject> ObjectsToSwap;

    public TabButton SelectedTab;

    public void Subscribe(TabButton button)
    {
        if (TabButtons == null)
        {
            TabButtons = new List<TabButton>();
        }

        TabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (SelectedTab == null || button != SelectedTab)
        {
            button.Selector.SetActive(true);
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        if (SelectedTab != null)
        {
            SelectedTab.Deselected();
        }

        SelectedTab = button;

        SelectedTab.Select();

        ResetTabs();
        button.Selector.SetActive(true);
        var index = button.transform.GetSiblingIndex();
        for (int i = 0; i < ObjectsToSwap.Count; i++)
        {
            if (i == index)
            {
                ObjectsToSwap[i].SetActive(true);
            }
            else
            {
                ObjectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetAll()
    {
        if (SelectedTab != null)
        {
            SelectedTab.Deselected();
        }

        SelectedTab = null;
        
        for (int i = 0; i < ObjectsToSwap.Count; i++)
        {
            ObjectsToSwap[i].SetActive(false);
        }

        ResetTabs();
    }
    public void ResetTabs()
    {
        foreach (var button in TabButtons)
        {
            if (SelectedTab != null && button == SelectedTab) continue;

            button.Selector.SetActive(false);
        }
    }
}