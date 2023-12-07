using Coffee.UIExtensions;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mission
{
    public class MissionItemUI : MonoBehaviour
    {
        [field:SerializeField] public int MissionID { get; private set; }
        [SerializeField] private MissionData m_missionData; 
        [SerializeField] private Image m_icon;
        [SerializeField] private Image m_background;
        [SerializeField] private TextMeshProUGUI m_nameText;
        [SerializeField] private Slider m_progressSlider;
        [SerializeField] private RewardUI m_rewardUI;

        public void Initialize(MissionData missionData,Sprite icon,Color backColor, string name, Sprite rewardIcon, int award)
        {
            m_missionData = missionData;
            MissionID = missionData.MissionID;
            m_icon.sprite = icon;
            m_background.color = backColor;
            m_nameText.text = name;
            m_rewardUI.Set(rewardIcon,award);
            if(m_missionData.MissionType != MissionTypeEnum.CompleteALevelUnderATimeLimit)
                m_progressSlider.maxValue = missionData.TargetAmount;
        }

        public void OnMissionDataUpdate()
        {
            if (m_missionData.MissionType == MissionTypeEnum.CompleteALevelUnderATimeLimit)
            {
                m_progressSlider.value = m_missionData.TargetAmount;
                return;
            }

            m_progressSlider.value++;
        }
        
    }
}