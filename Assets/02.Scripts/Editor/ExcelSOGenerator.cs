using Codice.CM.Client.Differences.Graphic;
using System.Data;
using UnityEditor;
using UnityEngine;

public static class ExcelSOGenerator
{
    public static void GenerateFromSheet(DataTable table, string sheetName, string scriptOutputPath, string assetOutputPath)
    {
        switch(sheetName)
        {
            case "EnemyData":

                ClassGenerator.GenerateEnemyDataClassFromTable(table, sheetName, scriptOutputPath);

                var enemyList = ScriptableObject.CreateInstance<EnemyDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var enemy = new EnemyData();

                    enemy.ID = int.Parse(row[0].ToString());
                    enemy.Name = row[1].ToString();
                    enemy.HP = int.Parse(row[2].ToString());
                    enemy.ATK = int.Parse(row[3].ToString());
                    enemy.Speed = float.Parse(row[4].ToString());

                    
                    enemyList.dataList.Add(enemy);
                }
                string assetPath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(enemyList, assetPath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {assetPath}");
                break;
            case "DestinyData":
                break;
            case "EnhanceData":
                break;
            case "ItemData":
                break;
            default:
                Debug.LogError("이상한 반복 확인");
                break;

        }
    }
}
