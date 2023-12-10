using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopCat : MonoBehaviour
{
    [SerializeField]
    Image popCatImage;

    [SerializeField]
    Sprite popCat1Sprite;
    [SerializeField]
    Sprite popCat2Sprite;

    [SerializeField]
    TextMeshProUGUI cntText;

    int cnt = 0;

    private void Awake()
    {
        cntText.text = cnt.ToString();
        popCatImage.sprite = popCat1Sprite;
    }


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            popCatImage.sprite = popCat2Sprite; UpdateCount();
        }
        if(Input.GetMouseButtonUp(0))
        {
            popCatImage.sprite = popCat1Sprite;
        }
    }

    public void UpdateCount()
    {
        cnt++;
        cntText.text = cnt.ToString();
    }

    
}
