using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDestAndEn : MonoBehaviour
{
    public int id;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(TagName.Player))
        {
            if(id == 0)
            {
                UIManager.Instance.ShowUI<DestinyBoard>();
            }
            else if(id == 1) {
                UIManager.Instance.ShowUI<EnhanceBoard>();
            }            
        }
    }
}
