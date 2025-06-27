using Codice.CM.Client.Differences.Graphic;
using System;
using System.Data;
using UnityEditor;
using UnityEngine;

public static class ExcelSOGenerator
{
    public static void GenerateFromSheet(DataTable table, string sheetName, string scriptOutputPath, string assetOutputPath)
    {
        switch(sheetName)
        {
            case "PlayerData":
                Debug.Log($"[ExcelSOGenerator] PlayerData 처리 시작");

                var conditionData = ScriptableObject.CreateInstance<ConditionData>();

                DataRow typeRow = table.Rows[0];   // 2행 (타입)
                DataRow valueRow = table.Rows[1];  // 3행 (값)

                for(int col = 0; col < table.Columns.Count; col++)
                {
                    string columnName = table.Columns[col].ColumnName.Trim();
                    string typeStr = typeRow[col].ToString().Trim().ToLower();
                    string valueStr = valueRow[col].ToString().Trim();

                    if(!Enum.TryParse(columnName, out ConditionType conditionType))
                    {
                        Debug.LogWarning($"⚠️ ConditionType '{columnName}' 은 ConditionType Enum에 존재하지 않아 무시.");
                        continue;
                    }

                    if(string.IsNullOrEmpty(valueStr))
                    {
                        Debug.LogWarning($"⚠️ '{columnName}' 의 값이 비어있어 무시.");
                        continue;
                    }

                    try
                    {
                        float parsedValue = 0f;
                        if(typeStr == "int")
                            parsedValue = int.Parse(valueStr);
                        else if(typeStr == "float")
                            parsedValue = float.Parse(valueStr);
                        else
                        {
                            Debug.LogWarning($"⚠️ '{columnName}' 의 타입 '{typeStr}' 은 지원하지 않음. 무시.");
                            continue;
                        }

                        conditionData.AddCondition(conditionType, parsedValue);
                        Debug.Log($"✅ {conditionType} = {parsedValue} 적용 완료");
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError($"❌ '{columnName}' 값 '{valueStr}' 파싱 실패: {ex.Message}");
                    }
                }

                string playerAssetPath = $"{assetOutputPath}/PlayerConditionData.asset";
                AssetDatabase.CreateAsset(conditionData, playerAssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"✅ PlayerConditionData SO 생성 완료: {playerAssetPath}");
                break;

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
            case "ActiveItemData":
                break;
            case "DestinyEffectData":
                break;
            default:
                Debug.LogError("이상한 반복 확인");
                break;

        }
    }
}
