using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 方向控制器 - 管理发球机的方向滑块
/// </summary>
public class DirectionController : MonoBehaviour
{
    [Header("UI组件")]
    public Slider directionSlider;
    public TextMeshProUGUI directionText;

    [Header("控制对象")]
    public BallLauncher ballLauncher;

    [Header("方向设置")]
    [Range(-45f, 45f)]
    public float currentDirection = 0f;
    public float minDirection = -45f;  // 左转45度
    public float maxDirection = 45f;   // 右转45度

    void Start()
    {
        InitializeUI();
    }

    void InitializeUI()
    {
        // 自动找到组件（如果没有手动分配）
        if (directionSlider == null)
            directionSlider = GameObject.Find("DirectionSlider")?.GetComponent<Slider>();

        if (directionText == null)
            directionText = GameObject.Find("DirectionText")?.GetComponent<TextMeshProUGUI>();

        if (ballLauncher == null)
            ballLauncher = FindObjectOfType<BallLauncher>();

        // 配置滑块
        if (directionSlider != null)
        {
            directionSlider.minValue = minDirection;
            directionSlider.maxValue = maxDirection;
            directionSlider.value = currentDirection;
            directionSlider.onValueChanged.AddListener(OnDirectionChanged);

            Debug.Log("DirectionController: DirectionSlider已初始化");
        }
        else
        {
            Debug.LogError("DirectionController: 找不到DirectionSlider!");
        }

        // 更新UI显示
        UpdateDirectionText();
    }

    /// <summary>
    /// 滑块值改变时的回调
    /// </summary>
    public void OnDirectionChanged(float value)
    {
        currentDirection = value;

        // 通知BallLauncher更新方向
        if (ballLauncher != null)
        {
            // 假设BallLauncher有一个SetDirection方法
            ballLauncher.SendMessage("SetDirection", currentDirection, SendMessageOptions.DontRequireReceiver);
        }

        UpdateDirectionText();

        Debug.Log($"DirectionController: 方向已改变到 {currentDirection:F1}°");
    }

    /// <summary>
    /// 更新方向文本显示
    /// </summary>
    void UpdateDirectionText()
    {
        if (directionText != null)
        {
            string directionDesc = "";
            if (currentDirection < -5f)
                directionDesc = " (Left)";
            else if (currentDirection > 5f)
                directionDesc = " (Right)";
            else
                directionDesc = " (Center)";

            directionText.text = $"Direction: {currentDirection:F1}°{directionDesc}";
        }
    }

    /// <summary>
    /// 通过代码设置方向
    /// </summary>
    public void SetDirection(float direction)
    {
        currentDirection = Mathf.Clamp(direction, minDirection, maxDirection);

        if (directionSlider != null)
            directionSlider.value = currentDirection;

        UpdateDirectionText();
    }

    /// <summary>
    /// 获取当前方向角度
    /// </summary>
    public float GetDirection()
    {
        return currentDirection;
    }

    void Update()
    {
        // 键盘快捷键控制
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetDirection(currentDirection - 10f);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetDirection(currentDirection + 10f);
        }
    }
}