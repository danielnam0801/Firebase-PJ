using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseEvent : MonoBehaviour
{
    public GameObject particleSnow;
    public GameObject particleRain;

    // Start is called before the first frame update
    void Start()
    {
        //���� �̺�Ʈ
        //string strWeather = AuthManager.Instance.GetWeather();
        //Console.WriteLine(strWeather);
        //switch (strWeather)
        //{
        //    case "Snow":
        //        particleSnow.SetActive(true);
        //        break;
        //    case "Rain":
        //        particleRain.SetActive(true);
        //        break;
        //}

        int developerCoin = AuthManager.Instance.GetDeveloperCoin();
        if (developerCoin == 0)
            return;

        UIManager.Instance.ADDMessage(developerCoin);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
