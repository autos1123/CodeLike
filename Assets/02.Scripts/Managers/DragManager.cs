using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragManager : MonoSingleton<DragManager>
{
    [Header("드래그용 프리팹")]
    [SerializeField] private GameObject ghostPrefab;

    private GameObject ghostInstance;
    private Image ghostImage;

    /// <summary>
    /// 드래그 고스트 생성
    /// </summary>
    public void CreateGhost(Sprite sprite)
    {
        if (ghostInstance != null)
            Destroy(ghostInstance);

        ghostInstance = Instantiate(ghostPrefab, transform);
        ghostImage = ghostInstance.GetComponent<Image>();
        ghostImage.sprite = sprite;

        ghostInstance.SetActive(true);
        ghostImage.raycastTarget = false;
    }

    /// <summary>
    /// 드래그 중 마우스 위치에 따라 고스트 이동
    /// </summary>
    public void UpdateGhostPosition(Vector2 screenPosition)
    {
        if (ghostInstance != null)
            ghostInstance.transform.position = screenPosition;
    }

    /// <summary>
    /// 드래그 종료 시 고스트 제거
    /// </summary>
    public void ClearGhost()
    {
        if (ghostInstance != null)
        {
            Destroy(ghostInstance);
            ghostInstance = null;
            ghostImage = null;
        }
    }

    protected override bool Persistent => true; // 씬 전환에도 유지됨
    protected override bool ShouldRename => true;
}
