using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] RectTransform[] tabs;
    public void ShowTab(int tabIndex) 
    {
        //do nothing if in selected tab
        if (tabs[tabIndex] == CurrentTab) return;
        //hide previous tab
        HideTab();

        CurrentTab = tabs[tabIndex];

        //return if not a valid tab
        if(tabIndex > tabs.Length)
        {
            Debug.LogError("Not a valid buton");
            return;
        }
        CurrentTab = tabs[tabIndex];
        tabs[tabIndex].gameObject.SetActive(true);
    }
    void ShowTab(RectTransform tab) 
    {
        for(int i = 0; i < tabs.Length; i++)
        {
            if(CurrentTab == tabs[i])
            {
                return;
            }
            else if(tab == tabs[i])
            {
                HideTab();

                CurrentTab = tabs[i];
                tabs[i].gameObject.SetActive(true);
            }
        }
    }
    void HideTab()
    {
        if (CurrentTab != null)
        CurrentTab.gameObject.SetActive(false);
    }
    public RectTransform CurrentTab { get; private set; }
    /*
    Returns the current tab being shown, if any
    If no tab is being shown, return null
*/
    RectTransform[] Tabs { get { return tabs; } }
    /*
    Returns an array of RectTransform objects, each representing a tab
    This should always return a valid array (which may have 0 elements)
*/
}
