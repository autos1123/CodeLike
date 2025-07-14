using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;


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
    public List<ActiveItemData> activeItemDatas = new();
    [SerializeField] private Transform projectileSpawnPos;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        executors = new Dictionary<SkillType, ISkillExecutor>
        {
            { SkillType.Projectile, new ProjectileSkillExecutor() },
            { SkillType.AoE , new AoESkillExecutor() },
            { SkillType.Heal , new HealSkillExecutor()}
        };
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
        Debug.Log($"슬롯 {index}에 아이템 {activeItemData?.name} 장착 완료");
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
            Debug.Log("아이템 창이 비어 있음");
            return;
        }

        var used = TableManager.Instance.GetTable<ActiveItemEffectDataTable>().GetDataByID(activeItemDatas[index].skillID);
        executors[used.Type].Execute(used, projectileSpawnPos, projectileSpawnPos.forward);
    }
}