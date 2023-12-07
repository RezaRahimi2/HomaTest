using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mission;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;

namespace Reward
{
    [CustomEditor(typeof(RewardEnumManager))]
    public class RewardEnumManagerEditor : Editor
    {
        private SerializedProperty rewardTypeEnumProp;
        private List<string> rewardTypeNames = new List<string>();
        private int selectedRewardTypeIndex = -1;
        private bool isDirty;
        private string newValue;
        bool isCompiling = false;
        private void OnEnable()
        {
            // Initialize the string list from the enum
            rewardTypeNames = GetEnumNamesList();

            rewardTypeEnumProp = serializedObject.FindProperty("RewardTypeEnum");

            if (rewardTypeEnumProp == null)
            {
                // Use reflection to update the enum
                Type enumType = GetEnumType();
                enumType = EnumBuilder.BuildEnum(enumType.Name, rewardTypeNames.ToArray());

                // Set the updated enum value
                ((RewardEnumManager)target).RewardTypeEnum = (Enum)Enum.Parse(enumType, rewardTypeNames[0]);
            }

            rewardTypeEnumProp = serializedObject.FindProperty("RewardTypeEnum");

            isDirty = false;
        }

        private List<string> GetEnumNamesList()
        {
            Type enumType = GetEnumType();
            return Enum.GetNames(enumType).ToList();
        }

        private Type GetEnumType()
        {
            return (rewardTypeEnumProp == null) ? typeof(RewardTypeEnum) : GetType();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("RewardTypeEnum Values", EditorStyles.boldLabel);

            if (rewardTypeNames.Count == 0)
            {
                EditorGUILayout.HelpBox("Removing all values from the enum will remove all rewards from the game. and cause of error", MessageType.Error);
                EditorGUILayout.HelpBox("Reselect the RewardList", MessageType.Error);
                return;
            }
            
            // Display the enum values as a string list
            for (int i = 0; i < rewardTypeNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                rewardTypeNames[i] = EditorGUILayout.TextField(rewardTypeNames[i]);

                if (GUILayout.Button("Remove", GUILayout.MaxWidth(80)))
                {
                    RemoveEnumValue(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            if (!string.IsNullOrEmpty(newValue) && GUILayout.Button("Add"))
            {
                AddEnumValue();
            }

            EditorGUILayout.Space();

            // Add new enum value
            if (GUILayout.Button("Add"))
            {
                AddEnumValue();
                selectedRewardTypeIndex = rewardTypeNames.Count - 1;
            }
            
            EditorGUILayout.LabelField("Selected Reward Type Index For Generate Code", EditorStyles.boldLabel);
            selectedRewardTypeIndex = EditorGUILayout.IntField(selectedRewardTypeIndex);

            if (selectedRewardTypeIndex != -1)
            {
                // Update the enum using reflection
                if (GUILayout.Button($"Update RewardTypeEnum and Generate {rewardTypeNames[selectedRewardTypeIndex]} Logic Class"))
                {
                    UpdateEnum();
                    selectedRewardTypeIndex = -1;
                }
            }
            
            if (GUILayout.Button("Update RewardManager Prefab in MissionManager"))
            {
                // Generate all enums class dynamically
                EditorApplication.update += UpdateInEditMode;
            }

            // Apply modifications to the serialized object
            serializedObject.ApplyModifiedProperties();

            // Mark the object as dirty if changes were made
            if (isDirty)
            {
                EditorUtility.SetDirty(target);
                isDirty = false;
            }
        }

        private void AddEnumValue()
        {
            // Generate a new name for the enum value
            string newValue = "NewValue" + (rewardTypeNames.Count + 1);
            rewardTypeNames.Add(newValue);
            isDirty = true;
        }

        private void RemoveEnumValue(int index)
        {
            if (index >= 0 && index < rewardTypeNames.Count)
            {
                rewardTypeNames.RemoveAt(index);
                isDirty = true;
            }
        }

        private void UpdateEnum()
        {
            // Get the target object
            RewardEnumManager enumManager = (RewardEnumManager)target;

            // Use reflection to update the enum
            Type enumType = GetEnumType();
            enumType = EnumBuilder.BuildEnum(enumType.Name, rewardTypeNames.ToArray());

            // Set the updated enum value
            enumManager.RewardTypeEnum = (Enum)Enum.Parse(enumType, rewardTypeNames[0]);
            isDirty = true;

            SaveScriptChanges(enumType);
        }

        private void SaveScriptChanges(Type targetType)
        {
            // Get the file path of the assembly
            string scriptPath = GetEnumFilePath(targetType.FullName);

            if (!string.IsNullOrEmpty(scriptPath))
            {
                string enumContents = File.ReadAllText(scriptPath);

                // For example, replace the entire enum block
                string enumBlock = EnumBuilder.GenerateEnumBlock(targetType.Name, rewardTypeNames.ToArray());
                enumContents = enumContents.Replace(EnumBuilder.FindEnumBlock(enumContents,targetType.Name), enumBlock);

                // Write the changes back to the enum file
                File.WriteAllText(scriptPath, enumContents);

                // Define the base class and methods
                Type baseClassType = typeof(RewardBase);

                // Generate selected eum class dynamically
                RewardClassGenerator.GenerateClass(rewardTypeNames[selectedRewardTypeIndex], baseClassType);
            }
        }
        
        private void UpdateInEditMode()
        {
            if (Application.isPlaying || isCompiling)
                return;

            ModifyPrefabAfterCompilation();
        }

        private void ModifyPrefabAfterCompilation()
        {
            isCompiling = true;
            string prefabAssetPath =AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("MissionManagerPrefab")[0]);
            GameObject prefab = PrefabUtility.LoadPrefabContents(prefabAssetPath);

            if (prefab != null)
            {
                // Get all the classes that inherit from RewardBase
                List<RewardBase> rewardBaseClasses = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsSubclassOf(typeof(RewardBase)))
                    .Select(type => Activator.CreateInstance(type) as RewardBase).ToList();
                
                // Modify the value on the prefab instance
                ModifyPrefabInstance(prefab.transform.Find("RewardManager").gameObject,rewardBaseClasses);

                // Save the prefab.
                PrefabUtility.SaveAsPrefabAsset(prefab, prefabAssetPath);
 
                // Unload the prefab to stop editing it.
                PrefabUtility.UnloadPrefabContents(prefab);
            }
            else
            {
                Debug.LogError($"Prefab not found at path: {prefab}");
            }

            isCompiling = false;
            EditorApplication.update -= UpdateInEditMode;
        }

        private void ModifyPrefabInstance(GameObject prefabInstance,List<RewardBase> rewardBases)
        {
            // Example: Modify a component's property on the prefab instance
            RewardManager rewardManager = prefabInstance.GetComponent<RewardManager>();
            if (rewardManager != null)
            {
                Transform logicTransform = prefabInstance.transform.Find("Logic");
                if (!logicTransform)
                {
                    logicTransform = new GameObject("Logic").transform;
                    logicTransform.SetParent(prefabInstance.transform);
                }

                for (int i = 0; i < logicTransform.childCount; i++)
                {
                    // Get some child that you want to delete.
                    GameObject childToDelete = logicTransform.transform.GetChild(i).gameObject;
                    // Make sure you pass true to "allowDestroyingAssets"
                    GameObject.DestroyImmediate(childToDelete, true);
                }

                foreach (var rewardTypeEnumName in Enum.GetNames(typeof(RewardTypeEnum)))
                {
                    GameObject rewardLogicGO = new GameObject($"Reward{rewardTypeEnumName}Logic");
                    rewardLogicGO.transform.SetParent(logicTransform);
                    RewardBase rewardBase = (RewardBase)rewardLogicGO.AddComponent(FindType($"Reward{rewardTypeEnumName}Logic"));
                    rewardBase.RewardName = (RewardTypeEnum)Enum.Parse(typeof(RewardTypeEnum), rewardTypeEnumName);
                }
                
                rewardBases.ForEach(x =>
                {
                    Debug.Log("Find Reward Logic: " + x.GetType().FullName);
                });
            }
        }
        
        public static string GetEnumFilePath(string enumName)
        {
            // Get all assets in the project
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string assetPath in assetPaths)
            {
                if (assetPath.EndsWith(".cs"))
                {
                    // Read the content of the C# file
                    string fileContent = File.ReadAllText(assetPath);

                    // Check if the enum is declared in the file
                    if (fileContent.Contains($"enum {enumName}"))
                    {
                        return assetPath;
                    }
                }
            }

            Debug.LogError($"Enum '{enumName}' not found in any C# file.");
            return null;
        }
        
        public static System.Type FindType(string typeName, bool useFullName = false, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
 
            StringComparison e = (ignoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (useFullName)
            {
                foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var t in assemb.GetTypes())
                    {
                        if (string.Equals(t.FullName, typeName, e)) return t;
                    }
                }
            }
            else
            {
                foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var t in assemb.GetTypes())
                    {
                        if (string.Equals(t.Name, typeName, e) || string.Equals(t.FullName, typeName, e)) return t;
                    }
                }
            }
            return null;
        }
    }
}