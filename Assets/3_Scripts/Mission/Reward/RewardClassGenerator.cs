using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Reward
{
    public static class RewardClassGenerator
    {
        public static void GenerateClass(string className, Type baseClassType)
        {
            string code = GenerateClassCode(className, baseClassType);

            // Specify the file path where you want to save the generated code
            string[] dir = AssetDatabase.FindAssets("RewardLogic");
            string filePath = $"{AssetDatabase.GUIDToAssetPath(dir[0])}/Reward{className}Logic.cs";

            // Write the code to the file
            File.WriteAllText(filePath, code);

            // Refresh the asset database to make Unity aware of the new file
            UnityEditor.AssetDatabase.Refresh();

            Debug.Log($"Class generated at: {filePath}");

            // Get abstract method names from the base class
        }

        private static string GenerateClassCode(string className, Type baseClassType)
        {
            // Generate the code for the class with a base class and abstract methods
            string code = "\nusing System;\nnamespace Reward\n{\n" +
                          $"    public class Reward{className}Logic : {baseClassType.Name}\n";
            code += "   {\n";

            List<MethodInfo> methodNames = GetAbstractMethodNames(baseClassType);

            // Generate abstract methods
            for (int i = 0; i < methodNames.Count; i++)
            {
                code += $"      public override {methodNames[i].ReturnType.Name.ToLower()} {methodNames[i].Name}(RewardData rewardData)" +
                        "\n      {\n         throw new NotImplementedException();\n         }";
            }

            code += "\n   }\n}\n";

            return code;
        }

        private static List<MethodInfo> GetAbstractMethodNames(Type baseType)
        {
            List<MethodInfo> methodInfos = new List<MethodInfo>();

            if (baseType != null)
            {
                // Get all public instance methods from the base class
                MethodInfo[] methods = baseType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                // Filter abstract methods
                MethodInfo[] abstractMethods = methods.Where(m => m.IsAbstract).ToArray();

                // Display the names of abstract methods
                foreach (MethodInfo method in abstractMethods)
                {
                    methodInfos.Add(method);
                    Debug.Log($"Abstract method name: {method.Name}");
                }
            }
            else
            {
                Debug.LogError($"Failed to get type information for class: {baseType.FullName}");
            }

            return methodInfos;
        }
    }
}