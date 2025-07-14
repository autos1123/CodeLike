using UnityEditor;
using UnityEngine;
using System.IO;
using ExcelDataReader;
using System.Data;

public class ExcelGeneratorWindow:EditorWindow
{
    private string excelPath = "Assets/DataTable/Data.xlsx";
    private static string scriptOutputPath = "Assets/02.Scripts/Data";
    private static string assetOutputPath = "Assets/05.ScriptableObj/SO";

    [MenuItem("Tools/Excel → SO 생성기")]
    public static void Open()
    {
        GetWindow<ExcelGeneratorWindow>("Excel Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("📄 Excel → ScriptableObject 생성기", EditorStyles.boldLabel);
        excelPath = EditorGUILayout.TextField("Excel 파일 경로", excelPath);
        scriptOutputPath = EditorGUILayout.TextField("data 파일 경로", scriptOutputPath);
        assetOutputPath = EditorGUILayout.TextField("SO 파일 경로", assetOutputPath);
        if(GUILayout.Button("SO 생성"))
        {
            GenerateScriptableObjectsFromExcel(excelPath);
        }
    }

    public static void GenerateScriptableObjectsFromExcel(string path)
    {
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        var result = reader.AsDataSet();

        foreach(DataTable table in result.Tables)
        {
            string sheetName = table.TableName;
            ExcelSOGenerator.GenerateFromSheet(table, sheetName, scriptOutputPath , assetOutputPath);
        }
    }
}
