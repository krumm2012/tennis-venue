using UnityEngine;
using TMPro;

/// <summary>
/// 空气阻力系统 - 按照室内标准大气压优化网球飞行
/// </summary>
public class AirResistanceSystem : MonoBehaviour
{
    [Header("室内环境参数")]
    [Tooltip("室内标准大气压 (Pa)")]
    public float indoorAtmosphericPressure = 101325f; // 标准大气压 101.325 kPa

    [Tooltip("室内温度 (°C)")]
    public float indoorTemperature = 20f; // 室内标准温度 20°C

    [Tooltip("室内湿度 (%)")]
    public float indoorHumidity = 50f; // 室内标准湿度 50%

    [Header("网球空气阻力参数")]
    [Tooltip("网球直径 (m)")]
    public float ballDiameter = 0.067f; // 标准网球直径 6.7cm

    [Tooltip("网球质量 (kg)")]
    public float ballMass = 0.057f; // 标准网球质量 57g

    [Tooltip("网球阻力系数")]
    public float dragCoefficient = 0.47f; // 球体标准阻力系数

    [Header("计算结果显示")]
    public TextMeshProUGUI airResistanceInfoText;
    public bool showDetailedInfo = true;

    [Header("优化后的Unity物理参数")]
    public float optimizedLinearDrag = 0.15f;
    public float optimizedAngularDrag = 0.08f;

    private float airDensity; // 空气密度
    private float ballCrossSectionArea; // 网球横截面积
    private GameObject currentBall;

    void Start()
    {
        InitializeAirResistanceSystem();
    }

    void InitializeAirResistanceSystem()
    {
        Debug.Log("=== 初始化室内空气阻力系统 ===");

        // 计算室内空气密度
        CalculateIndoorAirDensity();

        // 计算网球物理参数
        CalculateBallPhysics();

        // 优化Unity物理参数
        OptimizeUnityPhysicsParameters();

        // 创建信息显示UI
        CreateAirResistanceInfoUI();

        Debug.Log("室内空气阻力系统初始化完成");
        LogAirResistanceData();
    }

    /// <summary>
    /// 计算室内空气密度
    /// </summary>
    void CalculateIndoorAirDensity()
    {
        // 根据理想气体定律计算空气密度
        // ρ = P / (R_specific * T)
        // R_specific = 287.05 J/(kg·K) 空气的比气体常数

        float temperatureKelvin = indoorTemperature + 273.15f;
        float specificGasConstant = 287.05f; // J/(kg·K)

        // 考虑湿度对空气密度的影响
        float humidityFactor = 1.0f - (indoorHumidity / 100f) * 0.02f; // 湿度对密度的轻微影响

        airDensity = (indoorAtmosphericPressure / (specificGasConstant * temperatureKelvin)) * humidityFactor;

        Debug.Log($"室内空气密度: {airDensity:F4} kg/m³");
    }

    /// <summary>
    /// 计算网球相关物理参数
    /// </summary>
    void CalculateBallPhysics()
    {
        // 计算网球横截面积
        float radius = ballDiameter / 2f;
        ballCrossSectionArea = Mathf.PI * radius * radius;

        Debug.Log($"网球横截面积: {ballCrossSectionArea:F6} m²");
    }

    /// <summary>
    /// 优化Unity物理参数
    /// </summary>
    void OptimizeUnityPhysicsParameters()
    {
        // 根据实际物理计算优化Unity的drag参数
        // 考虑室内环境下的空气阻力影响

        // 基于空气密度调整线性阻力
        float standardAirDensity = 1.225f; // 标准大气压下的空气密度
        float densityRatio = airDensity / standardAirDensity;

        optimizedLinearDrag = 0.12f * densityRatio; // 基础值乘以密度比例
        optimizedAngularDrag = 0.06f * densityRatio;

        Debug.Log($"优化后的线性阻力: {optimizedLinearDrag:F4}");
        Debug.Log($"优化后的角阻力: {optimizedAngularDrag:F4}");
    }

    /// <summary>
    /// 应用优化的空气阻力到网球
    /// </summary>
    public void ApplyOptimizedResistance(GameObject ball)
    {
        if (ball == null) return;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = ballMass;
            rb.drag = optimizedLinearDrag;
            rb.angularDrag = optimizedAngularDrag;

            Debug.Log($"已应用优化空气阻力到球: {ball.name}");
        }
    }

    /// <summary>
    /// 计算特定速度下的空气阻力力量
    /// </summary>
    public float CalculateAirResistanceForce(float velocity)
    {
        // F_drag = 0.5 * ρ * v² * C_d * A
        float dragForce = 0.5f * airDensity * velocity * velocity * dragCoefficient * ballCrossSectionArea;
        return dragForce;
    }

    /// <summary>
    /// 分析速度对落点的影响
    /// </summary>
    public Vector2 AnalyzeLandingPointImpact(float initialVelocity, float launchAngle)
    {
        // 无空气阻力的理论落点计算
        float gravity = 9.81f;
        float angleRad = launchAngle * Mathf.Deg2Rad;

        float theoreticalRange = (initialVelocity * initialVelocity * Mathf.Sin(2 * angleRad)) / gravity;

        // 考虑空气阻力的修正系数
        float averageVelocity = initialVelocity * 0.7f; // 平均速度约为初速度的70%
        float dragForce = CalculateAirResistanceForce(averageVelocity);
        float dragAcceleration = dragForce / ballMass;

        // 空气阻力导致的射程减少
        float resistanceReduction = dragAcceleration / gravity;
        float actualRange = theoreticalRange * (1f - resistanceReduction * 0.3f); // 经验修正

        return new Vector2(theoreticalRange, actualRange);
    }

    /// <summary>
    /// 创建空气阻力信息显示UI
    /// </summary>
    void CreateAirResistanceInfoUI()
    {
        if (!showDetailedInfo) return;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject infoObj = new GameObject("AirResistanceInfoText");
        infoObj.transform.SetParent(canvas.transform, false);
        infoObj.layer = 5;

        airResistanceInfoText = infoObj.AddComponent<TextMeshProUGUI>();
        airResistanceInfoText.fontSize = 12;
        airResistanceInfoText.color = Color.cyan;

        RectTransform rect = airResistanceInfoText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1); // 右上角
        rect.anchorMax = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-20, -20);
        rect.sizeDelta = new Vector2(300, 120);

        UpdateAirResistanceInfoDisplay();
    }

    /// <summary>
    /// 更新空气阻力信息显示
    /// </summary>
    void UpdateAirResistanceInfoDisplay()
    {
        if (airResistanceInfoText == null) return;

        string info = $"室内空气阻力分析:\n" +
                     $"• 大气压: {indoorAtmosphericPressure/1000:F1} kPa\n" +
                     $"• 温度: {indoorTemperature}°C, 湿度: {indoorHumidity}%\n" +
                     $"• 空气密度: {airDensity:F4} kg/m³\n" +
                     $"• 线性阻力: {optimizedLinearDrag:F4}\n" +
                     $"• 落点影响: 约减少15-25%";

        airResistanceInfoText.text = info;
    }

    /// <summary>
    /// 记录空气阻力详细数据
    /// </summary>
    void LogAirResistanceData()
    {
        Debug.Log("=== 室内空气阻力详细分析 ===");
        Debug.Log($"环境参数:");
        Debug.Log($"  大气压: {indoorAtmosphericPressure} Pa ({indoorAtmosphericPressure/1000:F1} kPa)");
        Debug.Log($"  温度: {indoorTemperature}°C");
        Debug.Log($"  湿度: {indoorHumidity}%");
        Debug.Log($"计算结果:");
        Debug.Log($"  空气密度: {airDensity:F4} kg/m³");
        Debug.Log($"  网球横截面积: {ballCrossSectionArea:F6} m²");
        Debug.Log($"Unity物理参数:");
        Debug.Log($"  优化线性阻力: {optimizedLinearDrag:F4}");
        Debug.Log($"  优化角阻力: {optimizedAngularDrag:F4}");

        // 计算不同速度下的阻力影响
        float[] testVelocities = {10f, 15f, 20f, 25f, 30f};
        Debug.Log("不同速度下的阻力影响:");
        foreach (float vel in testVelocities)
        {
            Vector2 range = AnalyzeLandingPointImpact(vel, 45f);
            float reduction = (range.x - range.y) / range.x * 100f;
            Debug.Log($"  {vel}m/s: 理论{range.x:F1}m → 实际{range.y:F1}m (减少{reduction:F1}%)");
        }
    }

    void Update()
    {
        // 按H键显示详细空气阻力分析
        if (Input.GetKeyDown(KeyCode.H))
        {
            LogAirResistanceData();
        }

        // 按U键切换信息显示
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (airResistanceInfoText != null)
            {
                airResistanceInfoText.gameObject.SetActive(!airResistanceInfoText.gameObject.activeSelf);
            }
        }
    }

    /// <summary>
    /// 获取当前环境下的空气阻力修正系数
    /// </summary>
    public float GetAirResistanceFactor()
    {
        return optimizedLinearDrag / 0.12f; // 相对于基准值的比例
    }
}