using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 인벤토리 UI를 관리하는 클래스.
/// 슬롯 UI 갱신 및 동기화를 담당.
/// </summary>
public class InventoryUI : UIBase
{
    public override string UIName => "InventoryUI"; 
    
    public Inventory inventory;
    
    public SlotUI[] inventorySlotUIs; // 중간 16칸
    
    public SlotUI[] equipSlotUIs; // 위쪽 4칸 
    
    public SlotUI[] activeSlotUIs; //오른쪽위 2칸
    
    [SerializeField] private TextMeshProUGUI infoText;
    
    /// <summary>
    /// UI 열기 시 슬롯 정보를 동기화
    /// </summary>
    public override void Open()
    {
        base.Open();
        
        foreach (var slot in inventorySlotUIs)
            slot.Init(this); 

        foreach (var slot in equipSlotUIs)
            slot.Init(this);
        foreach (var slot in activeSlotUIs)
            slot.Init(this);
        
        StartCoroutine(WaitAndRefresh());
        
    }
    /// <summary>
    /// 인벤토리 닫을시 설정
    /// </summary>
    public override void Close()
    {
        base.Close();
        TooltipManager.Instance.Hide(); // 툴팁 강제 비활성화
    }
    
    /// <summary>
    /// 슬롯 UI를 실제 인벤토리 데이터로 갱신
    /// </summary>
    public void RefreshUI()
    {
        for(int i = 0; i < inventorySlotUIs.Length; i++)
            inventorySlotUIs[i].Set(inventory.inventorySlots[i]);
        
        for (int i = 0; i < equipSlotUIs.Length; i++)
            equipSlotUIs[i].Set(inventory.equipSlots[i]);
        
        for(int i = 0; i < activeSlotUIs.Length; i++)
            activeSlotUIs[i].Set(inventory.activeItemSlots[i]);
    }
    
    /// <summary>
    /// 인벤토리 초기화가 완료될 때까지 기다린 후 UI 갱신
    /// </summary>
    private IEnumerator WaitAndRefresh()
    {
        yield return new WaitUntil(() =>
            inventory != null &&
            inventory.Initialized &&
            inventory.inventorySlots.Count >= 16);

        RefreshUI();
    }
    
    public void SetInfoText(string text)
    {
        infoText.text = text;
    }
}
