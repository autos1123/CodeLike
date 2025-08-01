using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrayscaleEffect : MonoBehaviour
{
    public Material effectMaterial;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(effectMaterial != null)
            Graphics.Blit(source, destination, effectMaterial);
        else
            Graphics.Blit(source, destination); // 머티리얼 없으면 그대로 출력
    }
}
