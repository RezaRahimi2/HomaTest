using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mission
{
    [Serializable]
    public struct MissionData
    {
        [SerializeField]
        public int MissionID;
        [SerializeField]
        public MissionTypeEnum MissionType;
        [SerializeField]
        public string Name;
        [SerializeField]
        public string Description;
        [SerializeField]
        public CollectItemTypeEnum CollectItemType;
        [SerializeField]
        public int TargetAmount;
        [SerializeField]
        public MissionDifficultiesEnum MissionDifficulty; // Add this property if needed for certain MissionDifficultiesEnum
        [SerializeField]
        public RewardData RewardData;
    }
}