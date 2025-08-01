using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionBoard : UIBase
{
    public override string UIName => this.GetType().Name;
    private readonly string resolutionDataKey = "ResolutionData"; // 해상도 데이터 키
    private readonly string screenModeDataKey = "ScreenModeData"; // 해상도 데이터 키

    private readonly string sfxVolumeDataKey = "SFX"; // SFX 볼륨 데이터 키
    private readonly string bgmVolumeDataKey = "BGM"; // BGM 볼륨 데이터 키
    private readonly string masterVolumeDataKey = "Master"; // 마스터 볼륨 데이터 키

    private readonly string sfxMuteDataKey = "SFXMuteData"; // SFX 음소거 데이터 키
    private readonly string bgmMuteDataKey = "BGMMuteData"; // BGM 음소거 데이터 키
    private readonly string masterMuteDataKey = "MasterMuteData"; // 마스터 음소거 데이터 키

    private ResolutionData resolutionData; // 해상도 데이터
    Resolution[] resolutions; // 해상도 리스트를 배열로 변환

    [Header("DisplaySettings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown; // 해상도 드롭다운
    [SerializeField] private TMP_Dropdown fullscreenDropdown; // 전체화면 모드 드롭다운
    [SerializeField] private TMP_Dropdown qualityDropdown; // 그래픽 품질 드롭다운

    [Header("Exit Button")]
    [SerializeField] private Button exitToLobbyButton; // 로비로 돌아가기 버튼
    [SerializeField] private Button exitToGameButton; // 게임으로 돌아가기 버튼
    [SerializeField] private string gameSceneName;

    [Header("AudioMixe")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Slider")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Audio Mute")]
    [SerializeField] private Toggle masterToggle;
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;


    bool hasSavedResolution = false;

    private void Awake()
    {
        resolutionData = new ResolutionData(); // 해상도 데이터 초기화
        resolutions = resolutionData.resolutions.ToArray(); // 해상도 리스트를 배열로 변환
        InitResolutionDropdown();
        InitFullScreenModeDropDown();

        SetSliderValue(masterSlider, masterVolumeDataKey);
        SetSliderValue(bgmSlider, bgmVolumeDataKey);
        SetSliderValue(sfxSlider, sfxVolumeDataKey);

        SetToggleValue(sfxToggle, sfxMuteDataKey);
        SetToggleValue(bgmToggle, bgmMuteDataKey);
        SetToggleValue(masterToggle, masterMuteDataKey);

        ApplySoundSetting(masterVolumeDataKey, masterMuteDataKey);
        ApplySoundSetting(bgmVolumeDataKey, masterMuteDataKey);
        ApplySoundSetting(sfxVolumeDataKey, masterMuteDataKey);
    }

    public override void Open()
    {
        base.Open();
        string currentScene = SceneManager.GetActiveScene().name;
        if(currentScene != "IntroScene")
        {
            SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position, "OptionOpen");
        }

        resolutionDropdown.onValueChanged.AddListener(ResolutionDropDownOptionChange); // 해상도 드롭다운 값 변경 이벤트 등록
        fullscreenDropdown.onValueChanged.AddListener(ScreenModeDropDownOptionChange); // 화면 모드 드롭다운 값 변경 이벤트 등록

        // 사운드 설정 슬라이더
        masterSlider.onValueChanged.AddListener((Volume) =>
        {
            SetVolume(masterVolumeDataKey, Volume);
            ApplySoundSetting(masterVolumeDataKey, masterMuteDataKey);
        });
        bgmSlider.onValueChanged.AddListener((Volume) =>
        {
            SetVolume(bgmVolumeDataKey, Volume);
            ApplySoundSetting(bgmVolumeDataKey, bgmMuteDataKey);
        });
        sfxSlider.onValueChanged.AddListener((Volume) =>
        {
            SetVolume(sfxVolumeDataKey, Volume);
            ApplySoundSetting(sfxVolumeDataKey, sfxMuteDataKey);
        });

        // 사운드 음소거 토글
        masterToggle.onValueChanged.AddListener((isOn) =>
        {
            SetMute(masterMuteDataKey, isOn); // 음소거 상태 저장
            ApplySoundSetting(masterVolumeDataKey, masterMuteDataKey); // 사운드 설정 적용
        });
        bgmToggle.onValueChanged.AddListener((isOn) =>
        {
            SetMute(bgmMuteDataKey, isOn); // 음소거 상태 저장
            ApplySoundSetting(bgmVolumeDataKey, bgmMuteDataKey); // 사운드 설정 적용
        });
        sfxToggle.onValueChanged.AddListener((isOn) =>
        {
            SetMute(sfxMuteDataKey, isOn); // 음소거 상태 저장
            ApplySoundSetting(sfxVolumeDataKey, sfxMuteDataKey); // 사운드 설정 적용
        });

        if(exitToGameButton!=null) exitToGameButton.onClick.AddListener(OnClickExitToGameButton); // 게임으로 돌아가는 버튼 클릭 이벤트 등록
        if(exitToLobbyButton != null) exitToLobbyButton.onClick.AddListener(OnClickExitToLobbyButton); // 로비로 돌아가는 버튼 클릭 이벤트 등록


        // 로비로 돌아가는 버튼은 게임씬에서만 활성화
        if(SceneManager.GetActiveScene().name != gameSceneName && exitToLobbyButton != null)
        {
            exitToLobbyButton.gameObject.SetActive(true);
        }
    }


    public override void Close()
    {
        base.Close();
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"OptionClose");

        resolutionDropdown.onValueChanged.RemoveAllListeners(); // 해상도 드롭다운 값 변경 이벤트 제거
        fullscreenDropdown.onValueChanged.RemoveAllListeners(); // 화면 모드 드롭다운 값 변경 이벤트 제거

        exitToGameButton.onClick.RemoveAllListeners(); // 게임으로 돌아가는 버튼 클릭 이벤트 제거
        exitToLobbyButton.onClick.RemoveAllListeners(); // 로비로 돌아가는 버튼 클릭 이벤트 제거
    }

    // 해상도 드롭다운 초기화
    private void InitResolutionDropdown()
    {
        resolutionDropdown.options.Clear();

        for(int i = 0; i < resolutions.Length; i++)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = resolutions[i].width + "x" + resolutions[i].height + " @ " + Mathf.Round((float)resolutions[i].refreshRateRatio.value) + "hz"; // ex)1920x1080@60hz
            resolutionDropdown.options.Add(option);

            // 저장된 해상도 데이터가 없을 경우
            // 해상도 드롭다운 현재 값을 해상도 리스트의 인덱스와 동일하게 설정
            // 현재 해상도 인덱스 정보 저장
            if(!PlayerPrefs.HasKey(resolutionDataKey))
            {
                // 탐색한 해상도가 현재 적용된 해상도일 경우
                if(resolutions[i].Equals(Screen.currentResolution))
                {
                    PlayerPrefs.SetInt(resolutionDataKey, i);
                    hasSavedResolution = true;
                }
            }
        }

        int idx = PlayerPrefs.GetInt(resolutionDataKey);
        // 현재 해상도 인덱스를 저장 및 Dropdown UI에 반영
        resolutionDropdown.value = idx;
        ResolutionDropDownOptionChange(idx);

        resolutionDropdown.RefreshShownValue();

#if UNITY_WEBGL
        if(hasSavedResolution)
        {
            PlayerPrefs.Save(); // WebGL에서는 확실하게 저장
            Debug.Log("Saved resolution index in WebGL PlayerPrefs.");
        }
#endif
    }

    // Dropdown UI 값 변경 이벤트
    private void ResolutionDropDownOptionChange(int x)
    {
        int screenModeIdx = (int)FullScreenMode.Windowed;

        if(PlayerPrefs.HasKey(screenModeDataKey))
        {
            screenModeIdx = PlayerPrefs.GetInt(screenModeDataKey);
        }

        StartCoroutine(FixWebGLCanvas(x, screenModeIdx));

        PlayerPrefs.SetInt(resolutionDataKey, x);
    }
    IEnumerator FixWebGLCanvas(int x , int screenModeIdx)
    {
        yield return new WaitForEndOfFrame(); // 초기화 후 한 프레임 기다림
        Screen.SetResolution(resolutions[x].width,
    resolutions[x].height,
    GetScreenModeToIndex(screenModeIdx),
    resolutions[x].refreshRateRatio);
    }

    // 화면 모드 드롭다운 초기화
    private void InitFullScreenModeDropDown()
    {
        fullscreenDropdown.options.Clear();

        fullscreenDropdown.options.Add(new TMP_Dropdown.OptionData("전체화면"));
        fullscreenDropdown.options.Add(new TMP_Dropdown.OptionData("테두리 없는 창모드"));
        fullscreenDropdown.options.Add(new TMP_Dropdown.OptionData("창모드"));

        if(!PlayerPrefs.HasKey(screenModeDataKey))
        {
            switch(Screen.fullScreenMode)
            {
                case FullScreenMode.FullScreenWindow:
                    PlayerPrefs.SetInt(resolutionDataKey, 0); // 전체화면
                    break;
                case FullScreenMode.MaximizedWindow:
                    PlayerPrefs.SetInt(resolutionDataKey, 1); // 테두리 없는 창모드
                    break;
                case FullScreenMode.Windowed:
                    PlayerPrefs.SetInt(resolutionDataKey, 2); // 창모드
                    break;
            }
        }

        fullscreenDropdown.value = PlayerPrefs.GetInt(screenModeDataKey);

        fullscreenDropdown.RefreshShownValue();
    }

    private void ScreenModeDropDownOptionChange(int x)
    {
        Screen.fullScreenMode = GetScreenModeToIndex(x);

        PlayerPrefs.SetInt(screenModeDataKey, x);
    }

    private FullScreenMode GetScreenModeToIndex(int index)
    {
        switch(index)
        {
            case 0:
                return FullScreenMode.ExclusiveFullScreen; // 전체화면
            case 1:
                return FullScreenMode.FullScreenWindow; // 테두리 없는 창모드
            case 2:
                return FullScreenMode.Windowed; // 창모드
            default:
                return FullScreenMode.ExclusiveFullScreen; // 기본값은 전체화면
        }
    }
    private void SetSliderValue(Slider slider, string path)
    {
        if(PlayerPrefs.HasKey(path))
        {
            slider.value = PlayerPrefs.GetFloat(path);
        }
        else
        {
            slider.value = 1f; // 초기값은 1(최대 볼륨)
        }
    }

    private void SetToggleValue(Toggle toggle, string path)
    {
        if(PlayerPrefs.HasKey(path))
        {
            toggle.isOn = PlayerPrefs.GetInt(path) == 0; // 1이면 음소거 해제, 0이면 음소거
        }
        else
        {
            toggle.isOn = false; // 초기값은 음소거 해제
        }
    }

    private void SetVolume(string path,float value)
    {
        PlayerPrefs.SetFloat(path, value);
    }

    private void SetMute(string path, bool isMute)
    {
        PlayerPrefs.SetInt(path, isMute ? 0 : 1); // 음소거 상태 저장
    }

    private void ApplySoundSetting(string volumeKey, string muteKey)
    {
        float volume = 1;
        int isMute = 1; // 음소거 상태 (1: 음소거 해제, 0: 음소거)

        if(PlayerPrefs.HasKey(muteKey))
        {
            isMute = PlayerPrefs.GetInt(muteKey);
        }

        if(PlayerPrefs.HasKey(volumeKey))
        {
            volume = PlayerPrefs.GetFloat(volumeKey);
        }

        volume *= isMute; // 음소거 상태에 따라 볼륨 조정

        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;        
        audioMixer.SetFloat(volumeKey, dB); // 볼륨을 데시벨로 변환하여 오디오 믹서에 설정
    }

    private void OnClickExitToLobbyButton()
    {
        UIManager.Instance.ShowConfirmPopup(
            "모든 정보를 잃고 로비씬으로 돌아갑니다",
            onConfirm: () =>
            {
                LoadingSceneController.LoadScene("LobbyScene");
            },
            onCancel: () => {}
        );
    }

    private void OnClickExitToGameButton()
    {
        GameManager.Instance.setState(GameState.Play); // 게임 상태를 Playing으로 변경
        UIManager.Instance.Hide<OptionBoard>(); // 옵션 UI 비활성화
    }
}
