using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDestAndEn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(TagName.Player))
        {

                UIManager.Instance.ShowUI<EnhanceBoard>();  
        }
    }
}
