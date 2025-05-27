using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用于测试滑块功能的测试脚本
/// </summary>
public class SliderTester : MonoBehaviour
{
    [Header("测试组件")]
    public Slider testSlider;
    public BallLauncher ballLauncher;
    
    void Start()
    {
        // 寻找AngleSlider
        if (testSlider == null)
        {
            testSlider = GameObject.Find("AngleSlider")?.GetComponent<Slider>();
        }
        
        // 寻找BallLauncher
        if (ballLauncher == null)
        {
            ballLauncher = GameObject.Find("BallLauncher")?.GetComponent<BallLauncher>();
        }
        
        // 测试滑块值变化
        if (testSlider != null)
        {
            testSlider.onValueChanged.AddListener(OnSliderChanged);
            Debug.Log($"AngleSlider当前值: {testSlider.value}");
            Debug.Log($"AngleSlider范围: {testSlider.minValue} - {testSlider.maxValue}");
        }
        else
        {
            Debug.LogError("找不到AngleSlider!");
        }
        
        if (ballLauncher != null)
        {
            Debug.Log("BallLauncher脚本找到了！");
        }
        else
        {
            Debug.LogError("找不到BallLauncher脚本!");
        }
    }
    
    void OnSliderChanged(float value)
    {
        Debug.Log($"AngleSlider值改变为: {value}");
        if (ballLauncher != null)
        {
            Transform launcher = ballLauncher.transform;
            Debug.Log($"发球机当前旋转: {launcher.rotation.eulerAngles}");
            Debug.Log($"发球机前向量: {launcher.forward}");
        }
    }
    
    void Update()
    {
        // 按T键测试滑块
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (testSlider != null)
            {
                float newValue = Random.Range(testSlider.minValue, testSlider.maxValue);
                testSlider.value = newValue;
                Debug.Log($"测试：设置滑块值为 {newValue}");
            }
        }
    }
}