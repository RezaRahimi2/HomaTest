using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;

namespace Reward
{
    public class RewardManager : Singleton<RewardManager>
    {
#if UNITY_EDITOR
        
        public List<RewardBase> RewardList
        {
            set => m_rewardList = value;
            get => m_rewardList;
        }
#endif
        
        [SerializeField] private List<RewardBase> m_rewardList;

        public void Initialize()
        {
            m_rewardList = GetComponentsInChildren<RewardBase>().ToList();
            EventManager.Subscribe<OnMissionCompleteEvent>(OnMissionComplete);
        }

        private void OnMissionComplete(OnMissionCompleteEvent onMissionCompleteEventData)
        {
            Debug.Log($"<color=pink>Reward {onMissionCompleteEventData.RewardData.RewardName}|{onMissionCompleteEventData.RewardData.Amount} Received</color>");
            
            m_rewardList.Find(x=>x.RewardName == onMissionCompleteEventData.RewardData.RewardName).GrantReward(onMissionCompleteEventData.RewardData);
            
            EventManager.Broadcast<OnGrantRewardEvent>(new OnGrantRewardEvent()
                { RewardData = onMissionCompleteEventData.RewardData });
        }
    }
}