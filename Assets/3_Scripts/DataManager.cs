using System;
using System.Collections.Generic;
using Events;
using Mission;
using UnityEngine;
using UnityEngine.Serialization;

public class DataManager : MonoBehaviour
{
    [FormerlySerializedAs("missions")] [SerializeField]private MissionsListData m_missions;
    private WebRequest webRequest;

    public void Awake()
    {
        if(!RemoteConfig.BOOL_MISSIONS_ENABLED)
            return;
        
        if (RemoteConfig.BOOL_REMOTE_DATA_ENABLED)
        {
            webRequest = new WebRequest();
            webRequest.MakeRequest<MissionsListData>(RemoteConfig.STRING_MISSION_DATA_URL, OnMissionDataReceived);
        }
        else
        {
            //Load Mission Data from Scriptable Object
        }
    }

    private void OnMissionDataReceived(MissionsListData data)
    {
        m_missions = data;
        EventManager.Broadcast<OnMissionDataLoadedEvent>(new OnMissionDataLoadedEvent(){MissionListData = m_missions});
    }
}
