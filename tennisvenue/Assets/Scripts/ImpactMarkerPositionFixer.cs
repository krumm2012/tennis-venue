using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ImpactMarker_Ring 位置修复器 - 修复圆环标记与轨迹线落球点坐标不一致的问题
/// </summary>
public class ImpactMarkerPositionFixer : MonoBehaviour
{
    [Header("系统引用")]
    public BallLauncher ballLauncher;
    public BounceImpactMarker bounceImpactMarker;
    public LineRenderer trajectoryLine;

    [Header("修复设置")]
    public bool enablePositionFix = true;
    public bool showDebugInfo = true;
    public float positionTolerance = 0.1f; // 位置误差容忍度

    [Header("预测坐标")]
    public bool showTrajectoryEndPoint = true;
    public GameObject trajectoryEndMarker;

    private Vector3 lastPredictedLandingPoint;
    private Dictionary<GameObject, Vector3> ballExpectedLanding = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        Debug.Log("=== ImpactMarker Position Fixer 已启动 ===");
        Debug.Log("🔧 将修复圆环标记与轨迹线落球点的位置不一致问题");
        Debug.Log("⌨️ 快捷键:");
        Debug.Log("   P键: 显示当前轨迹线终点坐标");
        Debug.Log("   O键: 手动对比圆环和轨迹终点位置");
        Debug.Log("   I键: 切换位置修复功能");
        Debug.Log("   U键: 创建轨迹终点可视化标记");

        FindSystemComponents();
        CreateTrajectoryEndMarker();
    }

    void Update()
    {
        // 实时更新预测落点
        if (enablePositionFix)
        {
            UpdatePredictedLandingPoint();
            UpdateTrajectoryEndMarker();
        }

        // 快捷键控制
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowTrajectoryEndPoint();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ComparePositions();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            TogglePositionFix();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleTrajectoryEndMarker();
        }
    }

    /// <summary>
    /// 查找系统组件
    /// </summary>
    void FindSystemComponents()
    {
        if (ballLauncher == null)
        {
            ballLauncher = FindObjectOfType<BallLauncher>();
            if (ballLauncher != null)
            {
                Debug.Log($"✅ 找到BallLauncher: {ballLauncher.gameObject.name}");
                trajectoryLine = ballLauncher.trajectoryLine;
            }
        }

        if (bounceImpactMarker == null)
        {
            bounceImpactMarker = FindObjectOfType<BounceImpactMarker>();
            if (bounceImpactMarker != null)
            {
                Debug.Log($"✅ 找到BounceImpactMarker: {bounceImpactMarker.gameObject.name}");
            }
        }
    }

    /// <summary>
    /// 更新预测落点坐标
    /// </summary>
    void UpdatePredictedLandingPoint()
    {
        if (trajectoryLine == null || trajectoryLine.positionCount < 2)
            return;

        // 获取轨迹线的最后一个点作为预测落点
        Vector3 trajectoryEnd = trajectoryLine.GetPosition(trajectoryLine.positionCount - 1);

        // 将落点投射到地面(Y=0)
        Vector3 predictedLanding = new Vector3(trajectoryEnd.x, 0.01f, trajectoryEnd.z);

        if (Vector3.Distance(predictedLanding, lastPredictedLandingPoint) > 0.1f)
        {
            lastPredictedLandingPoint = predictedLanding;

            if (showDebugInfo)
            {
                Debug.Log($"🎯 预测落点更新: ({predictedLanding.x:F2}, {predictedLanding.y:F2}, {predictedLanding.z:F2})");
            }
        }
    }

    /// <summary>
    /// 创建轨迹终点可视化标记
    /// </summary>
    void CreateTrajectoryEndMarker()
    {
        if (trajectoryEndMarker == null)
        {
            trajectoryEndMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trajectoryEndMarker.name = "TrajectoryEndMarker";
            trajectoryEndMarker.transform.localScale = new Vector3(0.3f, 0.05f, 0.3f);

            // 设置蓝色材质以区分
            Renderer renderer = trajectoryEndMarker.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.blue;
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.blue * 0.5f);
                renderer.material = mat;
            }

            // 移除碰撞器
            Collider collider = trajectoryEndMarker.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            trajectoryEndMarker.SetActive(showTrajectoryEndPoint);
            Debug.Log("✅ 轨迹终点标记已创建");
        }
    }

    /// <summary>
    /// 更新轨迹终点标记位置
    /// </summary>
    void UpdateTrajectoryEndMarker()
    {
        if (trajectoryEndMarker != null && showTrajectoryEndPoint && lastPredictedLandingPoint != Vector3.zero)
        {
            trajectoryEndMarker.transform.position = lastPredictedLandingPoint + Vector3.up * 0.05f;
            trajectoryEndMarker.SetActive(true);
        }
        else if (trajectoryEndMarker != null)
        {
            trajectoryEndMarker.SetActive(false);
        }
    }

    /// <summary>
    /// 显示当前轨迹线终点坐标
    /// </summary>
    void ShowTrajectoryEndPoint()
    {
        if (trajectoryLine == null || trajectoryLine.positionCount < 2)
        {
            Debug.Log("❌ 轨迹线不存在或点数不足");
            return;
        }

        Debug.Log("=== 轨迹线终点坐标分析 ===");

        // 获取轨迹线所有点
        List<Vector3> trajectoryPoints = new List<Vector3>();
        for (int i = 0; i < trajectoryLine.positionCount; i++)
        {
            trajectoryPoints.Add(trajectoryLine.GetPosition(i));
        }

        Vector3 firstPoint = trajectoryPoints[0];
        Vector3 lastPoint = trajectoryPoints[trajectoryPoints.Count - 1];

        Debug.Log($"起点坐标: ({firstPoint.x:F2}, {firstPoint.y:F2}, {firstPoint.z:F2})");
        Debug.Log($"终点坐标: ({lastPoint.x:F2}, {lastPoint.y:F2}, {lastPoint.z:F2})");
        Debug.Log($"总点数: {trajectoryPoints.Count}");

        // 找到最接近地面的点
        Vector3 closestToGround = lastPoint;
        float minHeight = lastPoint.y;

        foreach (Vector3 point in trajectoryPoints)
        {
            if (point.y < minHeight)
            {
                minHeight = point.y;
                closestToGround = point;
            }
        }

        Debug.Log($"最接近地面的点: ({closestToGround.x:F2}, {closestToGround.y:F2}, {closestToGround.z:F2})");

        // 投射到地面的预测落点
        Vector3 groundProjection = new Vector3(lastPoint.x, 0.01f, lastPoint.z);
        Debug.Log($"地面投射落点: ({groundProjection.x:F2}, {groundProjection.y:F2}, {groundProjection.z:F2})");

        lastPredictedLandingPoint = groundProjection;
    }

    /// <summary>
    /// 对比圆环标记和轨迹终点位置
    /// </summary>
    void ComparePositions()
    {
        Debug.Log("=== 位置对比分析 ===");

        // 获取轨迹终点
        ShowTrajectoryEndPoint();
        Vector3 trajectoryEnd = lastPredictedLandingPoint;

        if (trajectoryEnd == Vector3.zero)
        {
            Debug.Log("❌ 无法获取轨迹终点坐标");
            return;
        }

        Debug.Log($"轨迹线预测落点: ({trajectoryEnd.x:F2}, {trajectoryEnd.y:F2}, {trajectoryEnd.z:F2})");

        // 查找场景中的圆环标记
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> rings = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring"))
            {
                rings.Add(obj);
            }
        }

        if (rings.Count == 0)
        {
            Debug.Log("⚠️ 场景中未找到圆环标记");
            Debug.Log("💡 发射一个网球来创建圆环标记，然后再次运行对比");
            return;
        }

        Debug.Log($"找到 {rings.Count} 个圆环标记:");

        foreach (GameObject ring in rings)
        {
            Vector3 ringPos = ring.transform.position;
            Vector3 ringGroundPos = new Vector3(ringPos.x, 0.01f, ringPos.z);
            float distance = Vector3.Distance(trajectoryEnd, ringGroundPos);

            Debug.Log($"  圆环 {ring.name}:");
            Debug.Log($"    位置: ({ringPos.x:F2}, {ringPos.y:F2}, {ringPos.z:F2})");
            Debug.Log($"    地面投射: ({ringGroundPos.x:F2}, {ringGroundPos.y:F2}, {ringGroundPos.z:F2})");
            Debug.Log($"    与轨迹终点距离: {distance:F2}m");

            if (distance > positionTolerance)
            {
                Debug.LogWarning($"    ⚠️ 位置偏差过大! (>={positionTolerance:F1}m)");
                Vector3 correction = trajectoryEnd - ringGroundPos;
                Debug.Log($"    📐 需要修正: X{correction.x:F2}m, Z{correction.z:F2}m");
            }
            else
            {
                Debug.Log($"    ✅ 位置基本准确 (<{positionTolerance:F1}m)");
            }
        }
    }

    /// <summary>
    /// 当网球被发射时记录预期落点
    /// </summary>
    public void OnBallLaunched(GameObject ball)
    {
        if (lastPredictedLandingPoint != Vector3.zero)
        {
            ballExpectedLanding[ball] = lastPredictedLandingPoint;

            if (showDebugInfo)
            {
                Debug.Log($"🎾 记录网球 {ball.name} 的预期落点: {lastPredictedLandingPoint}");
            }
        }
    }

    /// <summary>
    /// 获取网球的预期落点
    /// </summary>
    public Vector3 GetExpectedLandingPoint(GameObject ball)
    {
        if (ballExpectedLanding.ContainsKey(ball))
        {
            return ballExpectedLanding[ball];
        }
        return Vector3.zero;
    }

    /// <summary>
    /// 修正圆环标记位置
    /// </summary>
    public Vector3 GetCorrectedImpactPosition(Vector3 originalImpactPoint, GameObject ball)
    {
        if (!enablePositionFix)
            return originalImpactPoint;

        Vector3 expectedLanding = GetExpectedLandingPoint(ball);
        if (expectedLanding == Vector3.zero)
            return originalImpactPoint;

        // 使用轨迹预测的落点作为圆环位置
        Vector3 correctedPosition = new Vector3(expectedLanding.x, 0.01f, expectedLanding.z);

        if (showDebugInfo)
        {
            float difference = Vector3.Distance(originalImpactPoint, correctedPosition);
            Debug.Log($"🔧 圆环位置修正:");
            Debug.Log($"   原始位置: ({originalImpactPoint.x:F2}, {originalImpactPoint.z:F2})");
            Debug.Log($"   修正位置: ({correctedPosition.x:F2}, {correctedPosition.z:F2})");
            Debug.Log($"   修正距离: {difference:F2}m");
        }

        return correctedPosition;
    }

    /// <summary>
    /// 切换位置修复功能
    /// </summary>
    void TogglePositionFix()
    {
        enablePositionFix = !enablePositionFix;
        Debug.Log($"🔧 位置修复功能: {(enablePositionFix ? "启用" : "禁用")}");
    }

    /// <summary>
    /// 切换轨迹终点标记显示
    /// </summary>
    void ToggleTrajectoryEndMarker()
    {
        showTrajectoryEndPoint = !showTrajectoryEndPoint;
        if (trajectoryEndMarker != null)
        {
            trajectoryEndMarker.SetActive(showTrajectoryEndPoint);
        }
        Debug.Log($"🎯 轨迹终点标记: {(showTrajectoryEndPoint ? "显示" : "隐藏")}");
    }

    /// <summary>
    /// 清理已销毁的网球记录
    /// </summary>
    void CleanupBallRecords()
    {
        List<GameObject> ballsToRemove = new List<GameObject>();

        foreach (var ball in ballExpectedLanding.Keys)
        {
            if (ball == null)
            {
                ballsToRemove.Add(ball);
            }
        }

        foreach (var ball in ballsToRemove)
        {
            ballExpectedLanding.Remove(ball);
        }
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        GUILayout.BeginArea(new Rect(Screen.width - 350, 10, 340, 250));
        GUILayout.BeginVertical("box");

        GUILayout.Label("Impact Marker Position Fixer", new GUIStyle("label") { fontStyle = FontStyle.Bold });

        // 状态显示
        GUI.color = enablePositionFix ? Color.green : Color.red;
        GUILayout.Label($"位置修复: {(enablePositionFix ? "启用" : "禁用")}");

        GUI.color = Color.white;
        if (lastPredictedLandingPoint != Vector3.zero)
        {
            GUILayout.Label($"预测落点: ({lastPredictedLandingPoint.x:F1}, {lastPredictedLandingPoint.z:F1})");
        }
        else
        {
            GUILayout.Label("预测落点: 未计算");
        }

        GUILayout.Label($"追踪网球数: {ballExpectedLanding.Count}");

        // 控制按钮
        if (GUILayout.Button("P: 显示轨迹终点"))
        {
            ShowTrajectoryEndPoint();
        }

        if (GUILayout.Button("O: 对比位置"))
        {
            ComparePositions();
        }

        if (GUILayout.Button("I: 切换修复功能"))
        {
            TogglePositionFix();
        }

        if (GUILayout.Button("U: 切换终点标记"))
        {
            ToggleTrajectoryEndMarker();
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        // 清理记录
        if (Time.frameCount % 300 == 0) // 每5秒清理一次
        {
            CleanupBallRecords();
        }
    }
}