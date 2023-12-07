using UnityEditor;
using UnityEngine;

namespace Reward
{
    [CustomEditor(typeof(RewardManager))]
    public class RewardManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            RewardManager rewardManager = (RewardManager)target;

            if (rewardManager.RewardList != null)
            {
                foreach (RewardBase reward in rewardManager.RewardList)
                {
                    if (GUILayout.Button($"Grant {reward.RewardName} Reward"))
                    {
                        reward.GrantReward(new RewardData());
                    }
                }
            }
        }
    }
}