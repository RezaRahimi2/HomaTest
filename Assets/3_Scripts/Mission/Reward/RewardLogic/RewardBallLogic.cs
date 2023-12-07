
using System;
using Events;

namespace Reward
{
    public class RewardBallLogic : RewardBase
   {
      public override void GrantReward(RewardData rewardData)
      {
         EventManager.Broadcast<OnBallRewardReceivedEvent>(new OnBallRewardReceivedEvent(){BallAmount = rewardData.Amount});
      }
   }
}
