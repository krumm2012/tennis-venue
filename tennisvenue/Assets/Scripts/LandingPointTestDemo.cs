using UnityEngine;
using System.Collections;

/// <summary>
/// 落点坐标追踪演示脚本 - 自动测试落点追踪功能
/// </summary>
public class LandingPointTestDemo : MonoBehaviour
{
    [Header("自动测试参数")]
    [Tooltip("是否启动时自动运行测试")]
    public bool autoRunTest = true;

    [Tooltip("测试间隔时间（秒）")]
    public float testInterval = 3f;

    [Tooltip("测试发射次数")]
    public int testCount = 5;

    [Header("测试配置")]
    public float[] testAngles = {30f, 45f, 60f};
    public float[] testSpeeds = {15f, 20f, 25f};
    public float[] testDirections = {-20f, 0f, 20f};

    private BallLauncher ballLauncher;
    private LandingPointTracker landingTracker;
    private int currentTestIndex = 0;
    private bool isTesting = false;

    void Start()
    {
        // 查找必要的组件
        ballLauncher = FindObjectOfType<BallLauncher>();
        landingTracker = FindObjectOfType<LandingPointTracker>();

        if (autoRunTest)
        {
            StartCoroutine(RunLandingPointTests());
        }

        Debug.Log("落点坐标测试演示已初始化");
        LogTestParameters();
    }

    /// <summary>
    /// 运行落点坐标测试
    /// </summary>
    IEnumerator RunLandingPointTests()
    {
        if (ballLauncher == null || landingTracker == null)
        {
            Debug.LogWarning("缺少必要组件，无法运行测试");
            yield break;
        }

        isTesting = true;
        Debug.Log("=== 开始落点坐标自动测试 ===");

        yield return new WaitForSeconds(2f); // 等待系统初始化

        for (int i = 0; i < testCount; i++)
        {
            currentTestIndex = i;

            // 设置测试参数
            float angle = testAngles[i % testAngles.Length];
            float speed = testSpeeds[i % testSpeeds.Length];
            float direction = testDirections[i % testDirections.Length];

            Debug.Log($"--- 测试 {i + 1}/{testCount} ---");
            Debug.Log($"参数: 角度{angle}°, 速度{speed}, 方向{direction}°");

            // 设置发球机参数
            SetLauncherParameters(angle, speed, direction);

            // 等待参数设置生效
            yield return new WaitForSeconds(0.5f);

            // 发射网球
            ballLauncher.LaunchBall(Vector3.zero);

            // 等待球落地并记录结果
            yield return StartCoroutine(WaitForLanding());

            // 输出测试结果
            LogTestResult(i + 1);

            // 测试间隔
            yield return new WaitForSeconds(testInterval);
        }

        isTesting = false;
        Debug.Log("=== 落点坐标测试完成 ===");
        LogFinalResults();
    }

    /// <summary>
    /// 设置发球机参数
    /// </summary>
    void SetLauncherParameters(float angle, float speed, float direction)
    {
        // 通过反射或直接访问设置参数
        if (ballLauncher.angleSlider != null)
        {
            ballLauncher.angleSlider.value = angle;
        }

        if (ballLauncher.speedSlider != null)
        {
            ballLauncher.speedSlider.value = speed;
        }

        if (ballLauncher.directionSlider != null)
        {
            ballLauncher.directionSlider.value = direction;
        }

        // 直接调用设置方法
        ballLauncher.SetDirection(direction);
    }

    /// <summary>
    /// 等待网球落地
    /// </summary>
    IEnumerator WaitForLanding()
    {
        Vector3 lastLanding = landingTracker.GetLastLandingPoint();
        float waitTime = 0f;
        float maxWaitTime = 10f; // 最大等待时间

        while (waitTime < maxWaitTime)
        {
            Vector3 currentLanding = landingTracker.GetLastLandingPoint();

            // 检查是否有新的落点记录
            if (currentLanding != lastLanding && currentLanding != Vector3.zero)
            {
                Debug.Log($"检测到新落点: ({currentLanding.x:F2}, {currentLanding.y:F2}, {currentLanding.z:F2})");
                break;
            }

            yield return new WaitForSeconds(0.1f);
            waitTime += 0.1f;
        }

        if (waitTime >= maxWaitTime)
        {
            Debug.LogWarning("等待落地超时");
        }
    }

    /// <summary>
    /// 记录测试结果
    /// </summary>
    void LogTestResult(int testNumber)
    {
        Vector3 landing = landingTracker.GetLastLandingPoint();

        if (landing != Vector3.zero)
        {
            // 计算与发球机的距离
            Vector3 launcherPos = ballLauncher.transform.position;
            float distance = Vector3.Distance(new Vector3(landing.x, 0, landing.z),
                                            new Vector3(launcherPos.x, 0, launcherPos.z));

            Debug.Log($"测试 {testNumber} 结果:");
            Debug.Log($"  落点坐标: ({landing.x:F2}, {landing.y:F2}, {landing.z:F2})");
            Debug.Log($"  与发球机距离: {distance:F2}m");
            Debug.Log($"  飞行距离: {Mathf.Abs(landing.z - launcherPos.z):F2}m");
        }
        else
        {
            Debug.LogWarning($"测试 {testNumber}: 未检测到落点");
        }
    }

    /// <summary>
    /// 记录最终测试结果统计
    /// </summary>
    void LogFinalResults()
    {
        var history = landingTracker.GetLandingHistory();

        Debug.Log("=== 测试结果统计 ===");
        Debug.Log($"总落点记录数: {history.Count}");

        if (history.Count > 0)
        {
            // 计算平均落点
            Vector3 average = Vector3.zero;
            foreach (var point in history)
            {
                average += point;
            }
            average /= history.Count;

            Debug.Log($"平均落点: ({average.x:F2}, {average.y:F2}, {average.z:F2})");

            // 计算散布范围
            float maxX = float.MinValue, minX = float.MaxValue;
            float maxZ = float.MinValue, minZ = float.MaxValue;

            foreach (var point in history)
            {
                if (point.x > maxX) maxX = point.x;
                if (point.x < minX) minX = point.x;
                if (point.z > maxZ) maxZ = point.z;
                if (point.z < minZ) minZ = point.z;
            }

            Debug.Log($"散布范围: X轴 {minX:F2} 到 {maxX:F2} (差值{maxX - minX:F2}m)");
            Debug.Log($"散布范围: Z轴 {minZ:F2} 到 {maxZ:F2} (差值{maxZ - minZ:F2}m)");
        }
    }

    /// <summary>
    /// 记录测试参数
    /// </summary>
    void LogTestParameters()
    {
        Debug.Log("=== 测试参数配置 ===");
        Debug.Log($"自动测试: {autoRunTest}");
        Debug.Log($"测试次数: {testCount}");
        Debug.Log($"测试间隔: {testInterval}秒");

        string angles = string.Join(", ", System.Array.ConvertAll(testAngles, x => x.ToString("F0") + "°"));
        string speeds = string.Join(", ", System.Array.ConvertAll(testSpeeds, x => x.ToString("F0")));
        string directions = string.Join(", ", System.Array.ConvertAll(testDirections, x => x.ToString("F0") + "°"));

        Debug.Log($"测试角度: {angles}");
        Debug.Log($"测试速度: {speeds}");
        Debug.Log($"测试方向: {directions}");
    }

    void Update()
    {
        // 手动触发测试
        if (Input.GetKeyDown(KeyCode.F1) && !isTesting)
        {
            StartCoroutine(RunLandingPointTests());
        }

        // 清除所有落点记录
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (landingTracker != null)
            {
                landingTracker.ClearLandingHistory();
                Debug.Log("已手动清除落点历史");
            }
        }

        // 显示当前状态
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ShowCurrentStatus();
        }
    }

    /// <summary>
    /// 显示当前状态
    /// </summary>
    void ShowCurrentStatus()
    {
        Debug.Log("=== 当前状态 ===");
        Debug.Log($"正在测试: {isTesting}");
        Debug.Log($"当前测试索引: {currentTestIndex}");

        if (landingTracker != null)
        {
            var history = landingTracker.GetLandingHistory();
            Debug.Log($"落点历史数量: {history.Count}");

            Vector3 lastPoint = landingTracker.GetLastLandingPoint();
            if (lastPoint != Vector3.zero)
            {
                Debug.Log($"最后落点: ({lastPoint.x:F2}, {lastPoint.y:F2}, {lastPoint.z:F2})");
            }
        }

        Debug.Log("快捷键说明:");
        Debug.Log("  F1: 开始/重新开始测试");
        Debug.Log("  F2: 清除落点历史");
        Debug.Log("  F3: 显示当前状态");
    }
}