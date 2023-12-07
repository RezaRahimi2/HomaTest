using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mission
{

    [CreateAssetMenu(fileName = "MissionListData", menuName = "MissionsData/Mission List", order = 1)]
    public class MissionList : ScriptableObject
    {
        public List<MissionData> Missions = new List<MissionData>();
    }

}