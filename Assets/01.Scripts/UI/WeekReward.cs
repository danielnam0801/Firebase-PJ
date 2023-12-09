using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeekReward
{
    GameObject Focus;
    List<Reward> Lists;
    TextMeshProUGUI focusTxt;

    int curDay;
    public bool CanUpdate;

    public WeekReward(List<Reward> Lists, GameObject Focus, TextMeshProUGUI focusTxt)
    {
        this.Lists = Lists;
        this.Focus = Focus;
        this.focusTxt = focusTxt;
        curDay = 0;
        CanUpdate = false;
    }
    
    public void Update()
    {
        if (!CanUpdate) return;
       
        if (Lists[curDay].IsClicked)
        {
            GetReward();
        }
    }

    private void GetReward()
    {
        CanUpdate = false;
        //Clear
        Lists[curDay].transform.Find("Clear").gameObject.SetActive(true);

        focusTxt.text = "tommorow reward";
        if (curDay + 1 <= Lists.Count - 1)
        {     
            Focus.transform.SetParent(Lists[curDay + 1].transform);
            Focus.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
        else
            Focus.SetActive(false);

    }

    public void Reward(int day)
    {
        curDay = day;
        CanUpdate = true;
        
        for(int i = 0; i < day; i++)
        {
            Lists[i].transform.Find("Clear").gameObject.SetActive(true);
            Lists[i].IsClicked = true;
        }
        // FocusÀÌµ¿

        Focus.transform.SetParent(Lists[curDay].transform);
        Focus.GetComponent<RectTransform>().localPosition = Vector3.zero;
        //focusTxt = Focus.transform.Find("Text_Message").GetComponent<TextMeshProUGUI>();
        focusTxt.text = "today reward";
    }
}
