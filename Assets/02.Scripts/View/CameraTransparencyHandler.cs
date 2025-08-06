using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransparencyHandler : MonoBehaviour
{
    public Transform player;
    public LayerMask obstacleLayer; // 발판이나 벽 레이어

    private List<Renderer> prevRenderers = new List<Renderer>();

    private void LateUpdate()
    {
        // 현재 시점이 3D가 아닐 경우 → 이전 오브젝트 복구하고 리턴
        if (ViewManager.HasInstance && ViewManager.Instance.CurrentViewMode != ViewModeType.View3D)
        {
            foreach (var rend in prevRenderers)
            {
                if (rend != null)
                    ResetMaterial(rend);
            }
            prevRenderers.Clear();
            return;
        }

        Vector3 camPos = transform.position;
        Vector3 playerPos = player.position + Vector3.up * 1.0f;

        Ray ray = new Ray(camPos, playerPos - camPos);
        float dist = Vector3.Distance(camPos, playerPos);

        RaycastHit[] hits = Physics.RaycastAll(ray, dist, obstacleLayer);

        // 이전 프레임의 오브젝트 복구
        foreach (var rend in prevRenderers)
        {
            if (rend != null)
                ResetMaterial(rend);
        }
        prevRenderers.Clear();

        // 이번 프레임 오브젝트 투명화
        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                MakeTransparent(rend);
                prevRenderers.Add(rend);
            }
        }
    }

    private void MakeTransparent(Renderer rend)
    {
        foreach (var mat in rend.materials)
        {
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            Color c = mat.color;
            c.a = 0.2f;
            mat.color = c;
        }
    }

    private void ResetMaterial(Renderer rend)
    {
        foreach (var mat in rend.materials)
        {
            Color c = mat.color;
            c.a = 1f;
            mat.color = c;
            
            mat.SetFloat("_Mode", 0);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = -1;
        }
    }
}
