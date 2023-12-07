using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tower))]
public class TowerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Tower towerObj = target as Tower;
        if (GUILayout.Button("Build Tower")) {
            TileColorManager.Instance.SetColorList(SaveData.CurrentColorList);
            TileColorManager.Instance.SetMaxColors(Mathf.FloorToInt( GameManager.Instance.colorCountPerLevel.Evaluate(SaveData.CurrentLevel)), true);
            towerObj.BuildTower();
        }
        if (GUILayout.Button("Reset Tower")) {
            towerObj.ResetTower();
        }
        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("StartGame")) {
            towerObj.StartGame();
        }
    }
}
