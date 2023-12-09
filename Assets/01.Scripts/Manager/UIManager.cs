using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GameObject changePassword;
    public GameObject loginUIwithEmail;
    public GameObject registerUI;
    public GameObject playUI;
    public GameObject weekRewardPanel;
    public GameObject user;
    public GameObject FirebaseEvent;

    public void LoginPanelwithName()
    {
        changePassword.SetActive(true);
        loginUIwithEmail.SetActive(false);
        registerUI.SetActive(false);
    }

    public void LoginPanelwithEmail()
    {
        loginUIwithEmail.SetActive(true);
        changePassword.SetActive(false);
        registerUI.SetActive(false);
    }

    public void RegisterPanel()
    {
        changePassword.SetActive(false);
        loginUIwithEmail.SetActive(false);
        registerUI.SetActive(true);
    }

    public void CloseLogin()
    {
        changePassword.SetActive(false);
        loginUIwithEmail.SetActive(false);
        //user.SetActive(true);
        playUI.SetActive(true);
        RewardEvent();
    }

    public void RewardEvent()
    {
        weekRewardPanel.SetActive(true);
    }

    
    public void StartGame()
    {
        //FirebaseEvent?.SetActive(true);
    }
}
