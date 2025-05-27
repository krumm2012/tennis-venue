using UnityEngine;

/// <summary>
/// 异常网球清理器 - 自动检测和清理掉落到异常位置的网球，防止误触发
/// </summary>
public class AbnormalBallCleaner : MonoBehaviour
{
    [Header("清理设置")]
    [Tooltip("是否启用自动清理")]
    public bool enableAutoCleaning = true;

    [Tooltip("检查间隔（秒）")]
    public float checkInterval = 1f;

    [Tooltip("异常高度阈值（低于此高度视为异常）")]
    public float abnormalHeightThreshold = -10f;

    [Tooltip("异常速度阈值（超过此速度视为异常）")]
    public float abnormalSpeedThreshold = 100f;

    [Tooltip("场地边界（超出此范围视为异常）")]
    public float fieldBoundary = 15f;

    [Tooltip("网球存活时间上限（秒）")]
    public float maxBallLifetime = 30f;

    private float lastCheckTime;

    void Start()
    {
        Debug.Log("=== Abnormal Ball Cleaner Started ===");
        Debug.Log($"Auto cleaning: {enableAutoCleaning}");
        Debug.Log($"Height threshold: {abnormalHeightThreshold}m");
        Debug.Log($"Speed threshold: {abnormalSpeedThreshold}m/s");
        Debug.Log($"Field boundary: ±{fieldBoundary}m");
        Debug.Log("Press Ctrl+Delete to manually clean all abnormal balls");
    }

    void Update()
    {
        // 自动清理
        if (enableAutoCleaning && Time.time - lastCheckTime > checkInterval)
        {
            CleanAbnormalBalls();
            lastCheckTime = Time.time;
        }

        // 手动清理快捷键
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Delete))
        {
            ManualCleanAllAbnormalBalls();
        }
    }

    /// <summary>
    /// 清理异常网球
    /// </summary>
    void CleanAbnormalBalls()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int cleanedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                if (IsAbnormalBall(obj))
                {
                    Debug.LogWarning($"🧹 清理异常网球: {obj.name} - {GetAbnormalReason(obj)}");
                    Destroy(obj);
                    cleanedCount++;
                }
            }
        }

        if (cleanedCount > 0)
        {
            Debug.Log($"✅ 自动清理了 {cleanedCount} 个异常网球");
        }
    }

    /// <summary>
    /// 手动清理所有异常网球
    /// </summary>
    void ManualCleanAllAbnormalBalls()
    {
        Debug.Log("=== 手动清理所有异常网球 ===");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int totalBalls = 0;
        int abnormalBalls = 0;
        int cleanedBalls = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                totalBalls++;

                if (IsAbnormalBall(obj))
                {
                    abnormalBalls++;
                    string reason = GetAbnormalReason(obj);
                    Debug.LogWarning($"🧹 清理异常网球: {obj.name} - {reason}");
                    Destroy(obj);
                    cleanedBalls++;
                }
                else
                {
                    // 显示正常网球的状态
                    Vector3 pos = obj.transform.position;
                    Rigidbody rb = obj.GetComponent<Rigidbody>();
                    float speed = rb != null ? rb.velocity.magnitude : 0f;
                    Debug.Log($"✅ 正常网球: {obj.name} - 位置: {pos}, 速度: {speed:F2}m/s");
                }
            }
        }

        Debug.Log($"📊 清理统计:");
        Debug.Log($"   总网球数: {totalBalls}");
        Debug.Log($"   异常网球数: {abnormalBalls}");
        Debug.Log($"   已清理数: {cleanedBalls}");
        Debug.Log($"   剩余正常网球数: {totalBalls - cleanedBalls}");
    }

    /// <summary>
    /// 检查是否为异常网球
    /// </summary>
    bool IsAbnormalBall(GameObject ball)
    {
        Vector3 position = ball.transform.position;
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        // 检查高度异常
        if (position.y < abnormalHeightThreshold)
        {
            return true;
        }

        // 检查速度异常
        if (rb != null && rb.velocity.magnitude > abnormalSpeedThreshold)
        {
            return true;
        }

        // 检查位置超出场地边界
        if (Mathf.Abs(position.x) > fieldBoundary || Mathf.Abs(position.z) > fieldBoundary)
        {
            return true;
        }

        // 检查网球是否存在时间过长（可能卡住了）
        // 这里简化处理，实际项目中可以记录创建时间
        if (position.y < -2f && rb != null && rb.velocity.magnitude < 0.1f)
        {
            return true; // 静止在地面以下的球
        }

        return false;
    }

    /// <summary>
    /// 获取异常原因
    /// </summary>
    string GetAbnormalReason(GameObject ball)
    {
        Vector3 position = ball.transform.position;
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (position.y < abnormalHeightThreshold)
        {
            return $"高度异常 ({position.y:F2}m < {abnormalHeightThreshold}m)";
        }

        if (rb != null && rb.velocity.magnitude > abnormalSpeedThreshold)
        {
            return $"速度异常 ({rb.velocity.magnitude:F2}m/s > {abnormalSpeedThreshold}m/s)";
        }

        if (Mathf.Abs(position.x) > fieldBoundary || Mathf.Abs(position.z) > fieldBoundary)
        {
            return $"超出场地边界 (位置: {position})";
        }

        if (position.y < -2f && rb != null && rb.velocity.magnitude < 0.1f)
        {
            return $"静止在地面以下 (高度: {position.y:F2}m, 速度: {rb.velocity.magnitude:F2}m/s)";
        }

        return "未知异常";
    }

    /// <summary>
    /// 获取当前网球统计信息
    /// </summary>
    public void GetBallStatistics()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int totalBalls = 0;
        int normalBalls = 0;
        int abnormalBalls = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                totalBalls++;
                if (IsAbnormalBall(obj))
                {
                    abnormalBalls++;
                }
                else
                {
                    normalBalls++;
                }
            }
        }

        Debug.Log($"📊 当前网球统计:");
        Debug.Log($"   总数: {totalBalls}");
        Debug.Log($"   正常: {normalBalls}");
        Debug.Log($"   异常: {abnormalBalls}");
    }

    void OnDestroy()
    {
        Debug.Log("Abnormal Ball Cleaner 已关闭");
    }
}