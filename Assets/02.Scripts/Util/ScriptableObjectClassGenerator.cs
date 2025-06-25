using System.IO;
using System.Text;
using System.Data;
using UnityEngine;

public static class ScriptableObjectClassGenerator
{
    public static void GenerateClassFromTable(DataTable table, string className, string scriptOutputPath)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine("[System.Serializable]");
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");

        int columnCount = table.Columns.Count;

        for(int i = 0; i < columnCount; i++)
        {
            string fieldName = table.Rows[0][i].ToString();
            string fieldType = table.Rows[1][i].ToString();

            sb.AppendLine($"    public {fieldType} {fieldName};");
        }

        sb.AppendLine("}");

        string filePath = Path.Combine(scriptOutputPath, className + ".cs");
        Directory.CreateDirectory(scriptOutputPath);
        File.WriteAllText(filePath, sb.ToString());
        Debug.Log($"[SO Generator] {filePath} 생성 완료");
    }
}
