using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 网球清理器 - 自动清理超出边界的网球
/// </summary>
public class TennisBallCleaner : MonoBehaviour
{
    [Header("清理设置")]
    public bool enableAutoCleaning = true;
    public float cleanupInterval = 5f;
    public float maxDistance = 50f;
    public int maxBallCount = 20;

    [Header("边界设置")]
    public Vector3 boundsCenter = Vector3.zero;
    public Vector3 boundsSize = new Vector3(100, 50, 100);

    private float lastCleanupTime;

    void Start()
    {
        lastCleanupTime = Time.time;
        Debug.Log("✅ TennisBallCleaner initialized - Auto cleaning enabled");
    }

    void Update()
    {
        if (!enableAutoCleaning) return;

        // 定期清理
        if (Time.time - lastCleanupTime >= cleanupInterval)
        {
            CleanupOutOfBoundsBalls();
            CleanupExcessBalls();
            lastCleanupTime = Time.time;
        }
    }

    /// <summary>
    /// 清理超出边界的网球
    /// </summary>
    void CleanupOutOfBoundsBalls()
    {
        List<GameObject> ballsToDestroy = new List<GameObject>();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (IsTennisBall(obj))
            {
                if (IsOutOfBounds(obj.transform.position))
                {
                    ballsToDestroy.Add(obj);
                }
            }
        }

        foreach (GameObject ball in ballsToDestroy)
        {
            if (ball != null)
            {
                Debug.Log($"🧹 Cleaning out-of-bounds ball: {ball.name} at {ball.transform.position}");
                Destroy(ball);
            }
        }
    }

    /// <summary>
    /// 检查对象是否是网球
    /// </summary>
    bool IsTennisBall(GameObject obj)
    {
        if (obj.name.Contains("TennisBall") || obj.name.Contains("Ball"))
        {
            // 确保对象有物理组件，更可能是真实的网球
            return obj.GetComponent<Rigidbody>() != null || obj.GetComponent<Collider>() != null;
        }
        return false;
    }

    /// <summary>
    /// 清理多余的网球
    /// </summary>
    void CleanupExcessBalls()
    {
        List<GameObject> allBalls = new List<GameObject>();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (IsTennisBall(obj))
            {
                allBalls.Add(obj);
            }
        }

        // 如果网球数量超过限制，删除最老的
        if (allBalls.Count > maxBallCount)
        {
            int ballsToRemove = allBalls.Count - maxBallCount;

            // 按创建时间排序（通过实例ID近似）
            allBalls.Sort((a, b) => a.GetInstanceID().CompareTo(b.GetInstanceID()));

            for (int i = 0; i < ballsToRemove; i++)
            {
                if (allBalls[i] != null)
                {
                    Debug.Log($"🧹 Cleaning excess ball: {allBalls[i].name}");
                    Destroy(allBalls[i]);
                }
            }
        }
    }

    /// <summary>
    /// 检查位置是否超出边界
    /// </summary>
    bool IsOutOfBounds(Vector3 position)
    {
        Vector3 min = boundsCenter - boundsSize * 0.5f;
        Vector3 max = boundsCenter + boundsSize * 0.5f;

        return position.x < min.x || position.x > max.x ||
               position.y < min.y || position.y > max.y ||
               position.z < min.z || position.z > max.z;
    }

    /// <summary>
    /// 手动清理所有网球
    /// </summary>
    [ContextMenu("Clean All Balls")]
    public void CleanAllBalls()
    {
        List<GameObject> ballsToDestroy = new List<GameObject>();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (IsTennisBall(obj))
            {
                ballsToDestroy.Add(obj);
            }
        }

        foreach (GameObject ball in ballsToDestroy)
        {
            if (ball != null)
            {
                Destroy(ball);
            }
        }

        Debug.Log($"🧹 Manually cleaned {ballsToDestroy.Count} balls");
    }

    void OnGUI()
    {
        if (!enableAutoCleaning) return;

        // 显示当前网球数量
        int ballCount = CountTennisBalls();
        GUILayout.Label($"当前网球数量: {ballCount}");

        if (ballCount > maxBallCount)
        {
            GUI.color = Color.red;
            GUILayout.Label($"⚠️ 网球数量过多！限制: {maxBallCount}");
        }
    }

    /// <summary>
    /// 计算当前网球数量
    /// </summary>
    int CountTennisBalls()
    {
        int count = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (IsTennisBall(obj))
            {
                count++;
            }
        }

        return count;
    }

    void OnDrawGizmosSelected()
    {
        // 绘制清理边界
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boundsCenter, boundsSize);
    }
}