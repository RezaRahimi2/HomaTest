using Events;
using UnityEngine;

namespace Mission
{
    public class CompleteALevelUnderATimeLimitMission : MissionBase
    {
        private float timeLimitInSeconds;
        private float currentTime;

        public CompleteALevelUnderATimeLimitMission(MissionData missionData, MissionManager missionManager) : base(
            missionData, missionManager)
        {
            currentTime = 0f;
            timeLimitInSeconds = missionData.TargetAmount;
            EventManager.Subscribe<OnLevelStartEvent>(OnLevelStart);
            EventManager.Subscribe<OnLevelWinEvent>(OnLevelWin);
            EventManager.Subscribe<OnLevelLoseEvent>(OnLevelLose);
        }

        private void OnLevelLose(OnLevelLoseEvent onLevelLoseEvent)
        {
            currentTime = 0f;
        }

        private void OnLevelStart(OnLevelStartEvent onLevelStartEvent)
        {
            currentTime = 0f;
        }

        private void OnLevelWin(OnLevelWinEvent onLevelWinEvent)
        {
            if (IsMissionComplete())
            {
                Debug.Log($"Starting time-limited mission for level {LevelNumber}. Time limit: {timeLimitInSeconds} seconds.");
                base.UpdateMission();
            }
        }

        public override void StartMission()
        {
            Debug.Log($"Starting time-limited mission for level {LevelNumber}. Time limit: {timeLimitInSeconds} seconds.");
            currentTime = 0f;
        }

        public override void UpdateMission()
        {
            currentTime += Time.deltaTime % 60;;
        }

        public override void Finish()
        {
            Debug.Log($"<color=green>Mission {MissionData.Name} Complete in {currentTime} seconds</color>");
            EventManager.Unsubscribe<OnLevelStartEvent>(OnLevelStart);
            EventManager.Unsubscribe<OnLevelWinEvent>(OnLevelWin);
            m_missionManager.OnMissionComplete(MissionData.MissionID);
        }

        public override bool IsMissionComplete()
        {
            return currentTime <= timeLimitInSeconds;
        }
    }
}