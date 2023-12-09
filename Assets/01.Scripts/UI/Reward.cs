using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Reward : MonoBehaviour, IPointerDownHandler
{
    public bool IsClicked { get; set; }

    private bool _clearReward;
    public bool ClearReward => _clearReward;

    private void Start()
    {
        IsClicked = false;
        //_clearReward = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsClicked = true;
        //_clearReward = true;
    }
}
