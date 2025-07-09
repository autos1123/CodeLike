using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public int ID;

    [SerializeField]private ItemData data;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => TableManager.Instance.loadComplete);
        data = TableManager.Instance.GetTable<ItemDataTable>().GetDataByID(ID);
    }
}
