using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SoundManager : MonoSingleton<SoundManager>
{
    List<AudioSource> bgmSources;
    List<AudioSource> sfxSources;

    AudioSource curBgm;

    float volumeBGM;
    float volumeSFX;

    public void Init()
    {
        LoadBGMAsync();
        LoadSFXAsync();
    }
    /// <summary>
    /// 어드레서블에 등록된 BGM 불러오기
    /// </summary>
    private void LoadBGMAsync()
    {
        Addressables.LoadAssetsAsync<AudioSource>(
            AddressbleLabels.BGMLabel,
            (source) =>
            {
                bgmSources.Add(source);
            }
        );
    }
    /// <summary>
    /// 어드레서블에 등록된 SFX 불러오기
    /// </summary>
    private void LoadSFXAsync()
    {
        Addressables.LoadAssetsAsync<AudioSource>(
            AddressbleLabels.SFXLabel,
            (source) =>
            {
                sfxSources.Add(source);
            }
        );
    }

}
