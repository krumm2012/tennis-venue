using UnityEngine;

/// <summary>
/// 即时球对象修复器 - 立即修复ballPrefab引用问题
/// </summary>
public class InstantBallFix : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 即时球对象修复器启动 ===");
        FixBallPrefabReference();
    }

    void FixBallPrefabReference()
    {
        BallLauncher launcher = FindObjectOfType<BallLauncher>();
        if (launcher == null)
        {
            Debug.LogError("❌ 未找到BallLauncher组件！");
            return;
        }

        if (launcher.ballPrefab == null)
        {
            Debug.LogWarning("⚠️ ballPrefab引用丢失，正在修复...");

            // 方法1: 尝试从Assets/Prefabs加载
            GameObject prefab = null;

#if UNITY_EDITOR
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/TennisBall.prefab");
#endif

            if (prefab != null)
            {
                launcher.ballPrefab = prefab;
                Debug.Log("✅ ballPrefab已成功修复！");
                Debug.Log($"   预制体名称: {prefab.name}");
                Debug.Log($"   预制体路径: Assets/Prefabs/TennisBall.prefab");
            }
            else
            {
                Debug.LogError("❌ 无法加载TennisBall预制体！");
                CreateTemporaryBall(launcher);
            }
        }
        else
        {
            Debug.Log("✅ ballPrefab引用正常");
            Debug.Log($"   当前预制体: {launcher.ballPrefab.name}");
        }

        // 同时检查其他重要引用
        if (launcher.launchPoint == null)
        {
            launcher.launchPoint = launcher.transform;
            Debug.Log("🔧 launchPoint已设置为发球机自身");
        }

        if (launcher.mainCamera == null)
        {
            launcher.mainCamera = Camera.main;
            Debug.Log("🔧 mainCamera已设置为主摄像机");
        }
    }

    void CreateTemporaryBall(BallLauncher launcher)
    {
        Debug.Log("=== 创建临时网球对象 ===");

        // 创建临时网球
        GameObject tempBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tempBall.name = "TennisBall_Temp";
        tempBall.transform.localScale = Vector3.one * 0.067f;

        // 添加物理组件
        Rigidbody rb = tempBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.02f;
        rb.useGravity = true;

        // 设置材质
        Renderer renderer = tempBall.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.yellow;
        renderer.material = mat;

        // 设置为ballPrefab
        launcher.ballPrefab = tempBall;
        Debug.Log("🔧 临时网球对象已创建并设置为ballPrefab");
        Debug.Log("⚠️ 这是临时解决方案，建议重新创建TennisBall预制体");

        // 隐藏临时球（它只是作为预制体使用）
        tempBall.SetActive(false);
    }

    void Update()
    {
        // 提供手动修复快捷键
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("手动触发ballPrefab修复...");
            FixBallPrefabReference();
        }
    }
}