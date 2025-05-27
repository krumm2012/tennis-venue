using UnityEngine;
using UnityEngine.UI;

public class QuickUIFix : MonoBehaviour
{
    void Start()
    {
        FixHandleColors();
    }
    
    void FixHandleColors()
    {
        // 查找所有Handle并修复颜色
        Image[] allImages = FindObjectsOfType<Image>();
        
        foreach (Image img in allImages)
        {
            if (img.name.Contains("Handle"))
            {
                // 设置Handle为半透明灰色
                img.color = new Color(0.7f, 0.7f, 0.7f, 0.9f);
                Debug.Log("修复Handle颜色: " + img.name);
            }
        }
        
        // 确保所有Slider可交互
        Slider[] allSliders = FindObjectsOfType<Slider>();
        foreach (Slider slider in allSliders)
        {
            slider.interactable = true;
            Debug.Log("启用Slider交互: " + slider.name);
        }
    }
}