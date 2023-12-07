using System;
using Events;
using UnityEngine;

namespace Mission
{
    public class CompleteSpecificNumberOfLevelsMission : MissionBase
    {
        public CompleteSpecificNumberOfLevelsMission(MissionData missionData,MissionManager missionManager): base(missionData,missionManager)
        {
            CollectedAmount = 0;
            TargetAmount = missionData.TargetAmount;
            EventManager.Subscribe<OnLevelWinEvent>(OnCompleteLevel);
        }
        
        private void OnCompleteLevel(OnLevelWinEvent onTileHitEvent)
        {
            CollectedAmount++;
            UpdateMission();
        }

        public override void StartMission()
        {
        }

        public override void Finish()
        {
            Debug.Log($"<color=green>Mission {MissionData.Name} Complete</color>");
            EventManager.Unsubscribe<OnLevelWinEvent>(OnCompleteLevel);
            m_missionManager.OnMissionComplete(MissionData.MissionID);}

        public override bool IsMissionComplete()
        {
            return CollectedAmount >= TargetAmount;
        }
    }
}