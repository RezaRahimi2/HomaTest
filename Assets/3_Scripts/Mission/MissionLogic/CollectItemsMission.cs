using Events;
using UnityEngine;

namespace Mission
{
    public class CollectItemsMission : MissionBase
    {
        public CollectItemsMission(MissionData missionData,MissionManager missionManager) : base(missionData,missionManager)
        {
            CollectedAmount = 0;
            TargetAmount = missionData.TargetAmount;
            
            if( missionData.MissionType == MissionTypeEnum.CollectItems && missionData.CollectItemType == CollectItemTypeEnum.Tile)
                EventManager.Subscribe<OnTileHitEvent>(OnTileHit);
            else if(missionData.MissionType == MissionTypeEnum.CollectItems && missionData.CollectItemType == CollectItemTypeEnum.Tile)
                EventManager.Subscribe<OnTileHitEvent>(OnGemHit);
        }
        
        public override void StartMission()
        {
            CollectedAmount = 0;
        }

        public override bool IsMissionComplete()
        {
            return CollectedAmount >= TargetAmount;
        }

        public override void Finish()
        {
            Debug.Log($"<color=green>Mission {MissionData.Name} Complete</color>");
            EventManager.Unsubscribe<OnTileHitEvent>(OnTileHit);
            EventManager.Unsubscribe<OnTileHitEvent>(OnTileHit);
            m_missionManager.OnMissionComplete(MissionData.MissionID);
        }

        private void OnTileHit(OnTileHitEvent onTileHitEvent)
        {
            CollectedAmount++;
            UpdateMission();
        }
        
        private void OnGemHit(OnTileHitEvent arg0)
        {
            CollectedAmount++;
            UpdateMission();
        }
    }
}