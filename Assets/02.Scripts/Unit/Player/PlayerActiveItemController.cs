using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;


public enum Skillinput
{
    X = 0,
    C = 1,
}
public class PlayerActiveItemController : MonoBehaviour
{
    private Dictionary<SkillType, ISkillExecutor> executors;
    private PlayerController playerController;
    public List<ActiveItemData> activeItemDatas = new();

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
    /// 테스트용
    /// </summary>
    /// <param name="activeItemData"></param>
    public void TakeItem(ActiveItemData activeItemData)
    {
        activeItemDatas.Add(activeItemData);
    }

    public void TakeItem(Skillinput skillinput, ActiveItemData activeItemData)
    {
        activeItemDatas[(int)skillinput] = activeItemData;
    }

    public void UseItem(Skillinput skillinput)
    {
        if(activeItemDatas[(int)skillinput] == null)
        {
            Debug.Log("아이템 창이 비어 있음");
            return;
        }
        var used = TableManager.Instance.GetTable<ActiveItemEffectDataTable>().GetDataByID(activeItemDatas[(int)skillinput].skillID);
        executors[used.Type].Execute(used, playerController.VisualTransform , playerController.VisualTransform.forward);
    }
}
