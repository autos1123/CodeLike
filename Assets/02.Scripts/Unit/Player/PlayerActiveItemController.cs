using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;


public enum Skillinput
{
    None = -1, // 기본값, 아이템이 없는 상태
    X = 0,
    C = 1,
}
public class PlayerActiveItemController:MonoBehaviour
{
    private Dictionary<SkillType, ISkillExecutor> executors;
    private PlayerController playerController;
    private ActiveItemEffectDataTable activeItemEffectDataTable;

    public List<ActiveItemData> activeItemDatas = new();
    [SerializeField] private Transform projectileSpawnPos;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {        
        activeItemEffectDataTable = TableManager.Instance.GetTable<ActiveItemEffectDataTable>();
        executors = new Dictionary<SkillType, ISkillExecutor>
        {
            { SkillType.Projectile, new ProjectileSkillExecutor() },
            { SkillType.AoE , new AoESkillExecutor() },
            { SkillType.Heal , new HealSkillExecutor() },
            { SkillType.Zone , new ZoneSkillExecutor() },
        };
    }

    private void Update()
    {
        ActiveItemEffectData used = new();
        if(Input.GetKeyDown(KeyCode.F3))
        {
            used = activeItemEffectDataTable.GetDataByID(5001);
            executors[used.Type].Execute(used, projectileSpawnPos, projectileSpawnPos.forward);
        }
        if(Input.GetKeyDown(KeyCode.F4))
        {
            used = activeItemEffectDataTable.GetDataByID(5002);
            executors[used.Type].Execute(used, projectileSpawnPos, projectileSpawnPos.forward);
        }
        if(Input.GetKeyDown(KeyCode.F5))
        {
            used = activeItemEffectDataTable.GetDataByID(5003);
            executors[used.Type].Execute(used, transform, projectileSpawnPos.forward);
        }
        if(Input.GetKeyDown(KeyCode.F6))
        {
            used = activeItemEffectDataTable.GetDataByID(5004);
            executors[used.Type].Execute(used, projectileSpawnPos, projectileSpawnPos.forward);
        }
        
    }

    /// <summary>
    /// 테스트용 (기존 방식 유지)
    /// </summary>
    /// <param name="activeItemData"></param>
    public void TakeItem(ActiveItemData activeItemData)
    {
        activeItemDatas.Add(activeItemData);
    }

    public void TakeItem(Skillinput skillinput, ActiveItemData activeItemData)
    {
        int index = (int)skillinput;

        // 리스트 크기 확장 (필요한 경우만)
        while(activeItemDatas.Count <= index)
        {
            activeItemDatas.Add(null);
        }

        activeItemDatas[index] = activeItemData;
    }

    public void UseItem(Skillinput skillinput)
    {
        int index = (int)skillinput;

        // 안전한 인덱스 체크
        if(index < 0 || index >= activeItemDatas.Count)
        {
            Debug.LogError($"잘못된 스킬 인덱스: {index}");
            return;
        }

        if(activeItemDatas[index] == null)
        {
            return;
        }

        var used = activeItemEffectDataTable.GetDataByID(activeItemDatas[index].skillID);
        executors[used.Type].Execute(used, projectileSpawnPos, projectileSpawnPos.forward);
    }
}