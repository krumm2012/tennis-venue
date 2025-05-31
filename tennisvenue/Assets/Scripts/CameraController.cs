using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Header("摄像机设置")]
    public Camera mainCamera;

    [Header("运行时控制")]
    public bool enableKeyboardControl = true;
    public bool enableMouseControl = true;
    public float mouseSensitivity = 2f;
    public float moveSpeed = 5f;
    public float scrollSpeed = 2f;

    [Header("限制")]
    public float minY = 0.5f;
    public float maxY = 10f;

    [Header("UI控制")]
    public Button viewSwitchButton;
    public Text viewSwitchButtonText;

    private Vector3 lastMousePosition;
    private bool isMouseDragging = false;
    private CameraPreset[] cameraPresets;
    private int currentPresetIndex = 0;

    /// <summary>
    /// 获取当前视角预设索引的公共属性
    /// </summary>
    public int CurrentPresetIndex => currentPresetIndex;

    [System.Serializable]
    public struct CameraPreset
    {
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public float fieldOfView;
    }

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        InitializeCameraPresets();
        SetupUI();
    }

    void Start()
    {
        if (cameraPresets.Length > 0)
            SetCameraPreset(0);
    }

    void Update()
    {
        if (enableKeyboardControl)
            HandleKeyboardInput();
        if (enableMouseControl)
            HandleMouseInput();
    }

    void InitializeCameraPresets()
    {
        cameraPresets = new CameraPreset[]
        {
            new CameraPreset { name = "默认视角", position = new Vector3(0f, 2.5f, -5f), rotation = new Vector3(18f, 0f, 0f), fieldOfView = 60f },
            new CameraPreset { name = "俯视角度", position = new Vector3(0f, 8f, -2f), rotation = new Vector3(70f, 0f, 0f), fieldOfView = 50f },
            new CameraPreset { name = "侧面视角", position = new Vector3(-8f, 3f, 0f), rotation = new Vector3(15f, 90f, 0f), fieldOfView = 55f },
            new CameraPreset { name = "近距离观察", position = new Vector3(0f, 1.5f, -2f), rotation = new Vector3(5f, 0f, 0f), fieldOfView = 70f },
            new CameraPreset { name = "全景视角", position = new Vector3(0f, 6f, -8f), rotation = new Vector3(35f, 0f, 0f), fieldOfView = 45f },
            new CameraPreset { name = "后场视角", position = new Vector3(0f, 2f, 4.5f), rotation = new Vector3(10f, 180f, 0f), fieldOfView = 65f } // 新增后场视角
        };
    }

    void SetupUI()
    {
        // 如果没有指定UI按钮，尝试自动创建
        if (viewSwitchButton == null)
        {
            CreateViewSwitchButton();
        }

        if (viewSwitchButton != null)
        {
            viewSwitchButton.onClick.AddListener(ToggleSimpleView);
            UpdateButtonText();
        }
    }

    void CreateViewSwitchButton()
    {
        // 查找Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("未找到Canvas，无法创建视角切换按钮");
            return;
        }

        // 创建按钮GameObject
        GameObject buttonObj = new GameObject("ViewSwitchButton");
        buttonObj.transform.SetParent(canvas.transform, false);

        // 添加RectTransform
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-20, -20);
        rectTransform.sizeDelta = new Vector2(120, 40);

        // 添加Image组件
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.3f, 0.8f, 0.8f);

        // 添加Button组件
        viewSwitchButton = buttonObj.AddComponent<Button>();

        // 创建文本子对象
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        viewSwitchButtonText = textObj.AddComponent<Text>();
        viewSwitchButtonText.text = "切换视角";
        viewSwitchButtonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        viewSwitchButtonText.fontSize = 14;
        viewSwitchButtonText.color = Color.white;
        viewSwitchButtonText.alignment = TextAnchor.MiddleCenter;

        Debug.Log("✅ 已创建视角切换按钮");
    }

    void HandleKeyboardInput()
    {
        // 字母键切换预设视角
        if (Input.GetKeyDown(KeyCode.R)) SetCameraPreset(0);
        if (Input.GetKeyDown(KeyCode.T)) SetCameraPreset(1);
        if (Input.GetKeyDown(KeyCode.F)) SetCameraPreset(2);
        if (Input.GetKeyDown(KeyCode.C)) SetCameraPreset(3);
        if (Input.GetKeyDown(KeyCode.V)) SetCameraPreset(4);
        if (Input.GetKeyDown(KeyCode.B)) SetCameraPreset(5); // 新增B键切换到后场视角

        // 数字键1和2用于简化视角切换
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetCameraPreset(0); // 默认视角
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetCameraPreset(5); // 后场视角

        // WASD移动摄像机
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movement += mainCamera.transform.forward;
        if (Input.GetKey(KeyCode.S)) movement -= mainCamera.transform.forward;
        if (Input.GetKey(KeyCode.A)) movement -= mainCamera.transform.right;
        if (Input.GetKey(KeyCode.D)) movement += mainCamera.transform.right;
        if (Input.GetKey(KeyCode.Q)) movement += Vector3.up;
        if (Input.GetKey(KeyCode.E)) movement -= Vector3.up;

        if (movement != Vector3.zero)
        {
            MoveCameraWithLimits(movement * moveSpeed * Time.deltaTime);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 scrollMovement = mainCamera.transform.forward * scroll * scrollSpeed;
            MoveCameraWithLimits(scrollMovement);
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isMouseDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isMouseDragging = false;
        }

        if (isMouseDragging)
        {
            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;
            float yRotation = deltaMousePosition.x * mouseSensitivity * Time.deltaTime;
            mainCamera.transform.Rotate(0, yRotation, 0, Space.World);
            float xRotation = -deltaMousePosition.y * mouseSensitivity * Time.deltaTime;
            mainCamera.transform.Rotate(xRotation, 0, 0, Space.Self);
            lastMousePosition = Input.mousePosition;
        }
    }

    void MoveCameraWithLimits(Vector3 movement)
    {
        Vector3 newPosition = mainCamera.transform.position + movement;
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        mainCamera.transform.position = newPosition;
    }

    public void SetCameraPreset(int presetIndex)
    {
        if (presetIndex < 0 || presetIndex >= cameraPresets.Length)
            return;

        currentPresetIndex = presetIndex;
        CameraPreset preset = cameraPresets[presetIndex];

        mainCamera.transform.position = preset.position;
        mainCamera.transform.rotation = Quaternion.Euler(preset.rotation);
        mainCamera.fieldOfView = preset.fieldOfView;

        Debug.Log("切换到 " + preset.name + " 视角");
        UpdateButtonText();
    }

    /// <summary>
    /// 简化的视角切换功能 - 在默认视角和后场视角之间切换
    /// </summary>
    public void ToggleSimpleView()
    {
        if (currentPresetIndex == 0)
        {
            SetCameraPreset(5); // 切换到后场视角
        }
        else
        {
            SetCameraPreset(0); // 切换到默认视角
        }
    }

    void UpdateButtonText()
    {
        if (viewSwitchButtonText != null)
        {
            if (currentPresetIndex == 0)
            {
                viewSwitchButtonText.text = "后场视角";
            }
            else if (currentPresetIndex == 5)
            {
                viewSwitchButtonText.text = "默认视角";
            }
            else
            {
                viewSwitchButtonText.text = "切换视角";
            }
        }
    }

    void OnGUI()
    {
        if (enableKeyboardControl)
        {
            GUILayout.BeginArea(new Rect(10, 10, 350, 250));
            GUILayout.Box("摄像机控制说明:");
            GUILayout.Label("视角切换:");
            GUILayout.Label("  R键: 默认视角   T键: 俯视角度");
            GUILayout.Label("  F键: 侧面视角   C键: 近距离观察");
            GUILayout.Label("  V键: 全景视角   B键: 后场视角");
            GUILayout.Label("简化切换:");
            GUILayout.Label("  1键: 默认视角   2键: 后场视角");
            GUILayout.Label("  右上角按钮: 快速切换");
            GUILayout.Label("移动控制:");
            GUILayout.Label("  WASD: 移动摄像机   QE: 上下移动");
            GUILayout.Label("  鼠标滚轮: 前后移动");
            GUILayout.Label("  右键拖拽: 旋转视角");
            GUILayout.Label("当前视角: " + cameraPresets[currentPresetIndex].name);
            GUILayout.EndArea();
        }
    }
}