using UnityEngine;

/// <summary>
/// 空气阻力测试数据脚本 - 展示不同参数下的落点影响
/// </summary>
public class AirResistanceTestData : MonoBehaviour
{
    [Header("测试参数")]
    public bool autoRunTests = true;
    public AirResistanceSystem airResistanceSystem;

    void Start()
    {
        if (autoRunTests)
        {
            Invoke("RunAirResistanceTests", 2f); // 延迟2秒确保系统初始化完成
        }
    }

    /// <summary>
    /// 运行空气阻力测试
    /// </summary>
    public void RunAirResistanceTests()
    {
        if (airResistanceSystem == null)
        {
            airResistanceSystem = FindObjectOfType<AirResistanceSystem>();
        }

        if (airResistanceSystem == null)
        {
            Debug.LogWarning("未找到空气阻力系统，无法运行测试");
            return;
        }

        Debug.Log("=== 空气阻力影响测试 ===");

        // 测试不同速度的影响
        TestVelocityImpact();

        // 测试不同角度的影响
        TestAngleImpact();

        // 测试室内环境优化效果
        TestIndoorOptimization();
    }

    /// <summary>
    /// 测试不同速度下的空气阻力影响
    /// </summary>
    void TestVelocityImpact()
    {
        Debug.Log("--- 不同速度下的空气阻力影响 ---");

        float[] testVelocities = {10f, 15f, 20f, 25f, 30f};
        float testAngle = 45f; // 使用45度最优角度

        foreach (float velocity in testVelocities)
        {
            Vector2 range = airResistanceSystem.AnalyzeLandingPointImpact(velocity, testAngle);
            float reduction = (range.x - range.y) / range.x * 100f;
            float dragForce = airResistanceSystem.CalculateAirResistanceForce(velocity);

            Debug.Log($"速度 {velocity:F0}m/s: " +
                     $"理论射程 {range.x:F1}m → 实际射程 {range.y:F1}m " +
                     $"(减少 {reduction:F1}%) " +
                     $"阻力 {dragForce:F4}N");
        }
    }

    /// <summary>
    /// 测试不同角度下的空气阻力影响
    /// </summary>
    void TestAngleImpact()
    {
        Debug.Log("--- 不同发射角度下的空气阻力影响 ---");

        float[] testAngles = {15f, 30f, 45f, 60f, 75f};
        float testVelocity = 20f; // 使用20m/s标准速度

        foreach (float angle in testAngles)
        {
            Vector2 range = airResistanceSystem.AnalyzeLandingPointImpact(testVelocity, angle);
            float reduction = (range.x - range.y) / range.x * 100f;

            Debug.Log($"角度 {angle:F0}°: " +
                     $"理论射程 {range.x:F1}m → 实际射程 {range.y:F1}m " +
                     $"(减少 {reduction:F1}%)");
        }
    }

    /// <summary>
    /// 测试室内环境优化效果
    /// </summary>
    void TestIndoorOptimization()
    {
        Debug.Log("--- 室内环境优化效果对比 ---");

        float resistanceFactor = airResistanceSystem.GetAirResistanceFactor();

        Debug.Log($"空气阻力优化系数: {resistanceFactor:F4}");
        Debug.Log($"室内环境优化: {(resistanceFactor < 1.0f ? "阻力减小" : "阻力增大")} " +
                  $"{Mathf.Abs((1.0f - resistanceFactor) * 100f):F1}%");

        // 与标准环境对比
        float standardVelocity = 20f;
        float standardAngle = 45f;

        Vector2 indoorRange = airResistanceSystem.AnalyzeLandingPointImpact(standardVelocity, standardAngle);

        // 模拟标准环境下的射程（假设阻力增加20%）
        float standardReduction = 0.2f;
        float theoreticalRange = indoorRange.x;
        float standardRange = theoreticalRange * (1f - standardReduction);

        float improvement = (indoorRange.y - standardRange) / standardRange * 100f;

        Debug.Log($"20m/s, 45°发射对比:");
        Debug.Log($"  理论射程: {theoreticalRange:F1}m");
        Debug.Log($"  室内环境: {indoorRange.y:F1}m");
        Debug.Log($"  标准环境: {standardRange:F1}m");
        Debug.Log($"  室内优势: {improvement:F1}%");
    }

    void Update()
    {
        // 按T键手动运行测试
        if (Input.GetKeyDown(KeyCode.T))
        {
            RunAirResistanceTests();
        }
    }
}