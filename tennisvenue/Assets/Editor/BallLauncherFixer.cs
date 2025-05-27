using UnityEngine;
using UnityEditor;

/// <summary>
/// 发球机修复器 - Editor工具版本
/// </summary>
public class BallLauncherFixer
{
    [MenuItem("Tools/Fix Ball Launcher")]
    public static void FixBallLauncher()
    {
        Debug.Log("=== 开始修复发球机 ===");

        BallLauncher launcher = Object.FindObjectOfType<BallLauncher>();
        if (launcher == null)
        {
            Debug.LogError("❌ 未找到BallLauncher组件！");
            return;
        }

        bool needsFix = false;

        // 1. 检查并修复ballPrefab
        if (launcher.ballPrefab == null)
        {
            Debug.LogWarning("⚠️ ballPrefab丢失，正在修复...");

            // 尝试从Assets/Prefabs加载
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/TennisBall.prefab");
            
            if (prefab == null)
            {
                // 尝试查找所有TennisBall预制体
                string[] guids = AssetDatabase.FindAssets("TennisBall t:GameObject");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    Debug.Log($"找到预制体: {path}");
                }
            }

            if (prefab != null)
            {
                launcher.ballPrefab = prefab;
                Debug.Log("✅ ballPrefab已修复");
                needsFix = true;
            }
            else
            {
                Debug.LogError("❌ 无法找到TennisBall预制体！");
                CreateTennisBallPrefab();
            }
        }
        else
        {
            Debug.Log("✅ ballPrefab正常");
        }

        // 2. 检查并修复launchPoint
        if (launcher.launchPoint == null)
        {
            Debug.LogWarning("⚠️ launchPoint丢失，设置为发球机自身...");
            launcher.launchPoint = launcher.transform;
            Debug.Log("✅ launchPoint已修复");
            needsFix = true;
        }
        else
        {
            Debug.Log("✅ launchPoint正常");
        }

        // 3. 检查并修复mainCamera
        if (launcher.mainCamera == null)
        {
            Debug.LogWarning("⚠️ mainCamera丢失，正在修复...");
            launcher.mainCamera = Camera.main;
            if (launcher.mainCamera != null)
            {
                Debug.Log("✅ mainCamera已修复");
                needsFix = true;
            }
            else
            {
                Debug.LogError("❌ 无法找到主摄像机！");
            }
        }
        else
        {
            Debug.Log("✅ mainCamera正常");
        }

        if (needsFix)
        {
            Debug.Log("🔧 发球机设置已修复！");
            EditorUtility.SetDirty(launcher);
        }
        else
        {
            Debug.Log("✅ 发球机设置完好，无需修复");
        }
    }

    [MenuItem("Tools/Verify Ball Launcher")]
    public static void VerifyBallLauncher()
    {
        Debug.Log("=== 验证发球机状态 ===");

        BallLauncher launcher = Object.FindObjectOfType<BallLauncher>();
        if (launcher == null)
        {
            Debug.LogError("❌ 未找到BallLauncher组件！");
            return;
        }

        Debug.Log($"发球机位置: {launcher.transform.position}");
        Debug.Log($"发球机旋转: {launcher.transform.rotation.eulerAngles}");

        // 检查ballPrefab
        if (launcher.ballPrefab != null)
        {
            Debug.Log($"✅ ballPrefab: {launcher.ballPrefab.name}");
        }
        else
        {
            Debug.LogError("❌ ballPrefab为空！");
        }

        // 检查launchPoint
        if (launcher.launchPoint != null)
        {
            Debug.Log($"✅ launchPoint: {launcher.launchPoint.name} at {launcher.launchPoint.position}");
        }
        else
        {
            Debug.LogError("❌ launchPoint为空！");
        }

        // 检查mainCamera
        if (launcher.mainCamera != null)
        {
            Debug.Log($"✅ mainCamera: {launcher.mainCamera.name}");
        }
        else
        {
            Debug.LogError("❌ mainCamera为空！");
        }

        Debug.Log("=== 验证完成 ===");
    }

    static void CreateTennisBallPrefab()
    {
        Debug.Log("🏗️ 创建TennisBall预制体...");

        // 创建球体
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "TennisBall";
        ball.transform.localScale = Vector3.one * 0.067f; // 网球标准大小

        // 添加Rigidbody
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = 0.057f; // 网球标准重量
        rb.drag = 0.1f;
        rb.angularDrag = 0.05f;

        // 创建材质
        Material ballMaterial = new Material(Shader.Find("Standard"));
        ballMaterial.color = Color.yellow;
        ballMaterial.name = "TennisBallMaterial";

        // 应用材质
        Renderer renderer = ball.GetComponent<Renderer>();
        renderer.material = ballMaterial;

        // 确保Prefabs文件夹存在
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // 保存为预制体
        string prefabPath = "Assets/Prefabs/TennisBall.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(ball, prefabPath);

        // 删除场景中的临时对象
        Object.DestroyImmediate(ball);

        Debug.Log($"✅ TennisBall预制体已创建: {prefabPath}");

        // 刷新资源数据库
        AssetDatabase.Refresh();
    }
}