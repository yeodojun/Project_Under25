using UnityEngine;
using UnityEngine.UI;

public class BossGaugeManager : MonoBehaviour
{ 
    public static BossGaugeManager Instance;

    [Header("Gauge Settings")]
    public Slider gaugeSlider;       // 슬라이더 오브젝트 (Inspector에서 할당)
    public float maxGauge = 300f;      // 최대 게이지 (보스 체력)
    private float currentGauge = 0f;   // 현재 게이지 값

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 씬 전환 시 유지할 경우 아래 주석 해제
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gaugeSlider != null)
        {
            // 처음에는 게이지를 숨겨둠
            gaugeSlider.gameObject.SetActive(false);

            gaugeSlider.minValue = 0;
            gaugeSlider.maxValue = maxGauge;
            gaugeSlider.value = currentGauge;
            gaugeSlider.value = 0;      // 시작 시 0으로 설정
            Image fillImage = gaugeSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.red;
            }
        }
    }
    public void ShowGauge()
    {
        if (gaugeSlider != null)
        {
            gaugeSlider.gameObject.SetActive(true);
        }
    }

    // 웨이브가 끝날 때마다 게이지를 증가시키는 메서드
    public void AddGauge(float amount)
    {
        currentGauge += amount;
        if (currentGauge > maxGauge)
            currentGauge = maxGauge;
        if (gaugeSlider != null)
            gaugeSlider.value = currentGauge;
        Debug.Log("게이지 증가: " + currentGauge);
    }

    // 보스가 총알에 맞을 때 데미지만큼 게이지(보스 체력)를 감소시키는 메서드
    public void ReduceGauge(float amount)
    {
        currentGauge -= amount;
        if (currentGauge < 0)
            currentGauge = 0;
        if (gaugeSlider != null)
            gaugeSlider.value = currentGauge;
    }

    // 현재 게이지 값을 반환 (보스 체력으로 사용)
    public float GetGaugeValue()
    {
        return currentGauge;
    }

    // 게이지를 최대치로 채웁니다 (예: 24웨이브가 끝났을 때 호출)
    public void FillGauge()
    {
        currentGauge = maxGauge;
        if (gaugeSlider != null)
            gaugeSlider.value = currentGauge;
    }
}
