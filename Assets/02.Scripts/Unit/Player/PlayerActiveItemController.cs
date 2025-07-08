using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Skillinput
{
    X = 0,
    C = 1,
}
public class PlayerActiveItemController : MonoBehaviour
{
    private Dictionary<SkillType, ISkillExecutor> executors;
    private PlayerController playerController;
    public ActiveItemData[] activeItemDatas;

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
        activeItemDatas = new ActiveItemData[2];
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
