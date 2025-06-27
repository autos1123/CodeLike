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

                var playerConditionTable = ScriptableObject.CreateInstance<ConditionDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)  // 플레이어 데이터가 여러 개면 반복, 아니면 그냥 i=2 한 번만 돌려도 됨
                {
                    var row = table.Rows[i];

                    var conditionData = new ConditionData();
                    conditionData.ID = int.Parse(row[0].ToString());
                    conditionData.CharacterName = row[1].ToString();

                    // 열 순서에 맞게 직접 InitCondition 호출
                    conditionData.InitCondition(ConditionType.HP, float.Parse(row[2].ToString()));
                    conditionData.InitCondition(ConditionType.Stamina, float.Parse(row[3].ToString()));
                    conditionData.InitCondition(ConditionType.StaminaRegen, float.Parse(row[4].ToString()));
                    conditionData.InitCondition(ConditionType.AttackPower, float.Parse(row[5].ToString()));
                    conditionData.InitCondition(ConditionType.AttackSpeed, float.Parse(row[6].ToString()));
                    conditionData.InitCondition(ConditionType.AttackRange, float.Parse(row[7].ToString()));
                    conditionData.InitCondition(ConditionType.Defense, float.Parse(row[8].ToString()));
                    conditionData.InitCondition(ConditionType.MoveSpeed, float.Parse(row[9].ToString()));
                    conditionData.InitCondition(ConditionType.JumpPower, float.Parse(row[10].ToString()));
                    conditionData.InitCondition(ConditionType.CriticalChance, float.Parse(row[11].ToString()));
                    conditionData.InitCondition(ConditionType.CriticalDamage, float.Parse(row[12].ToString()));
                    // 필요하면 추가 컨디션도 넣기

                    playerConditionTable.dataList.Add(conditionData);
                }

                string playerAssetPath = $"{assetOutputPath}/PlayerConditionDataTable.asset";
                AssetDatabase.CreateAsset(playerConditionTable, playerAssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"✅ PlayerConditionDataTable SO 생성 완료: {playerAssetPath}");
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
