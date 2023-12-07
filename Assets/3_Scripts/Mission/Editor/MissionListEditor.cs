using System;
using System.Collections.Generic;
using System.IO;
using Mission;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionList))]
public class MissionListEditor : Editor
{
    private string _jsonFilePath;
    private SerializedProperty _missionsProp;
    
    private void OnEnable()
    {
        _missionsProp = serializedObject.FindProperty("Missions");
        _jsonFilePath = Application.dataPath + "/StreamingAssets/mission_data.json";
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Mission"))
        {
            AddNewMission();
        }

        EditorGUILayout.Space();

        // Show CollectItemType if MissionType is CollectItems
        for (int i = 0; i < _missionsProp.arraySize; i++)
        {
            SerializedProperty missionProp = _missionsProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            DrawMissionFields(missionProp,i);

            EditorGUILayout.Space();

            if (GUILayout.Button("Remove Mission"))
            {
                RemoveMission(i);
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Import Json"))
        {
            ImportJson();
        }
        
        if (GUILayout.Button("Export Json"))
        {
            ExportJson();
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawMissionFields(SerializedProperty missionProp, int index)
    {
        SerializedProperty missionIDProp = missionProp.FindPropertyRelative("MissionID");
        SerializedProperty missionTypeEnumProp = missionProp.FindPropertyRelative("MissionType");
        SerializedProperty collectItemTypeEnumProp = missionProp.FindPropertyRelative("CollectItemType");
        SerializedProperty targetAmountProp = missionProp.FindPropertyRelative("TargetAmount");
        SerializedProperty missionDifficultyProp = missionProp.FindPropertyRelative("MissionDifficulty");
        SerializedProperty missionMissionNameProp = missionProp.FindPropertyRelative("Name");
        SerializedProperty missionMissionDescriptionProp = missionProp.FindPropertyRelative("Description");
        

        // Make missionData read-only and auto-fill based on index
        EditorGUI.BeginDisabledGroup(true);
        missionIDProp.intValue = index;
        EditorGUILayout.PropertyField(missionIDProp, new GUIContent("Mission ID"));
        EditorGUI.EndDisabledGroup();

        MissionTypeEnum missionTypeEnum = (MissionTypeEnum)missionTypeEnumProp.enumValueIndex;

        // Expose fields manually
        EditorGUILayout.PropertyField(missionTypeEnumProp, new GUIContent("Mission Type"));
        if (missionTypeEnum == MissionTypeEnum.CollectItems)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(collectItemTypeEnumProp, new GUIContent("Collect Item Type"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(targetAmountProp, new GUIContent( missionTypeEnum == MissionTypeEnum.CompleteALevelUnderATimeLimit ? "Max Time(Seconds)" : "Target Amount"));
        EditorGUILayout.PropertyField(missionDifficultyProp, new GUIContent("Mission Difficulty"));
        
        EditorGUILayout.PropertyField(missionMissionNameProp, new GUIContent("Mission Name"));
        EditorGUILayout.PropertyField(missionMissionDescriptionProp, new GUIContent("Mission Description"));
        
        EditorGUILayout.PropertyField(missionProp.FindPropertyRelative("RewardData"), new GUIContent("Reward Data"));
        
    }

    private void AddNewMission()
    {
        serializedObject.Update();
        _missionsProp.arraySize++;
        serializedObject.ApplyModifiedProperties();
    }

    private void RemoveMission(int index)
    {
        serializedObject.Update();
        _missionsProp.DeleteArrayElementAtIndex(index);
        serializedObject.ApplyModifiedProperties();
    }

    private void ImportJson()
    {
        // Read the JSON data from a file in the StreamingAssets directory
        string json = File.ReadAllText(_jsonFilePath);

        // Deserialize the JSON data into MissionListData
        MissionsListData missionList =  new MissionsListData();
        JsonUtility.FromJsonOverwrite(json, missionList);

        if (missionList.Missions != null)
        {
            // Clear existing missions
            _missionsProp.ClearArray();
        
            // Add the imported missions to missionsProp
            for (int i = 0; i < missionList.Missions.Count; i++)
            {
                _missionsProp.InsertArrayElementAtIndex(i);
                SerializedProperty missionProp = _missionsProp.GetArrayElementAtIndex(i);
                FillMissionProperty(missionProp, missionList.Missions[i]);
            }
        
            // Apply modifications to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void FillMissionProperty(SerializedProperty missionProp, MissionData missionData)
    {
        missionProp.FindPropertyRelative("missionData").intValue = missionData.MissionID;
        missionProp.FindPropertyRelative("MissionType").enumValueIndex = (int)missionData.MissionType;

        if (missionData.MissionType == MissionTypeEnum.CollectItems)
        {
            missionProp.FindPropertyRelative("CollectItemType").enumValueIndex = (int)missionData.CollectItemType;
        }
        
        missionProp.FindPropertyRelative("TargetAmount").intValue = missionData.TargetAmount;
        missionProp.FindPropertyRelative("MissionDifficulty").intValue = (int)missionData.MissionDifficulty;
        missionProp.FindPropertyRelative("Name").stringValue = missionData.Name;
        missionProp.FindPropertyRelative("Description").stringValue = missionData.Description;
    }

    private void ExportJson()
    {
        MissionsListData missionList = new MissionsListData();
        missionList.Missions = new List<MissionData>();
        
        for (int i = 0; i < _missionsProp.arraySize; i++)
        {
            SerializedProperty missionProp = _missionsProp.GetArrayElementAtIndex(i);
            MissionData missionData = ExtractMissionData(missionProp);
            missionList.Missions.Add(missionData);
        }

        string json = JsonUtility.ToJson(missionList, true);

        if(!Directory.Exists(Application.dataPath + "/StreamingAssets"))
        {
            Directory.CreateDirectory(Application.dataPath + "/StreamingAssets");
        }
        
        // Write the JSON data to a file in the StreamingAssets directory
        File.WriteAllText(_jsonFilePath, json);
        Debug.Log("Exported JSON to: " + _jsonFilePath);

        AssetDatabase.Refresh();
    }
    
    private MissionData ExtractMissionData(SerializedProperty missionProp)
    {
        MissionData missionData = new MissionData();

        missionData.MissionID = missionProp.FindPropertyRelative("MissionID").intValue;
        missionData.MissionType = (MissionTypeEnum)missionProp.FindPropertyRelative("MissionType").enumValueIndex;

        if (missionData.MissionType == MissionTypeEnum.CollectItems)
        {
            missionData.CollectItemType = (CollectItemTypeEnum)missionProp.FindPropertyRelative("CollectItemType").enumValueIndex;
        }

        missionData.TargetAmount = missionProp.FindPropertyRelative("TargetAmount").intValue;
        missionData.MissionDifficulty = (MissionDifficultiesEnum)missionProp.FindPropertyRelative("MissionDifficulty").enumValueIndex;
        missionData.Name = missionProp.FindPropertyRelative("Name").stringValue;
        missionData.Description = missionProp.FindPropertyRelative("Description").stringValue;
        
        SerializedProperty rewardDataProp = missionProp.FindPropertyRelative("RewardData");
        missionData.RewardData.RewardName = (RewardTypeEnum)rewardDataProp.FindPropertyRelative("RewardName").enumValueIndex;
        missionData.RewardData.Amount = rewardDataProp.FindPropertyRelative("Amount").intValue;

        return missionData;
    }
}
