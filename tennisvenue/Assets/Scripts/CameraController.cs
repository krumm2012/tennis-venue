using UnityEngine;

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

    private Vector3 lastMousePosition;
    private bool isMouseDragging = false;
    private CameraPreset[] cameraPresets;
    private int currentPresetIndex = 0;

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
            new CameraPreset { name = "全景视角", position = new Vector3(0f, 6f, -8f), rotation = new Vector3(35f, 0f, 0f), fieldOfView = 45f }
        };
    }

    void HandleKeyboardInput()
    {
        // 字母键切换预设视角
        if (Input.GetKeyDown(KeyCode.R)) SetCameraPreset(0);
        if (Input.GetKeyDown(KeyCode.T)) SetCameraPreset(1);
        if (Input.GetKeyDown(KeyCode.F)) SetCameraPreset(2);
        if (Input.GetKeyDown(KeyCode.C)) SetCameraPreset(3);
        if (Input.GetKeyDown(KeyCode.V)) SetCameraPreset(4);

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
    }

    void OnGUI()
    {
        if (enableKeyboardControl)
        {
            GUILayout.BeginArea(new Rect(10, 10, 350, 200));
            GUILayout.Box("摄像机控制说明:");
            GUILayout.Label("视角切换:");
            GUILayout.Label("  R键: 默认视角   T键: 俯视角度");
            GUILayout.Label("  F键: 侧面视角   C键: 近距离观察");
            GUILayout.Label("  V键: 全景视角");
            GUILayout.Label("移动控制:");
            GUILayout.Label("  WASD: 移动摄像机   QE: 上下移动");
            GUILayout.Label("  鼠标滚轮: 前后移动");
            GUILayout.Label("  右键拖拽: 旋转视角");
            GUILayout.Label("当前视角: " + cameraPresets[currentPresetIndex].name);
            GUILayout.EndArea();
        }
    }
}