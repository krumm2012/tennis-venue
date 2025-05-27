using UnityEngine;
using UnityEngine.UI;

public class CameraUI : MonoBehaviour
{
    [Header("UI引用")]
    public Button[] presetButtons;
    public Slider fovSlider;
    public Text fovText;
    public Text currentViewText;
    
    private CameraController cameraController;
    
    void Start()
    {
        // 查找摄像机控制器
        cameraController = FindObjectOfType<CameraController>();
        
        if (cameraController == null)
        {
            Debug.LogWarning("未找到CameraController组件");
            return;
        }
        
        // 设置FOV滑块
        if (fovSlider != null)
        {
            fovSlider.minValue = 30f;
            fovSlider.maxValue = 90f;
            fovSlider.value = cameraController.mainCamera.fieldOfView;
            fovSlider.onValueChanged.AddListener(OnFOVChanged);
        }
        
        // 设置预设按钮
        SetupPresetButtons();
        
        // 初始化UI文本
        UpdateUI();
    }
    
    void SetupPresetButtons()
    {
        string[] presetNames = { "默认", "俯视", "侧面", "近距", "全景" };
        
        for (int i = 0; i < presetButtons.Length && i < presetNames.Length; i++)
        {
            if (presetButtons[i] != null)
            {
                int index = i; // 闭包变量
                presetButtons[i].onClick.AddListener(() => OnPresetButtonClicked(index));
                
                // 设置按钮文本
                Text buttonText = presetButtons[i].GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = presetNames[i];
                }
            }
        }
    }
    
    void OnPresetButtonClicked(int presetIndex)
    {
        if (cameraController != null)
        {
            cameraController.SetCameraPreset(presetIndex);
            UpdateUI();
        }
    }
    
    void OnFOVChanged(float value)
    {
        if (cameraController != null && cameraController.mainCamera != null)
        {
            cameraController.mainCamera.fieldOfView = value;
            UpdateUI();
        }
    }
    
    void UpdateUI()
    {
        if (fovText != null && cameraController != null)
        {
            fovText.text = $"视野: {cameraController.mainCamera.fieldOfView:F0}°";
        }
        
        if (currentViewText != null)
        {
            Vector3 pos = cameraController.mainCamera.transform.position;
            currentViewText.text = $"位置: ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})";
        }
    }
    
    void Update()
    {
        // 实时更新UI
        UpdateUI();
    }
    
    // 公共方法供按钮调用
    public void SetDefaultView() { OnPresetButtonClicked(0); }
    public void SetTopView() { OnPresetButtonClicked(1); }
    public void SetSideView() { OnPresetButtonClicked(2); }
    public void SetCloseView() { OnPresetButtonClicked(3); }
    public void SetPanoramicView() { OnPresetButtonClicked(4); }
}