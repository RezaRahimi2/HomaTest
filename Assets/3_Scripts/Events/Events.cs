using System.Collections.Generic;
using Mission;
using UnityEngine.Serialization;

namespace Events
{
    public struct OnMissionDataLoadedEvent
    {
        public MissionsListData MissionListData;

        public OnMissionDataLoadedEvent(MissionsListData missionListData)
        {
            MissionListData = missionListData;
        }
    }
    
    public struct OnMissionUIInstantiatedEvent
    {
    }
    
    public struct OnSetNewActiveMissionsEvent
    {
        public List<MissionData> ActiveMissions;

        public OnSetNewActiveMissionsEvent(List<MissionData> activeMissions)
        {
            ActiveMissions = activeMissions;
        }
    }
    
    public struct OnMissionDataUpdateEvent
    {
        public int MissionID;
        public OnMissionDataUpdateEvent(int missionID)
        {
            MissionID = missionID;
        }
    }
    
    public struct OnTileHitEvent
    {
    }

    public struct OnGameLoadEvent
    {
        
    }
    
    public struct OnLevelStartEvent
    {
        
    }
    
    public struct OnLevelFinishedEvent
    {
    }

    public struct OnLevelWinEvent
    {
    }

    public struct OnLevelLoseEvent
    {
    }
    
    public struct OnMissionCompleteEvent
    {
        public RewardData RewardData;

        public OnMissionCompleteEvent(RewardData rewardData)
        {
            RewardData= rewardData;
        }
    }
    
    public struct OnGrantRewardEvent
    {
        public RewardData RewardData;

        public OnGrantRewardEvent(RewardData rewardData)
        {
            RewardData= rewardData;
        }
    }
    
    public struct OnBallRewardReceivedEvent
    {
        public int BallAmount;

        public OnBallRewardReceivedEvent(int ballAmount)
        {
            BallAmount= ballAmount;
        }
    }
}