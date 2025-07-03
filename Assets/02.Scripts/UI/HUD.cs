using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Button minMapButton;
    Button optionButton;
    TextMeshProUGUI goldText;

    Image HPFill;

    Image ItemSlot1;
    Image ItemSlot2;

    private void Awake()
    {
        Debug.Log("[HUD] Awake 시작");

        Transform playerInfo = transform.Find("PlayerInfo");
        Transform miniMapBtnTransform = transform.Find("minimapBtn");
        Transform optionBtnTransform = transform.Find("optionBtn");
        Transform goldInfo = transform.Find("GoldInfo");

        if(miniMapBtnTransform != null)
            minMapButton = miniMapBtnTransform.GetComponent<Button>();

        if(optionBtnTransform != null)
            optionButton = optionBtnTransform.GetComponent<Button>();

        if(goldInfo != null)
            goldText = goldInfo.GetComponent<TextMeshProUGUI>();

        if(playerInfo != null)
        {
            var hpObj = playerInfo.Find("HPFill");
            if(hpObj != null)
                HPFill = hpObj.GetComponent<Image>();

            var itemSlot1Obj = playerInfo.Find("ItemSlot1");
            if(itemSlot1Obj != null)
                ItemSlot1 = itemSlot1Obj.GetComponent<Image>();

            var itemSlot2Obj = playerInfo.Find("ItemSlot2");
            if(itemSlot2Obj != null)
                ItemSlot2 = itemSlot2Obj.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("[HUD] PlayerInfo 오브젝트를 찾을 수 없습니다.");
        }
    }

}
