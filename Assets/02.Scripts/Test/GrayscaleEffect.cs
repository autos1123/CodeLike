using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
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

    private void OnEnable()
    {
        ViewManager.Instance.OnViewChanged += SwitchView;
    }

    private void OnDisable()
    {
        ViewManager.Instance.OnViewChanged -= SwitchView;
    }

    public void SwitchView(ViewModeType modeType)
    {
        if(modeType == ViewModeType.View3D)
        {
            effectMaterial.SetFloat("_GrayscaleAmount", 0);
            effectMaterial.SetFloat("_Darken", -1f);
        }
        else
        {
            effectMaterial.SetFloat("_GrayscaleAmount", 1);
            effectMaterial.SetFloat("_Darken", -0.5f);
        }
        
    }
}
