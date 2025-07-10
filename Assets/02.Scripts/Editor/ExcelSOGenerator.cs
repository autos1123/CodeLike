using Codice.CM.Client.Differences.Graphic;
using System;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

public static class ExcelSOGenerator
{
    public static void GenerateFromSheet(DataTable table, string sheetName, string scriptOutputPath, string assetOutputPath)
    {
        switch(sheetName)
        {
           
            case "UnitData":

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
                    condition.InitCondition(ConditionType.Gold, float.Parse(row[15].ToString()));

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
                    destiny.Name = row[1].ToString();
                    destiny.PositiveEffectDataID = int.Parse(row[2].ToString());
                    destiny.NegativeEffectDataID = int.Parse(row[3].ToString());
                    destinyDataTable.dataList.Add(destiny);
                }

                string destinyAssetPath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(destinyDataTable, destinyAssetPath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {destinyAssetPath}");
                break;
            case "DestinyEffectData":
                ClassGenerator.GenerateDataTableClassFromTable(table, sheetName, scriptOutputPath);
                var destinyEffectDataTable = ScriptableObject.CreateInstance<DestinyEffectDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var destinyEffect = new DestinyEffectData();
                    destinyEffect.ID = int.Parse(row[0].ToString());
                    destinyEffect.Name = row[1].ToString();
                    destinyEffect.effectType = (EffectType)int.Parse(row[2].ToString());
                    destinyEffect.effectedTarget = (EffectedTarget)int.Parse(row[3].ToString());
                    destinyEffect.conditionType = (ConditionType) Enum.Parse(typeof(ConditionType), row[4].ToString());
                    destinyEffect.value = int.Parse(row[5].ToString());
                    destinyEffect.dsecription = row[6].ToString().Replace("@", destinyEffect.value.ToString());
                    destinyEffectDataTable.dataList.Add(destinyEffect);
                }

                string destinyEffectAssetPath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(destinyEffectDataTable, destinyEffectAssetPath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {destinyEffectAssetPath}");
                break;
            case "ActiveItemData":
                ClassGenerator.GenerateDataTableClassFromTable(table, sheetName, scriptOutputPath);

                var activeItemDataTable = ScriptableObject.CreateInstance<ActiveItemDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var activeItem = new ActiveItemData();
                    activeItem.ID = int.Parse(row[0].ToString());
                    activeItem.name = row[1].ToString();
                    activeItem.rarity= (Rarity)int.Parse(row[2].ToString());
                    activeItem.skillID = int.Parse(row[3].ToString());
                    activeItem.description = row[4].ToString();
                    activeItem.IconPath = row[5].ToString();

                    activeItemDataTable.dataList.Add(activeItem);
                }

                string activeItemAssetPath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(activeItemDataTable, activeItemAssetPath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {activeItemAssetPath}");
                break;
            case "ActiveItemEffectData":
                ClassGenerator.GenerateDataTableClassFromTable(table, sheetName, scriptOutputPath);
                var activeItemEffectDataTable = ScriptableObject.CreateInstance<ActiveItemEffectDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var activeItemEffect = new ActiveItemEffectData();
                    activeItemEffect.SkillID = int.Parse(row[0].ToString());
                    activeItemEffect.Name = row[1].ToString();
                    activeItemEffect.Type = (SkillType)int.Parse(row[2].ToString());
                    activeItemEffect.Power = int.Parse(row[3].ToString());
                    activeItemEffect.Range = int.Parse(row[4].ToString());
                    activeItemEffect.Description = row[5].ToString();
                    activeItemEffect.Cooldown = int.Parse(row[6].ToString());
                    activeItemEffect.CostType = (CostType)int.Parse(row[7].ToString());
                    activeItemEffect.Cost = int.Parse(row[8].ToString());
                    activeItemEffect.EffectPrefab = row[9].ToString();
                    activeItemEffect.Description = row[10].ToString();

                    activeItemEffectDataTable.dataList.Add(activeItemEffect);
                }

                string activeItemEffectAssetPath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(activeItemEffectDataTable, activeItemEffectAssetPath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {activeItemEffectAssetPath}");
                break;
            case "ItemData":
                ClassGenerator.GenerateDataTableClassFromTable(table, sheetName, scriptOutputPath);
                var itemDataTable = ScriptableObject.CreateInstance<ItemDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var itemData = new ItemData();
                    itemData.ID = int.Parse(row[0].ToString());
                    itemData.name = row[1].ToString();
                    itemData.Rarity = (Rarity)int.Parse(row[2].ToString());                    
                    itemData.ConditionType = (ConditionType)Enum.Parse(typeof(ConditionType), row[3].ToString());
                    itemData.value = int.Parse(row[4].ToString());
                    itemData.description = row[5].ToString().Replace("@", itemData.value.ToString());
                    itemData.IconPath = row[6].ToString();
                    itemData.buyPrice = int.Parse(row[7].ToString());
                    //판매가격은 구매가격의 50퍼
                    string rawSell = row[8].ToString();
                    if (rawSell.Trim() == "@")
                    {
                        itemData.sellPrice = Mathf.FloorToInt(itemData.buyPrice * 0.5f); // 50% 계산
                    }
                    else
                    {
                        itemData.sellPrice = int.Parse(rawSell);
                    }

                    itemDataTable.dataList.Add(itemData);
                }

                string itemAssetPath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(itemDataTable, itemAssetPath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {itemAssetPath}");
                break;
            case "EnhanceData":
                ClassGenerator.GenerateDataTableClassFromTable(table, sheetName, scriptOutputPath);

                var enhanceDataTable = ScriptableObject.CreateInstance<EnhanceDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var enhanceData = new EnhanceData();
                    enhanceData.ID = int.Parse(row[0].ToString());
                    enhanceData.name = row[1].ToString();
                    enhanceData.ConditionType = (ConditionType)Enum.Parse(typeof(ConditionType), row[2].ToString().Trim().Replace("\u00A0", ""));
                    enhanceData.value = int.Parse(row[3].ToString());
                    enhanceData.dsecription = row[4].ToString().Replace("@", enhanceData.value.ToString());

                    enhanceDataTable.dataList.Add(enhanceData);
                }

                string enhancePath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(enhanceDataTable, enhancePath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {enhancePath}");

                break;
            case "NPCData":
                ClassGenerator.GenerateDataTableClassFromTable(table, sheetName, scriptOutputPath);

                var npcDataTable = ScriptableObject.CreateInstance<NPCDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var npcData = new NPCData();
                    npcData.ID = int.Parse(row[0].ToString());
                    npcData.Name = row[1].ToString();
                    npcData.shopItemIDs = ParseIntListFromCell(row[2]);
                    npcData.Type = ParseEnumFromCell<NPCType>(row[3]);
                    
                    npcDataTable.dataList.Add(npcData);
                }

                string npcPath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(npcDataTable, npcPath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {npcPath}");

                break;
            case "DialogueData":
                ClassGenerator.GenerateDataTableClassFromTable(table, sheetName, scriptOutputPath);

                var dialogueDataTable = ScriptableObject.CreateInstance<DialogueDataTable>();

                for(int i = 2; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var dialogueData = new DialogueData();
                    dialogueData.ID = int.Parse(row[0].ToString());
                    dialogueData.NPCID = int.Parse(row[0].ToString());
                    dialogueData.description = row[1].ToString();


                    dialogueDataTable.dataList.Add(dialogueData);
                }

                string dialoguePath = $"{assetOutputPath}/{table.ToString()}Table.asset";
                AssetDatabase.CreateAsset(dialogueDataTable, dialoguePath);
                Debug.Log($"✅ {sheetName} SO 생성됨: {dialoguePath}");

                break;
            case "Enums":
                ClassGenerator.GenerateEnumFromTable(table, sheetName, "Assets/02.Scripts/Emuns");
                break;
            default:
                Debug.LogError($"{sheetName}이상한 반복 확인");
                break;

        }
    }
    /// <summary>
    /// 리스트 파싱
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private static List<int> ParseIntListFromCell(object cell)
    {
        var raw = cell.ToString();
        Debug.Log($"🧪 Raw cell: '{raw}'");
        var split = raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var list = new List<int>();

        foreach (var part in split)
        {
            if (int.TryParse(part.Trim(), out int val))
                list.Add(val);
        }

        return list;
    }
    
    /// <summary>
    /// Enum 파싱
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private static T ParseEnumFromCell<T>(object cell) where T : System.Enum
    {
        var raw = cell.ToString().Trim();
        try
        {
            // Enum.Parse를 사용하여 문자열을 Enum 값으로 변환
            return (T)System.Enum.Parse(typeof(T), raw, true); // true는 대소문자 무시
        }
        catch (System.ArgumentException)
        {
            Debug.LogError($"Failed to parse '{raw}' as enum type {typeof(T).Name}. Returning default value.");
            return default(T); // 파싱 실패 시 Enum의 기본값 (보통 0번째 값) 반환
        }
    }
}
