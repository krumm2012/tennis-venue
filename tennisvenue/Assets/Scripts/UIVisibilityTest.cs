using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI可见性测试脚本
/// 用于检查DirectionSlider和相关UI元素是否正确显示
/// </summary>
public class UIVisibilityTest : MonoBehaviour
{
    [Header("日志控制")]
    public bool enablePeriodicLogging = false;  // 关闭定期日志输出
    public bool enableChangeLogging = true;     // 启用变化日志
    public float logInterval = 10f;             // 日志间隔（秒）

    private float lastLogTime = 0f;
    private float lastSliderValue = float.MinValue;
    private bool lastSliderActive = false;

    void Start()
    {
        Debug.Log("=== UI可见性测试开始 ===");

        // 检查Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"✅ Canvas找到: {canvas.name} (模式: {canvas.renderMode})");
        }
        else
        {
            Debug.LogError("❌ Canvas未找到!");
        }

        // 检查所有Slider (简化输出)
        Slider[] sliders = FindObjectsOfType<Slider>();
        Debug.Log($"📊 找到 {sliders.Length} 个Slider组件");

        foreach (Slider slider in sliders)
        {
            if (slider.name.Contains("Direction"))
            {
                Debug.Log($"🎯 DirectionSlider: 值={slider.value:F1}, 范围=[{slider.minValue:F1}, {slider.maxValue:F1}], 激活={slider.gameObject.activeInHierarchy}");
                lastSliderValue = slider.value;
                lastSliderActive = slider.gameObject.activeInHierarchy;
            }
        }

        // 简化TextMeshPro检查
        TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>();
        int activeTexts = 0;
        foreach (var text in texts)
        {
            if (text.gameObject.activeInHierarchy) activeTexts++;
        }
        Debug.Log($"📝 找到 {texts.Length} 个文本组件 (激活: {activeTexts})");

        Debug.Log("=== UI可见性测试完成 ===");
        Debug.Log($"💡 提示: enablePeriodicLogging={enablePeriodicLogging}, enableChangeLogging={enableChangeLogging}");
    }

    void Update()
    {
        // 只有在启用定期日志时才输出
        if (enablePeriodicLogging && Time.time - lastLogTime >= logInterval)
        {
            CheckDirectionSlider();
            lastLogTime = Time.time;
        }

        // 检查变化（如果启用）
        if (enableChangeLogging)
        {
            CheckSliderChanges();
        }
    }

    void CheckDirectionSlider()
    {
        GameObject directionSlider = GameObject.Find("DirectionSlider");
        if (directionSlider != null)
        {
            Slider slider = directionSlider.GetComponent<Slider>();
            if (slider != null)
            {
                Debug.Log($"🔄 [定期检查] DirectionSlider状态: 激活={directionSlider.activeInHierarchy}, 值={slider.value:F1}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ DirectionSlider未找到!");
        }
    }

    /// <summary>
    /// 检查滑块值变化，只在变化时输出日志
    /// </summary>
    void CheckSliderChanges()
    {
        GameObject directionSlider = GameObject.Find("DirectionSlider");
        if (directionSlider != null)
        {
            Slider slider = directionSlider.GetComponent<Slider>();
            if (slider != null)
            {
                bool currentActive = directionSlider.activeInHierarchy;
                float currentValue = slider.value;

                // 只在状态或值发生显著变化时输出日志
                bool valueChanged = Mathf.Abs(currentValue - lastSliderValue) > 0.1f;
                bool activeChanged = currentActive != lastSliderActive;

                if (valueChanged || activeChanged)
                {
                    if (valueChanged)
                    {
                        Debug.Log($"🎯 DirectionSlider值变化: {lastSliderValue:F1} → {currentValue:F1}");
                    }

                    if (activeChanged)
                    {
                        Debug.Log($"🔄 DirectionSlider激活状态变化: {lastSliderActive} → {currentActive}");
                    }

                    lastSliderValue = currentValue;
                    lastSliderActive = currentActive;
                }
            }
        }
    }

    /// <summary>
    /// 手动触发UI状态检查（供外部调用）
    /// </summary>
    public void ManualCheck()
    {
        Debug.Log("🔍 手动UI状态检查:");
        CheckDirectionSlider();
    }

    void OnDestroy()
    {
        Debug.Log("🔚 UI可见性测试脚本已销毁");
    }
}