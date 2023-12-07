using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Reward
{
    public static class EnumBuilder
    {
        public static Type BuildEnum(string enumName, string[] enumValues)
        {
            AssemblyName assemblyName = new AssemblyName("DynamicEnumAssembly");
            AssemblyBuilder assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicEnumModule");
            System.Reflection.Emit.EnumBuilder enumBuilder =
                moduleBuilder.DefineEnum(enumName, TypeAttributes.Public, typeof(int));

            for (int i = 0; i < enumValues.Length; i++)
            {
                enumBuilder.DefineLiteral(enumValues[i], i);
            }

            return enumBuilder.CreateTypeInfo();
        }

        public static string GenerateEnumBlock(string enumName, string[] enumValues)
        {
            string enumValuesString = string.Join(", ",
                enumValues.Select(value => $"{value} = {Array.IndexOf(enumValues, value)}"));

            return $@"
public enum {enumName}
{{
   {enumValuesString}
}}";
        }
        
        public static string FindEnumBlock(string scriptContents, string enumName)
        {
            // Regular expression to find the enum block in the script contents based on the enum name
            // This assumes the enum is declared with the specified name
            Regex regex = new Regex($@"\bpublic\s+enum\s+{enumName}\s*{{[\s\S]*?}}", RegexOptions.Singleline);

            Match match = regex.Match(scriptContents);
            if (match.Success)
            {
                return match.Value;
            }

            // Return an empty string if no match is found
            return string.Empty;
        }
    }
}