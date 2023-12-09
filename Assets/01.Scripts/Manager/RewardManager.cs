using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardManager : Singleton<RewardManager>
{
    public GameObject Focus;
    public List<Reward> weekRewardList;
    public TextMeshProUGUI focusTxt;

    private WeekReward weekReward;

    private int seqRewardDay;
    private int rewardDay;
    private void Start()
    {
        seqRewardDay = 0;
        rewardDay = 0;
        weekReward = new WeekReward(weekRewardList, Focus, focusTxt);
    }

    private void Update()
    {
        weekReward.Update();
    }

    public void WeekReward()
    {
        if(AuthManager.Instance.SeqConnectDay - 1 < weekRewardList.Count)
            weekReward.Reward(AuthManager.Instance.SeqConnectDay - 1);
    }

    public void SetUpdateFalse()
    {
        weekReward.CanUpdate = false;
    }

   
}
