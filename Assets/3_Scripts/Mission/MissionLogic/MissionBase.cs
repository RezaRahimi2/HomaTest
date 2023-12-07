using Events;
using UnityEngine;

namespace Mission
{
    public abstract class MissionBase
    {
        protected MissionManager m_missionManager;
        [field: SerializeField] public MissionData MissionData { get; private set; }
        public int TargetAmount;
        public int CollectedAmount;
        public int LevelNumber;

        // Constructor that takes MissionData as a parameter
        protected MissionBase(MissionData missionData,MissionManager missionManager)
        {
            MissionData = missionData;
            m_missionManager = missionManager; 
            TargetAmount = missionData.TargetAmount;
            LevelNumber = missionData.MissionID;
        }

        public abstract void StartMission();

        public virtual void UpdateMission()
        {
            Debug.Log($"<color=blue>Mission Update {CollectedAmount}|{MissionData.CollectItemType}</color>");
            EventManager.Broadcast<OnMissionDataUpdateEvent>(new OnMissionDataUpdateEvent(){MissionID = MissionData.MissionID});
            if (IsMissionComplete())
            {
                Finish();
            }
        }
        
        public abstract void Finish();
        
        public abstract bool IsMissionComplete();
    }
}