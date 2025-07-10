using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;


public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField] AudioMixer audioMixer;
    //audio clip 담을 수 있는 배열
    [SerializeField] List<AudioClip> bgms;
    [SerializeField] List<AudioClip> sfxs;

    Dictionary<string , AudioClip> bgmdic;
    Dictionary<string , AudioClip> sfxdic;
    //플레이하는 AudioSource
    [SerializeField] SoundSource audioBgm;

    public void Start()
    {
        Init();
    }
    public void Init()
    {
        Addressables.LoadAssetAsync<AudioMixer>("MainMixer")
            .Completed += (handle) =>
            {
                audioMixer = handle.Result;
            };
        LoadBGMAsync();
        LoadSFXAsync();
        bgmdic = new Dictionary<string, AudioClip>();
        sfxdic = new Dictionary<string, AudioClip>();
    }

    //테스트용 추후에 지움
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("f1");
            PlayBGM(transform.position , SoundAddressbleName.bgm1);
        }
        if(Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("f2");
            StopBGM();
        }
        if(Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("f3");
            PlaySFX(transform.position, SoundAddressbleName.sfx1);
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
        ).Completed += (handle) =>
        {
            foreach(var bgm in bgms)
            {
                bgmdic[bgm.name] = bgm;
            }
        };
    }
    private void LoadSFXAsync()
    {
        Addressables.LoadAssetsAsync<AudioClip>(
            AddressbleLabels.SFXLabel,
            (source) =>
            {
                sfxs.Add(source);
            }
        ).Completed += (handle) =>
        {
            foreach(var sfx in sfxs)
            {
                sfxdic[sfx.name] = sfx;
            }
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos">소리 생성 포지션</param>
    /// <param name="key">재생할 소리 이름</param>
    public void PlaySFX(Vector3 pos,string key)
    {
        var sound = PoolManager.Instance.GetObject(PoolType.SoundSource);
        sound.transform.position = pos;
        SoundSource soundSource = sound.GetComponent<SoundSource>();
        soundSource.Play(0.2f,sfxdic[key], true);
    }
    public void PlayBGM(Vector3 pos, string key)
    {
        if(audioBgm != null) StopBGM();

        var sound = PoolManager.Instance.GetObject(PoolType.SoundSource);
        sound.transform.position = pos;
        SoundSource soundSource = sound.GetComponent<SoundSource>();        
        soundSource.Play(bgmdic[key],false);
        audioBgm = soundSource;
    }

    // 현재 재생 중인 배경 음악 정지
    public void StopBGM()
    {
        audioBgm.Stop();
        audioBgm = null;
    }

}
