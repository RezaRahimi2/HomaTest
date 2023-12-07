using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Reward;
using UnityEngine;

namespace Mission
{
    public class MissionManager : Singleton<MissionManager>
    {
        private Dictionary<int,MissionBase> m_activeMissions;

        [SerializeField] private bool m_gameIsStarted;
        [SerializeField] private RewardManager m_rewardManager;
        [SerializeField] private List<MissionData> m_allLoadedMissions;
        [SerializeField] private List<MissionData> m_availableMissions;
        [SerializeField] private int m_maxActiveMissions = 3;
        [SerializeField] private CompleteALevelUnderATimeLimitMission m_currentTimeLimitMission;
        
        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            DontDestroyOnLoad(this);
            EventManager.Subscribe<OnMissionDataLoadedEvent>(OnLoadedMissionsData);
            EventManager.Subscribe<OnGameLoadEvent>(OnGameLoad);
            EventManager.Subscribe<OnLevelStartEvent>(OnLevelStart);
            EventManager.Subscribe<OnLevelFinishedEvent>(OnLevelFinished);
            m_maxActiveMissions = RemoteConfig.INT_MAX_CTIVE_MISSIONS;
            m_rewardManager.Initialize();
        }
        
        private void OnLoadedMissionsData(OnMissionDataLoadedEvent onMissionDataLoadedEvent)
        {
            if (onMissionDataLoadedEvent.MissionListData.Missions == null ||
                onMissionDataLoadedEvent.MissionListData.Missions.Count == 0)
            {
                return;
            }

            m_allLoadedMissions = new List<MissionData>();
            m_availableMissions = new List<MissionData>();
            foreach (MissionData missionData in onMissionDataLoadedEvent.MissionListData.Missions)
            {
                m_allLoadedMissions.Add(missionData);
                m_availableMissions.Add(missionData);
            }
            
            SelectMissions();
        }
        
        private void OnGameLoad(OnGameLoadEvent onGameLoadEvent)
        {
            if (m_activeMissions != null && m_activeMissions.Count == 0)
            {
                SelectMissions();
            }
        }

        private void SelectMissions()
        {
             m_activeMissions = new Dictionary<int,MissionBase>();

            if (m_availableMissions == null)
            {
                throw new Exception("there is no missions data");
            }
            
            if (m_availableMissions.Count == 0)
            {
                Debug.LogError("No more missions available, start from the beginning");
                m_availableMissions = m_allLoadedMissions;
            }
            
            int len = (m_availableMissions.Count >= m_maxActiveMissions)?m_maxActiveMissions:m_availableMissions.Count;
            
            for (int i = 0; i < len; i++)
            {
                MissionDifficultiesEnum missionDifficultiesEnum = ((MissionDifficultiesEnum)(i % 3));
                MissionData? selectedMissionData = null;
                bool hasMissionByDifficulty = m_availableMissions.Any(x => x.MissionDifficulty == missionDifficultiesEnum);
                if (!hasMissionByDifficulty)
                {
                    int newI = i + 1;
                    missionDifficultiesEnum = ((MissionDifficultiesEnum)(newI % 3));
                    do
                    {
                        if(m_availableMissions.Any(x => x.MissionDifficulty == missionDifficultiesEnum))
                        {
                            selectedMissionData =
                                m_availableMissions.Find(x => x.MissionDifficulty == missionDifficultiesEnum);
                            break;
                        }
                        newI++;
                        missionDifficultiesEnum = ((MissionDifficultiesEnum)(newI % 3));
                    } while (newI < 3);
                }
                else
                    selectedMissionData = m_availableMissions.Find(x => x.MissionDifficulty == missionDifficultiesEnum);

                if(selectedMissionData == null)
                    break;
                
                UpdateActiveMissions(selectedMissionData.Value);
            }
            
            EventManager.Broadcast<OnSetNewActiveMissionsEvent>(new OnSetNewActiveMissionsEvent(){ActiveMissions = m_activeMissions.Select(x=>x.Value.MissionData).ToList()});
        }

        private void OnLevelStart(OnLevelStartEvent onLevelStartEvent)
        {
            if(m_activeMissions.Any(x=> x.Value.MissionData.MissionType == MissionTypeEnum.CompleteALevelUnderATimeLimit))
                m_currentTimeLimitMission = m_activeMissions.First(x=>
                    x.Value.MissionData.MissionType == MissionTypeEnum.CompleteALevelUnderATimeLimit).Value as CompleteALevelUnderATimeLimitMission;
            
            m_gameIsStarted = true;
        }

        private void OnLevelFinished(OnLevelFinishedEvent onLevelStartEvent)
        {
            m_gameIsStarted = false;
        }
        
        private void Update()
        {
            if (m_gameIsStarted && m_currentTimeLimitMission != null)
            {
                m_currentTimeLimitMission.UpdateMission();
            }
        }

        private void UpdateActiveMissions(MissionData missionData)
        {
            m_activeMissions.Add(missionData.MissionID,CreateMission(missionData));
            m_availableMissions.Remove(missionData);
        }
        
        public void OnMissionComplete(int missionID)
        {
            if (m_activeMissions.TryGetValue(missionID, out var mission))
            {
                m_activeMissions.Remove(missionID);
                
                if(m_currentTimeLimitMission != null && mission == m_currentTimeLimitMission)
                    m_currentTimeLimitMission = null;
                
                EventManager.Broadcast<OnMissionCompleteEvent>(new OnMissionCompleteEvent(){RewardData = mission.MissionData.RewardData});
            }
        }

        private MissionBase CreateMission(MissionData missionData)
        {
            switch (missionData.MissionType)
            {
                case MissionTypeEnum.CollectItems:
                    return new CollectItemsMission(missionData,this);
                case MissionTypeEnum.CompleteSpecificNumberOfLevels:
                    return new CompleteSpecificNumberOfLevelsMission(missionData,this);
                case MissionTypeEnum.CompleteALevelUnderATimeLimit:
                    return new CompleteALevelUnderATimeLimitMission(missionData,this);
                default:
                    // Handle unsupported mission type
                    Debug.LogWarning($"Unsupported mission type: {missionData.MissionType}");
                    return null;
            }
        }
    }
}