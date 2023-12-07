using System.Collections.Generic;
using UnityEngine;

namespace Mission
{
    [CreateAssetMenu(fileName = "MissionViewList", menuName = "MissionsData/Mission Icon List", order = 0)]
    public class MissionViewList : ScriptableObject
    {
        public List<MissionTypeIconName> MissionIcons;
        
        public string GetSpriteName(MissionTypeEnum missionTypeEnum)
        {
           return  MissionIcons.Find(x=>x.MissionName == missionTypeEnum).SpriteName;
        }
        
        public Color GetBackColor(MissionTypeEnum missionTypeEnum)
        {
            return  MissionIcons.Find(x=>x.MissionName == missionTypeEnum).BackColor;
        }
    }
}