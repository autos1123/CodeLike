using UnityEditor;
using UnityEngine;
using System.IO;

public class ExcelWatcher:AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets, string[] deletedAssets,
        string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach(string path in importedAssets)
        {
            if(path.EndsWith(".xlsx"))
            {
                Debug.Log($"📦 엑셀 자동 감지됨: {path}");
                ExcelGeneratorWindow window = ScriptableObject.CreateInstance<ExcelGeneratorWindow>();
                ExcelGeneratorWindow.GenerateScriptableObjectsFromExcel(path);
            }
        }
    }
}
