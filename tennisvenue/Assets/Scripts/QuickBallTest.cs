using UnityEngine;

/// <summary>
/// 快速球对象测试器 - 验证ballPrefab修复是否成功
/// </summary>
public class QuickBallTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 快速球对象测试器启动 ===");
        CheckBallLauncher();

        Debug.Log("快捷键:");
        Debug.Log("  F6键: 测试发球 (独立测试)");
        Debug.Log("  F1键: 检查发球机状态");
        Debug.Log("  F2键: 强制修复ballPrefab");
        Debug.Log("💡 注意: 空格键已保留给BallLauncher正常发射使用");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6)) // 改为F6键，避免与BallLauncher冲突
        {
            TestLaunch();
        }
        else if (Input.GetKeyDown(KeyCode.F1))
        {
            CheckBallLauncher();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            ForceFixBallPrefab();
        }
    }

    void CheckBallLauncher()
    {
        Debug.Log("=== 检查发球机状态 ===");

        BallLauncher launcher = FindObjectOfType<BallLauncher>();
        if (launcher == null)
        {
            Debug.LogError("❌ 未找到BallLauncher组件！");
            return;
        }

        Debug.Log($"发球机位置: {launcher.transform.position}");

        if (launcher.ballPrefab != null)
        {
            Debug.Log($"✅ ballPrefab: {launcher.ballPrefab.name}");
            Debug.Log($"   预制体激活状态: {launcher.ballPrefab.activeSelf}");

            // 检查预制体组件
            Rigidbody rb = launcher.ballPrefab.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log($"   Rigidbody质量: {rb.mass}kg");
            }
            else
            {
                Debug.LogWarning("⚠️ 预制体缺少Rigidbody组件");
            }
        }
        else
        {
            Debug.LogError("❌ ballPrefab仍然为空！");
        }

        if (launcher.launchPoint != null)
        {
            Debug.Log($"✅ launchPoint: {launcher.launchPoint.name}");
        }
        else
        {
            Debug.LogError("❌ launchPoint为空！");
        }
    }

    void TestLaunch()
    {
        Debug.Log("=== 测试发球 ===");

        BallLauncher launcher = FindObjectOfType<BallLauncher>();
        if (launcher == null)
        {
            Debug.LogError("❌ 未找到BallLauncher组件！");
            return;
        }

        if (launcher.ballPrefab == null)
        {
            Debug.LogError("❌ ballPrefab为空，无法发球！");
            Debug.LogWarning("请按F2键修复ballPrefab");
            return;
        }

        Debug.Log("🚀 执行发球...");
        launcher.LaunchBall(Vector3.zero);

        // 检查是否成功创建球
        StartCoroutine(CheckBallCreation());
    }

    System.Collections.IEnumerator CheckBallCreation()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject[] balls = GameObject.FindGameObjectsWithTag("Untagged");
        bool foundBall = false;

        foreach (GameObject obj in balls)
        {
            if (obj.name.Contains("TennisBall") && obj.GetComponent<Rigidbody>() != null)
            {
                Debug.Log($"✅ 发球成功！创建了球: {obj.name}");
                Debug.Log($"   位置: {obj.transform.position}");
                Debug.Log($"   速度: {obj.GetComponent<Rigidbody>().velocity.magnitude:F2}m/s");
                foundBall = true;
                break;
            }
        }

        if (!foundBall)
        {
            Debug.LogWarning("⚠️ 未检测到新创建的球，可能发球失败");
        }
    }

    void ForceFixBallPrefab()
    {
        Debug.Log("=== 强制修复ballPrefab ===");

        BallLauncher launcher = FindObjectOfType<BallLauncher>();
        if (launcher == null)
        {
            Debug.LogError("❌ 未找到BallLauncher组件！");
            return;
        }

        // 创建一个新的网球预制体
        GameObject newBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newBall.name = "TennisBall_Fixed";
        newBall.transform.localScale = Vector3.one * 0.067f;

        // 添加物理组件
        Rigidbody rb = newBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.02f;
        rb.useGravity = true;

        // 设置材质
        Renderer renderer = newBall.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(1f, 1f, 0.8f); // 淡黄色
        renderer.material = mat;

        // 设置为ballPrefab
        launcher.ballPrefab = newBall;
        Debug.Log("🔧 ballPrefab已强制修复！");

        // 隐藏这个对象（它只是作为预制体模板）
        newBall.SetActive(false);

        // 验证修复结果
        CheckBallLauncher();
    }
}