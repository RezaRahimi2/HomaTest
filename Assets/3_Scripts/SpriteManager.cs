using Mission;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;

public class SpriteManager : MonoBehaviour
{
    private static SpriteManager _instance;
    public static SpriteManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SpriteManager>();
            }

            return _instance;
        }
    }

    
    [SerializeField]private SpriteAtlas m_spriteAtlas;
    [FormerlySerializedAs("m_missionIconList")] [SerializeField]private MissionViewList mMissionViewList;
    [SerializeField]private RewardIconList m_rewardIconList;
    
    public Color GetColor(MissionTypeEnum missionType)
    {
        return mMissionViewList.GetBackColor(missionType);
    }
    
    public Sprite GetIcon(MissionTypeEnum missionType)
    {
        Debug.Log(mMissionViewList.GetSpriteName(missionType));
        return m_spriteAtlas.GetSprite(mMissionViewList.GetSpriteName(missionType));
    }
    
    public Sprite GetIcon(RewardTypeEnum rewardType)
    {
        Debug.Log(m_rewardIconList.GetSpriteName(rewardType));
        return m_spriteAtlas.GetSprite(m_rewardIconList.GetSpriteName(rewardType));
    }
}