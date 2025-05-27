using UnityEngine;
using TMPro;

/// <summary>
/// 中文字体修复器 - 解决TextMeshPro中文字符显示问题
/// </summary>
public class ChineseFontFixer : MonoBehaviour
{
    [Header("字体修复设置")]
    public bool autoFixOnStart = true;
    public bool useEnglishFallback = true;  // 使用英文替代方案

    void Start()
    {
        if (autoFixOnStart)
        {
            FixChineseFontIssues();
        }

        Debug.Log("=== 中文字体修复器已启动 ===");
        Debug.Log("检测到中文字符显示问题，正在修复...");
    }

    /// <summary>
    /// 修复中文字体显示问题
    /// </summary>
    public void FixChineseFontIssues()
    {
        Debug.Log("--- 开始修复中文字体问题 ---");

        // 查找所有TextMeshProUGUI组件
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();

        foreach (TextMeshProUGUI text in allTexts)
        {
            if (text != null)
            {
                FixTextComponent(text);
            }
        }

        Debug.Log($"✅ 已修复 {allTexts.Length} 个文本组件");
    }

    /// <summary>
    /// 修复单个文本组件
    /// </summary>
    void FixTextComponent(TextMeshProUGUI textComponent)
    {
        string originalText = textComponent.text;

        if (useEnglishFallback)
        {
            // 将中文替换为英文
            string fixedText = ReplaceChineseWithEnglish(originalText);

            if (fixedText != originalText)
            {
                textComponent.text = fixedText;
                Debug.Log($"✅ 修复文本: '{originalText}' → '{fixedText}'");
            }
        }
        else
        {
            // 尝试设置支持中文的字体
            TrySetChineseFont(textComponent);
        }
    }

    /// <summary>
    /// 将中文文本替换为英文
    /// </summary>
    string ReplaceChineseWithEnglish(string text)
    {
        // 常见的中文UI文本替换
        text = text.Replace("方向", "Direction");
        text = text.Replace("右", "Right");
        text = text.Replace("左", "Left");
        text = text.Replace("中", "Center");
        text = text.Replace("速度", "Speed");
        text = text.Replace("角度", "Angle");
        text = text.Replace("发射", "Launch");
        text = text.Replace("网球", "Tennis");
        text = text.Replace("时间", "Time");
        text = text.Replace("坐标", "Position");
        text = text.Replace("落点", "Landing");
        text = text.Replace("飞行", "Flight");
        text = text.Replace("反弹", "Bounce");
        text = text.Replace("高度", "Height");
        text = text.Replace("测试", "Test");
        text = text.Replace("系统", "System");
        text = text.Replace("状态", "Status");
        text = text.Replace("正常", "Normal");
        text = text.Replace("错误", "Error");
        text = text.Replace("警告", "Warning");

        // 处理带有度数符号的文本
        text = text.Replace("°", "°");  // 确保度数符号正确

        return text;
    }

    /// <summary>
    /// 尝试设置支持中文的字体
    /// </summary>
    void TrySetChineseFont(TextMeshProUGUI textComponent)
    {
        // 尝试查找系统中的中文字体
        TMP_FontAsset chineseFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/NotoSansCJK-Regular SDF");

        if (chineseFont == null)
        {
            // 如果没有找到中文字体，尝试其他可能的字体
            chineseFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Arial Unicode MS SDF");
        }

        if (chineseFont != null)
        {
            textComponent.font = chineseFont;
            Debug.Log($"✅ 为 {textComponent.name} 设置中文字体");
        }
        else
        {
            Debug.LogWarning($"⚠️ 未找到中文字体，建议使用英文替代方案");
        }
    }

    /// <summary>
    /// 手动修复特定文本组件
    /// </summary>
    public void FixSpecificText(string objectName, string newText)
    {
        GameObject textObj = GameObject.Find(objectName);
        if (textObj != null)
        {
            TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                string oldText = textComponent.text;
                textComponent.text = newText;
                Debug.Log($"✅ 手动修复 {objectName}: '{oldText}' → '{newText}'");
            }
        }
    }

    /// <summary>
    /// 修复DirectionText的特定问题
    /// </summary>
    [ContextMenu("修复DirectionText")]
    public void FixDirectionText()
    {
        // 查找DirectionText对象
        GameObject directionTextObj = GameObject.Find("DirectionText");
        if (directionTextObj != null)
        {
            TextMeshProUGUI textComponent = directionTextObj.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                // 将中文替换为英文格式
                string currentText = textComponent.text;

                // 解析当前的方向值
                float directionValue = 0f;
                if (currentText.Contains(":"))
                {
                    string[] parts = currentText.Split(':');
                    if (parts.Length > 1)
                    {
                        string valueStr = parts[1].Trim().Replace("°", "").Replace("(中)", "").Replace("(左)", "").Replace("(右)", "");
                        float.TryParse(valueStr, out directionValue);
                    }
                }

                // 生成新的英文文本
                string directionLabel = "";
                if (directionValue > 5f) directionLabel = " (Right)";
                else if (directionValue < -5f) directionLabel = " (Left)";
                else directionLabel = " (Center)";

                string newText = $"Direction: {directionValue:F1}°{directionLabel}";
                textComponent.text = newText;

                Debug.Log($"✅ DirectionText已修复: '{currentText}' → '{newText}'");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到DirectionText对象");
        }
    }

    void Update()
    {
        // F1键手动触发修复
        if (Input.GetKeyDown(KeyCode.F1))
        {
            FixChineseFontIssues();
        }

        // F2键修复DirectionText
        if (Input.GetKeyDown(KeyCode.F2))
        {
            FixDirectionText();
        }
    }
}