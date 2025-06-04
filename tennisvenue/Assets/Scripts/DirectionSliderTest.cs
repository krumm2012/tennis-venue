using UnityEngine;
using UnityEngine.UI;

public class DirectionSliderTest : MonoBehaviour
{
    [Header("日志控制")]
    public bool enableValueChangeLogging = false;  // 默认关闭值变化日志
    public float logThreshold = 0.5f;              // 值变化阈值

    public Slider directionSlider;

    private float lastLoggedValue = float.MinValue;

    void Start()
    {
        if (directionSlider != null)
        {
            Debug.Log("✅ DirectionSliderTest: Slider初始化成功!");
            directionSlider.onValueChanged.AddListener(OnValueChanged);

            if (!enableValueChangeLogging)
            {
                Debug.Log("💡 DirectionSliderTest: 值变化日志已关闭 (enableValueChangeLogging=false)");
            }
        }
        else
        {
            Debug.LogError("❌ DirectionSliderTest: Slider为null!");
        }
    }

    void OnValueChanged(float value)
    {
        // 只有在启用日志且值变化足够大时才输出
        if (enableValueChangeLogging && Mathf.Abs(value - lastLoggedValue) >= logThreshold)
        {
            Debug.Log($"🎯 DirectionSliderTest: 值变化 {lastLoggedValue:F1} → {value:F1}");
            lastLoggedValue = value;
        }
    }

    /// <summary>
    /// 手动启用值变化日志（用于调试）
    /// </summary>
    public void EnableValueLogging()
    {
        enableValueChangeLogging = true;
        Debug.Log("🔊 DirectionSliderTest: 值变化日志已启用");
    }

    /// <summary>
    /// 手动禁用值变化日志
    /// </summary>
    public void DisableValueLogging()
    {
        enableValueChangeLogging = false;
        Debug.Log("🔇 DirectionSliderTest: 值变化日志已禁用");
    }
}