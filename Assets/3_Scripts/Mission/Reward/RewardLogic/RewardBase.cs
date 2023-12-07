using UnityEngine;

namespace Reward
{
    public abstract class RewardBase:MonoBehaviour
    {
        [field:SerializeField]public RewardTypeEnum RewardName { get; set; }
        
        public abstract void GrantReward(RewardData rewardData);
    }
}