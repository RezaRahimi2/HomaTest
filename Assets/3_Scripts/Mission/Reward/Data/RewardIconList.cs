using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mission
{
    [CreateAssetMenu(fileName = "RewardIconList", menuName = "MissionsData/Reward Icon List", order = 0)]
    public class RewardIconList : ScriptableObject
    {
        public List<RewardNameIconName> RewardNameIcons;
        
        public string GetSpriteName(RewardTypeEnum rewardTypeEnum)
        {
            return  RewardNameIcons.Find(x=>x.RewardName == rewardTypeEnum).SpriteName;
        }
    }
}