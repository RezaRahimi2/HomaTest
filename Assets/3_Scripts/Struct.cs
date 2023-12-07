using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Mission;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[Serializable]
public struct RewardData
{
    [SerializeField]
    public RewardTypeEnum RewardName;
    [SerializeField]
    public int Amount;
}

[Serializable]
public struct MissionsListData
{
    [SerializeField]
    public List<MissionData> Missions;
}

[Serializable]
public struct MissionTypeIconName
{
    [SerializeField]
    public MissionTypeEnum MissionName;
    [SerializeField] 
    public Color BackColor;
    [SerializeField]
    public string SpriteName;
}

[Serializable]
public struct RewardNameIconName
{
    [SerializeField]
    public RewardTypeEnum RewardName;
    [SerializeField]
    public string SpriteName;
}