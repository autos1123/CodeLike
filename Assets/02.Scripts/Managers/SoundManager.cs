using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField] AudioMixer audioMixer;
    //audio clip 담을 수 있는 배열
    [SerializeField] List<AudioClip> bgms;
    [SerializeField] List<AudioClip> sfxs;

    //플레이하는 AudioSource
    [SerializeField] AudioSource audioBgm;
    [SerializeField] AudioSource audioSfx;

    public void Start()
    {
        Init();
    }
    public void Init()
    {
        LoadBGMAsync();
        LoadSFXAsync();
        Addressables.LoadAssetAsync<AudioMixer>("MainMixer")
            .Completed += (handle) =>
            {
                audioMixer = handle.Result;
            };
        //Spatial Blend : 2d 3d 전환일때 사용
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("f1");
            PlayBGM();
        }
        if(Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("f2");
            StopBGM();
        }
        if(Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("f3");
            audioSfx.PlayOneShot(sfxs[0]);
        }
    }

    /// <summary>
    /// 어드레서블에 등록된 BGM , SFX불러오기
    /// </summary>
    private void LoadBGMAsync()
    {
        Addressables.LoadAssetsAsync<AudioClip>(
            AddressbleLabels.BGMLabel,
            (source) =>
            {
                bgms.Add(source);
            }
        );
    }
    private void LoadSFXAsync()
    {
        Addressables.LoadAssetsAsync<AudioClip>(
            AddressbleLabels.SFXLabel,
            (source) =>
            {
                sfxs.Add(source);
            }
        );
    }

    //public void PlayBGM(EBgm bgmIdx)
    public void PlayBGM()
    {
        //enum int형으로 형변환 가능
        //audioBgm.clip = bgms[(int)bgmIdx];
        audioBgm.clip = bgms[0];
        audioBgm.Play();
    }

    // 현재 재생 중인 배경 음악 정지
    public void StopBGM()
    {
        audioBgm.Stop();
    }

    // ESfx 열거형을 매개변수로 받아 해당하는 효과음 클립을 재생
    public void PlaySFX(/*ESfx esfx*/)
    {
        /*audioSfx.PlayOneShot(sfxs[(int)esfx]);*/
    }
}
