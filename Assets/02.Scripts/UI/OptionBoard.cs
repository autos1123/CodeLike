using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionBoard : UIBase
{
    public override string UIName => "OptionBoard";
    private readonly string resolutionDataKey = "ResolutionData"; // 해상도 데이터 키
    private readonly string screenModeDataKey = "ScreenModeData"; // 해상도 데이터 키

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

    private void Awake()
    {
        resolutionData = new ResolutionData(); // 해상도 데이터 초기화
        resolutions = resolutionData.resolutions.ToArray(); // 해상도 리스트를 배열로 변환
        InitResolutionDropdown();
        InitFullScreenModeDropDown();
    }

    public override void Open()
    {
        base.Open();

        resolutionDropdown.onValueChanged.AddListener(ResolutionDropDownOptionChange); // 해상도 드롭다운 값 변경 이벤트 등록
        fullscreenDropdown.onValueChanged.AddListener(ScreenModeDropDownOptionChange); // 화면 모드 드롭다운 값 변경 이벤트 등록

        exitToGameButton.onClick.AddListener(OnClickExitToGameButton); // 게임으로 돌아가는 버튼 클릭 이벤트 등록
        exitToLobbyButton.onClick.AddListener(OnClickExitToLobbyButton); // 로비로 돌아가는 버튼 클릭 이벤트 등록

        // 로비로 돌아가는 버튼은 게임씬에서만 활성화
        if(SceneManager.GetActiveScene().name != gameSceneName)
        {
            exitToLobbyButton.gameObject.SetActive(true);
        }
    }

    public override void Close()
    {
        base.Close();

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
                }
            }
        }

        int idx = PlayerPrefs.GetInt(resolutionDataKey);
        // 현재 해상도 인덱스를 저장 및 Dropdown UI에 반영
        resolutionDropdown.value = idx;
        ResolutionDropDownOptionChange(idx);

        resolutionDropdown.RefreshShownValue();
    }

    // Dropdown UI 값 변경 이벤트
    private void ResolutionDropDownOptionChange(int x)
    {
        Screen.SetResolution(resolutions[x].width,
            resolutions[x].height,
            GetScreenModeToIndex(PlayerPrefs.GetInt(screenModeDataKey)),
            resolutions[x].refreshRateRatio);

        PlayerPrefs.SetInt(resolutionDataKey, x);
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

    private void OnClickExitToLobbyButton()
    {
        // TODO : 로비로 돌아가는 로직 구현
        Debug.Log("로비로 돌아갑니다."); // 로비로 돌아가는 버튼 클릭 이벤트
    }

    private void OnClickExitToGameButton()
    {
        Debug.Log("게임으로 돌아갑니다."); // 게임으로 돌아가는 버튼 클릭 이벤트
        UIManager.Instance.Hide<OptionBoard>(); // 옵션 UI 비활성화
    }
}
