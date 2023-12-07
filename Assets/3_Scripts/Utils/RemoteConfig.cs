using UnityEngine;

public static class RemoteConfig
{
    public static int INT_LEVEL_GENERATION_BASE_SEED = 15;
    public static bool BOOL_EXPLOSIVE_BARRELS_ENABLED = true;
    public static int INT_EXPLOSIVE_BARRELS_MIN_LEVEL = 2;
    public static bool BOOL_LEVEL_TIMER_ON = true;
    public static float FLOAT_LEVEL_TIMER_SECONDS = 60;
    public static bool BOOL_COLOR_BLIND_ALT_ENABLED = true; 
    public static bool BOOL_PAUSE_BUTTON_ENABLED = true;

    public static bool BOOL_MISSIONS_ENABLED = true;
    public static int INT_MAX_CTIVE_MISSIONS = 3;
    public static bool BOOL_REMOTE_DATA_ENABLED = true;
    public static string STRING_MISSION_DATA_URL = "file://" + Application.dataPath + "/StreamingAssets/mission_data.json";
}
