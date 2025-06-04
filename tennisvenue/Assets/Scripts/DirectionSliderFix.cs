using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 专门修复DirectionSlider从SpeedSlider复制后的参数问题
/// </summary>
public class DirectionSliderFix : MonoBehaviour
{
    void Start()
    {
        Debug.Log("DirectionSliderFix: 开始修复");
        Invoke("FixDirectionSlider", 0.1f); // 延迟执行确保所有对象已加载
    }

    void FixDirectionSlider()
    {
        Debug.Log("=== 开始修复DirectionSlider参数 ===");

        // 查找DirectionSlider
        Slider directionSlider = GameObject.Find("DirectionSlider")?.GetComponent<Slider>();
        if (directionSlider == null)
        {
            Debug.LogError("DirectionSlider未找到!");
            return;
        }

        Debug.Log($"找到DirectionSlider，当前范围: {directionSlider.minValue} 到 {directionSlider.maxValue}，值: {directionSlider.value}");

        // 修复Slider参数（从Speed范围改为Direction范围）
        directionSlider.minValue = -15f;  // 左转15度
        directionSlider.maxValue = 15f;   // 右转15度
        directionSlider.value = 0f;       // 默认正前方
        directionSlider.wholeNumbers = false;
        directionSlider.interactable = true;

        Debug.Log("DirectionSlider参数已修复: 范围 -15° 到 +15°");

        // 修复Fill颜色（区别于SpeedSlider）
        FixSliderColors(directionSlider);

        // 确保BallLauncher能找到这个Slider
        ConnectToBallLauncher(directionSlider);

        // 修复DirectionText
        FixDirectionText();

        Debug.Log("=== DirectionSlider修复完成 ===");
    }

    void FixSliderColors(Slider slider)
    {
        // 获取所有Image组件
        Image[] images = slider.GetComponentsInChildren<Image>();

        foreach (Image img in images)
        {
            if (img.name.Contains("Background"))
            {
                img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                Debug.Log("设置Background颜色");
            }
            else if (img.name.Contains("Fill"))
            {
                img.color = new Color(0.2f, 0.8f, 0.2f, 0.8f); // 绿色
                Debug.Log("设置Fill颜色为绿色");
            }
            else if (img.name.Contains("Handle"))
            {
                img.color = new Color(0.8f, 0.8f, 0.8f, 0.9f);
                Debug.Log("设置Handle颜色");
            }
        }
    }

    void ConnectToBallLauncher(Slider directionSlider)
    {
        BallLauncher ballLauncher = FindObjectOfType<BallLauncher>();
        if (ballLauncher != null)
        {
            // 通过SendMessage的方式设置directionSlider
            ballLauncher.SendMessage("SetDirectionSlider", directionSlider, SendMessageOptions.DontRequireReceiver);
            Debug.Log("已连接DirectionSlider到BallLauncher");
        }
        else
        {
            Debug.LogWarning("未找到BallLauncher");
        }
    }

    void FixDirectionText()
    {
        TextMeshProUGUI directionText = GameObject.Find("DirectionText")?.GetComponent<TextMeshProUGUI>();
        if (directionText != null)
        {
            directionText.text = "Direction: 0.0°";
            directionText.color = Color.white;
            directionText.fontSize = 14;
            Debug.Log("DirectionText已修复");
        }
        else
        {
            Debug.LogWarning("DirectionText未找到");
        }
    }
}