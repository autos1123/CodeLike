using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD:MonoBehaviour
{
    Button minMapButton;
    Button optionButton;
    TextMeshProUGUI goldText;

    Image HPFill;

    Image ItemSlot1;
    Image ItemSlot2;

    private void Awake()
    {
        Debug.Log($"Child Count: {transform.childCount}");
        minMapButton = transform.GetChild(1).GetComponent<Button>();
        optionButton = transform.GetChild(2).GetComponent<Button>();
        goldText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        HPFill = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>();
        ItemSlot1 = transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
        ItemSlot2 = transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Image>();
    }
}