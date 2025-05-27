using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 落点调试器 - 专门用于诊断落地点标识不显示的问题
/// </summary>
public class LandingPointDebugger : MonoBehaviour
{
    [Header("调试设置")]
    [Tooltip("是否启用详细调试信息")]
    public bool enableDetailedDebug = true;

    [Tooltip("是否强制创建测试标记")]
    public bool forceCreateTestMarker = false;

    [Tooltip("测试标记位置")]
    public Vector3 testMarkerPosition = new Vector3(0, 0.1f, 5);

    private LandingPointTracker landingTracker;
    private BallLauncher ballLauncher;

    void Start()
    {
        landingTracker = FindObjectOfType<LandingPointTracker>();
        ballLauncher = FindObjectOfType<BallLauncher>();

        Debug.Log("=== 落点调试器启动 ===");
        DiagnoseSystem();

        if (forceCreateTestMarker)
        {
            CreateTestMarker();
        }
    }

    /// <summary>
    /// 诊断系统状态
    /// </summary>
    void DiagnoseSystem()
    {
        Debug.Log("--- 系统组件检查 ---");

        // 检查核心组件
        Debug.Log($"LandingPointTracker: {(landingTracker != null ? "✅ 存在" : "❌ 缺失")}");
        Debug.Log($"BallLauncher: {(ballLauncher != null ? "✅ 存在" : "❌ 缺失")}");

        // 检查地面对象
        CheckGroundObjects();

        // 检查现有网球
        CheckExistingBalls();

        // 检查标记创建设置
        if (landingTracker != null)
        {
            Debug.Log($"创建落点标记: {(landingTracker.createLandingMarkers ? "✅ 启用" : "❌ 禁用")}");
            Debug.Log($"标记生存时间: {landingTracker.markerLifetime}秒");
            Debug.Log($"地面高度阈值: {landingTracker.groundHeightThreshold}m");
            Debug.Log($"速度阈值: {landingTracker.velocityThreshold}m/s");
        }
    }

    /// <summary>
    /// 检查地面对象
    /// </summary>
    void CheckGroundObjects()
    {
        Debug.Log("--- 地面对象检查 ---");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        bool foundFloor = false;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Floor") || obj.name.Contains("Ground"))
            {
                foundFloor = true;
                Collider collider = obj.GetComponent<Collider>();
                Debug.Log($"地面对象: {obj.name}");
                Debug.Log($"  位置: {obj.transform.position}");
                Debug.Log($"  碰撞器: {(collider != null ? "✅ 存在" : "❌ 缺失")}");
                if (collider != null)
                {
                    Debug.Log($"  碰撞器类型: {collider.GetType().Name}");
                    Debug.Log($"  是否启用: {collider.enabled}");
                }
            }
        }

        if (!foundFloor)
        {
            Debug.LogWarning("❌ 未找到地面对象（名称包含'Floor'或'Ground'）");
        }
    }

    /// <summary>
    /// 检查现有网球
    /// </summary>
    void CheckExistingBalls()
    {
        Debug.Log("--- 网球对象检查 ---");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int ballCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                ballCount++;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                Debug.Log($"网球: {obj.name}");
                Debug.Log($"  位置: {obj.transform.position}");
                Debug.Log($"  Rigidbody: {(rb != null ? "✅ 存在" : "❌ 缺失")}");
                if (rb != null)
                {
                    Debug.Log($"  速度: {rb.velocity.magnitude:F2}m/s");
                    Debug.Log($"  是否运动: {(rb.velocity.magnitude > 0.1f ? "是" : "否")}");
                }
            }
        }

        Debug.Log($"总网球数量: {ballCount}");
    }

    /// <summary>
    /// 创建测试标记
    /// </summary>
    void CreateTestMarker()
    {
        Debug.Log("--- 创建测试标记 ---");

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "TestLandingMarker";
        marker.transform.position = testMarkerPosition;
        marker.transform.localScale = Vector3.one * 0.2f; // 稍大一点便于观察

        // 设置明亮的红色材质
        Renderer renderer = marker.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material markerMaterial = new Material(Shader.Find("Standard"));
            markerMaterial.color = Color.red;
            markerMaterial.SetFloat("_Metallic", 0.0f);
            markerMaterial.SetFloat("_Smoothness", 0.8f);
            markerMaterial.EnableKeyword("_EMISSION");
            markerMaterial.SetColor("_EmissionColor", Color.red * 0.5f); // 发光效果
            renderer.material = markerMaterial;
        }

        // 移除碰撞器
        Collider collider = marker.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        Debug.Log($"✅ 测试标记已创建于位置: {testMarkerPosition}");

        // 10秒后销毁
        Destroy(marker, 10f);
    }

    /// <summary>
    /// 强制触发落地检测
    /// </summary>
    void ForceTestLanding()
    {
        if (landingTracker == null) return;

        Debug.Log("--- 强制测试落地检测 ---");

        // 查找网球
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        bool foundBall = false;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                foundBall = true;
                Vector3 ballPos = obj.transform.position;
                Rigidbody rb = obj.GetComponent<Rigidbody>();

                Debug.Log($"找到网球: {obj.name}");
                Debug.Log($"  网球位置: {ballPos}");
                if (rb != null)
                {
                    Debug.Log($"  网球速度: {rb.velocity.magnitude:F2}m/s");
                }

                // 调用LandingPointTracker的强制检查方法
                landingTracker.ForceCheckBallLanding(obj);

                // 如果检查后仍未落地，手动记录落点
                Vector3 groundPos = new Vector3(ballPos.x, 0.05f, ballPos.z);
                Debug.Log($"手动记录落点: {groundPos}");
                landingTracker.ManualRecordLandingPoint(groundPos, obj);

                break; // 只处理第一个找到的网球
            }
        }

        if (!foundBall)
        {
            Debug.LogWarning("❌ 场景中没有找到网球对象");
            // 创建一个测试落点
            Vector3 testPos = new Vector3(0, 0.05f, 3);
            Debug.Log($"创建测试落点: {testPos}");
            landingTracker.ManualRecordLandingPoint(testPos, null);
        }
    }

    /// <summary>
    /// 检查已存在的落点标记
    /// </summary>
    void CheckExistingMarkers()
    {
        Debug.Log("--- 检查现有标记 ---");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int markerCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("LandingMarker") || obj.name.Contains("TestLandingMarker"))
            {
                markerCount++;
                Debug.Log($"找到标记: {obj.name} 位置: {obj.transform.position}");
            }
        }

        Debug.Log($"总标记数量: {markerCount}");

        if (markerCount == 0)
        {
            Debug.LogWarning("❌ 场景中没有找到任何落点标记");
        }
    }

    void Update()
    {
        if (enableDetailedDebug)
        {
            // 监控网球状态
            MonitorBalls();
        }

        // 调试快捷键
        if (Input.GetKeyDown(KeyCode.F6))
        {
            DiagnoseSystem();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            CreateTestMarker();
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            ForceTestLanding();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            CheckExistingMarkers();
        }
    }

    /// <summary>
    /// 监控网球状态
    /// </summary>
    void MonitorBalls()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 pos = obj.transform.position;
                    float speed = rb.velocity.magnitude;

                    // 检查是否满足落地条件
                    bool nearGround = pos.y <= (landingTracker?.groundHeightThreshold ?? 0.2f);
                    bool slowSpeed = speed <= (landingTracker?.velocityThreshold ?? 1.0f);

                    if (nearGround && slowSpeed)
                    {
                        Debug.Log($"⚠️ 网球可能已落地但未检测到: {obj.name}");
                        Debug.Log($"  位置: {pos}, 速度: {speed:F2}m/s");
                        Debug.Log($"  满足高度条件: {nearGround}, 满足速度条件: {slowSpeed}");

                        // 立即触发强制检测
                        if (landingTracker != null)
                        {
                            Debug.Log($"🔧 触发强制落地检测...");
                            landingTracker.ForceCheckBallLanding(obj);
                        }
                    }
                }
            }
        }
    }
}