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

                ClassGenerator.GenerateDataTableClassFromTable(table, sheetName, scriptOutputPath);

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
                string enemyAssetPath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(enemyList, enemyAssetPath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {enemyAssetPath}");
                break;
            case "DestinyData":
                ClassGenerator.GenerateDataTableClassFromTable(table, sheetName, scriptOutputPath);

                var destinyDataTable = ScriptableObject.CreateInstance<DestinyDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var destiny = new DestinyData();
                    destiny.ID = int.Parse(row[0].ToString());
                    destinyDataTable.dataList.Add(destiny);
                }

                string destinyAssetPath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(destinyDataTable, destinyAssetPath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {destinyAssetPath}");
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
