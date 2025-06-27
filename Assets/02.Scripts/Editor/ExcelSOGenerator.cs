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

                var conditions = ScriptableObject.CreateInstance<ConditionDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];

                    var condition = new ConditionData();
                    condition.ID = int.Parse(row[0].ToString());
                    condition.CharacterName = row[1].ToString();
                    condition.InitCondition(ConditionType.HP , float.Parse(row[2].ToString()));
                    condition.InitCondition(ConditionType.Stamina, float.Parse(row[3].ToString()));
                    condition.InitCondition(ConditionType.StaminaRegen, float.Parse(row[4].ToString()));
                    condition.InitCondition(ConditionType.AttackPower, float.Parse(row[5].ToString()));
                    condition.InitCondition(ConditionType.AttackSpeed, float.Parse(row[6].ToString()));
                    condition.InitCondition(ConditionType.AttackRange, float.Parse(row[7].ToString()));
                    condition.InitCondition(ConditionType.Defense, float.Parse(row[8].ToString()));
                    condition.InitCondition(ConditionType.MoveSpeed, float.Parse(row[9].ToString()));
                    condition.InitCondition(ConditionType.JumpPower, float.Parse(row[10].ToString()));
                    condition.InitCondition(ConditionType.CriticalChance, float.Parse(row[11].ToString()));
                    condition.InitCondition(ConditionType.CriticalDamage, float.Parse(row[12].ToString()));
                    condition.InitCondition(ConditionType.PatrolRange, float.Parse(row[13].ToString()));
                    condition.InitCondition(ConditionType.ChaseRange, float.Parse(row[14].ToString()));

                    conditions.dataList.Add(condition);
                }
                string enemyAssetPath = $"{assetOutputPath}/ConditionDataTable.asset";
                AssetDatabase.CreateAsset(conditions, enemyAssetPath);
                

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
            case "Enums":
                ClassGenerator.GenerateEnumFromTable(table, sheetName, "Assets/02.Scripts/Emuns");
                break;
            default:
                Debug.LogError($"{sheetName}이상한 반복 확인");
                break;

        }
    }
}
