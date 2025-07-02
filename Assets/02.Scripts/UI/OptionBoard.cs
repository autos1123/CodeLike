using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionBoard : UIBase
{
    public override string UIName => "OptionBoard";
    private readonly string resolutionDataKey = "ResolutionData"; // 해상도 데이터 키

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

    private void Start()
    {
        resolutionData = new ResolutionData(); // 해상도 데이터 초기화
        resolutions = resolutionData.resolutions.ToArray(); // 해상도 리스트를 배열로 변환
        InitResolutionDropdown();
    }

    private void OnEnable()
    {
        // 로비로 돌아가는 버튼은 게임씬에서만 활성화
        if(SceneManager.GetActiveScene().name != gameSceneName)
        {
            exitToLobbyButton.gameObject.SetActive(true);
        }

        resolutionDropdown.onValueChanged.AddListener(DropDownOptionChange); // 해상도 드롭다운 값 변경 이벤트 등록
    }

    private void OnDisable()
    {
        resolutionDropdown.onValueChanged.RemoveAllListeners(); // 해상도 드롭다운 값 변경 이벤트 제거
    }

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

        // 현재 해상도 인덱스를 저장 및 Dropdown UI에 반영
        resolutionDropdown.value = PlayerPrefs.GetInt(resolutionDataKey);

        resolutionDropdown.RefreshShownValue();
    }

    // Dropdown UI 값 변경 이벤트
    private void DropDownOptionChange(int x)
    {
        Screen.SetResolution(resolutions[x].width,
            resolutions[x].height,
            FullScreenMode.FullScreenWindow,
            resolutions[x].refreshRateRatio);

        PlayerPrefs.SetInt(resolutionDataKey, x);
    }
}
