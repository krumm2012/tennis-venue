using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI可见性测试脚本
/// 用于检查DirectionSlider和相关UI元素是否正确显示
/// </summary>
public class UIVisibilityTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== UI可见性测试开始 ===");
        
        // 检查Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"Canvas找到: {canvas.name}");
            Debug.Log($"Canvas渲染模式: {canvas.renderMode}");
            Debug.Log($"Canvas位置: {canvas.transform.position}");
            Debug.Log($"Canvas激活状态: {canvas.gameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("Canvas未找到!");
        }
        
        // 检查所有Slider
        Slider[] sliders = FindObjectsOfType<Slider>();
        Debug.Log($"找到 {sliders.Length} 个Slider:");
        
        foreach (Slider slider in sliders)
        {
            Debug.Log($"- Slider: {slider.name}");
            Debug.Log($"  位置: {slider.transform.position}");
            Debug.Log($"  本地位置: {slider.transform.localPosition}");
            Debug.Log($"  激活状态: {slider.gameObject.activeInHierarchy}");
            Debug.Log($"  图层: {slider.gameObject.layer}");
            Debug.Log($"  值范围: {slider.minValue} - {slider.maxValue}");
            Debug.Log($"  当前值: {slider.value}");
        }
        
        // 检查所有TextMeshPro组件
        TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>();
        Debug.Log($"找到 {texts.Length} 个TextMeshProUGUI:");
        
        foreach (TextMeshProUGUI text in texts)
        {
            Debug.Log($"- Text: {text.name}");
            Debug.Log($"  位置: {text.transform.position}");
            Debug.Log($"  本地位置: {text.transform.localPosition}");
            Debug.Log($"  激活状态: {text.gameObject.activeInHierarchy}");
            Debug.Log($"  文本内容: '{text.text}'");
        }
        
        Debug.Log("=== UI可见性测试完成 ===");
    }
    
    void Update()
    {
        // 每5秒输出一次UI状态
        if (Time.time % 5f < 0.1f)
        {
            CheckDirectionSlider();
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
                Debug.Log($"DirectionSlider状态: 激活={directionSlider.activeInHierarchy}, 值={slider.value:F1}");
            }
        }
        else
        {
            Debug.LogWarning("DirectionSlider未找到!");
        }
    }
}