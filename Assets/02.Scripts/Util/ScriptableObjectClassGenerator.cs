using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ClassGenerator
{
    public static void GenerateDataTableClassFromTable(DataTable table, string className, string scriptOutputPath)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine("[System.Serializable]");
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");

        int columnCount = table.Columns.Count;

        for(int i = 0; i < columnCount; i++)
        {
            string fieldName = table.Rows[0][i].ToString();
            string fieldType = table.Rows[1][i].ToString();

            if(string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(fieldType))
            {
                Debug.LogWarning($"⚠️ 필드명 또는 타입이 비어있습니다. 테이블명:{className} 열 인덱스: {i}, 필드명: '{fieldName}', 타입: '{fieldType}'");
                continue;
            }

            sb.AppendLine($"    public {fieldType} {fieldName};");
        }

        sb.AppendLine("}");

        string filePath = Path.Combine(scriptOutputPath, className + ".cs");
        Directory.CreateDirectory(scriptOutputPath);
        File.WriteAllText(filePath, sb.ToString());
        Debug.Log($"[SO Generator] {filePath} 생성 완료");
    }

    public static void GenerateDataTablestructFromTable(DataTable table, string structName, string scriptOutputPath)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine("[System.Serializable]");
        sb.AppendLine($"public struct  {structName}");
        sb.AppendLine("{");

        int columnCount = table.Columns.Count;

        for(int i = 0; i < columnCount; i++)
        {
            string fieldName = table.Rows[0][i].ToString();
            string fieldType = table.Rows[1][i].ToString();

            if(string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(fieldType))
            {
                Debug.LogWarning($"⚠️ 필드명 또는 타입이 비어있습니다. 열 인덱스: {i}, 필드명: '{fieldName}', 타입: '{fieldType}'");
                continue;
            }

            sb.AppendLine($"    public {fieldType} {fieldName};");
        }

        sb.AppendLine("}");

        string filePath = Path.Combine(scriptOutputPath, structName + ".cs");
        Directory.CreateDirectory(scriptOutputPath);
        File.WriteAllText(filePath, sb.ToString());
        Debug.Log($"[SO Generator] {filePath} 생성 완료");
    }

    public class EnumEntry
    {
        public string Group;
        public int ID;
        public string Name;
        public string Description;

        public EnumEntry(string group, int id, string name, string description)
        {
            Group = group;
            ID = id;
            Name = name;
            Description = description;
        }
    }
    public static void GenerateEnumFromTable(DataTable table, string className, string scriptOutputPath)
    {
        List<EnumEntry> entries = table.AsEnumerable()
            .Skip(2)
            .Where(row => !string.IsNullOrWhiteSpace(row[2].ToString())) // Name이 비어있지 않은 것만
            .Select(row => new EnumEntry(
                row[0].ToString().Trim(),
                int.Parse(row[1].ToString()),
                row[2].ToString().Trim(),
                row[3].ToString().Trim()
            )).ToList();

        var grouped = entries.GroupBy(e => e.Group);

        foreach(var group in grouped)
        {
            var sb = new StringBuilder();

            sb.AppendLine("// Auto-generated enum");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"public enum {group.Key}");
            sb.AppendLine("{");

            foreach(var entry in group.OrderBy(e => e.ID))
            {
                sb.AppendLine($"    {entry.Name} = {entry.ID}, // {entry.Description}");
            }

            sb.AppendLine("}");
            var filePath = Path.Combine(scriptOutputPath, $"{group.Key}.cs");
            File.WriteAllText(filePath, sb.ToString());
            Console.WriteLine($"Generated: {filePath}");
        }
    }
}
