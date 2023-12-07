using System;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mission
{
    public class MissionUI:Singleton<MissionUI>
    {
        private const string IS_OPEN_ANIMATOR_PARAM = "isOpen";
        
        [SerializeField] private List<MissionItemUI> m_missionItemUIs;
        [SerializeField] private Transform m_parent;
        [SerializeField] private Transform m_missionItemParent;
        [SerializeField] private MissionItemUI m_missionItemUIPrefab;
        
        [SerializeField] private Button m_closeButton;
        [SerializeField] private Button m_blocker;
        
        [field:SerializeField] public Animator Animator { get; private set; }

        private void Awake()
        {
            EventManager.Subscribe<OnSetNewActiveMissionsEvent>(OnSetNewActiveMissions);
            EventManager.Broadcast<OnMissionUIInstantiatedEvent>(new OnMissionUIInstantiatedEvent());

            m_closeButton.onClick.AddListener(OnClose);
            m_blocker.onClick.AddListener(OnClose);
        }

        public void OnOpen()
        {
            Animator.SetBool(IS_OPEN_ANIMATOR_PARAM, true);
        }
        
        public void OnClose()
        {
            Animator.SetBool(IS_OPEN_ANIMATOR_PARAM, false);
        }

        public void Enable()
        {
            m_parent.gameObject.SetActive(true);
        }

        public void Disable()
        {
            m_parent.gameObject.SetActive(false);
        }
        
        private void OnSetNewActiveMissions(OnSetNewActiveMissionsEvent onSetNewActiveMissionsEvent)
        {
            if (m_missionItemUIs != null && m_missionItemUIs.Count > 0)
            {
                m_missionItemUIs.ForEach(x=> Destroy(x.gameObject));
            }
            
            m_missionItemUIs = new List<MissionItemUI>();
            onSetNewActiveMissionsEvent.ActiveMissions.ForEach(missionData =>
            {
                MissionItemUI missionItemUI = Instantiate(m_missionItemUIPrefab,m_missionItemParent);
                missionItemUI.Initialize(missionData,SpriteManager.Instance.GetIcon(missionData.MissionType),
                    SpriteManager.Instance.GetColor(missionData.MissionType),missionData.Name,
                    SpriteManager.Instance.GetIcon(missionData.RewardData.RewardName),missionData.RewardData.Amount);
                m_missionItemUIs.Add(missionItemUI);
            });
            
            EventManager.Subscribe<OnMissionDataUpdateEvent>(OnMissionDataUpdate);

        }

        private void OnMissionDataUpdate(OnMissionDataUpdateEvent onMissionDataUpdate)
        {
            m_missionItemUIs.Find(x=>x.MissionID == onMissionDataUpdate.MissionID).OnMissionDataUpdate();
        }
    }
}