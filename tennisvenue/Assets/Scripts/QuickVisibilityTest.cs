using UnityEngine;

/// <summary>
/// 快速可见性测试 - 立即诊断圆环可见性问题
/// </summary>
public class QuickVisibilityTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 快速可见性测试启动 ===");
        Debug.Log("快捷键:");
        Debug.Log("  F7: 创建超大可见测试圆环");
        Debug.Log("  F8: 检查现有圆环");
        Debug.Log("  F9: 修复圆环可见性");

        // 立即运行诊断
        Invoke("RunQuickDiagnostic", 1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            CreateSuperVisibleRing();
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            CheckAllRings();
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            FixAllRings();
        }
    }

    void RunQuickDiagnostic()
    {
        Debug.Log("🔍 运行快速诊断...");

        // 检查摄像机
        Camera cam = Camera.main;
        if (cam != null)
        {
            Debug.Log($"📷 摄像机位置: {cam.transform.position}");
            Debug.Log($"📷 摄像机朝向: {cam.transform.forward}");
        }

        // 检查现有圆环
        CheckAllRings();
    }

    void CreateSuperVisibleRing()
    {
        Debug.Log("🎯 创建超大可见测试圆环");

        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "SuperVisibleTestRing";

        // 设置在场地中央，地面上方
        ring.transform.position = new Vector3(0, 0.5f, 0);
        ring.transform.localScale = new Vector3(2f, 0.2f, 2f);

        // 设置超亮材质
        Renderer renderer = ring.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.cyan;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.cyan * 5f);
        renderer.material = mat;

        // 15秒后销毁
        Destroy(ring, 15f);

        Debug.Log("✅ 超大测试圆环已创建！");
        Debug.Log("   位置: (0, 0.5, 0)");
        Debug.Log("   大小: 2m直径");
        Debug.Log("   如果看不到，请按T键切换俯视角度");
    }

    void CheckAllRings()
    {
        Debug.Log("🔍 检查所有圆环标记...");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int ringCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring") || obj.name.Contains("Impact"))
            {
                ringCount++;
                Debug.Log($"🔍 找到圆环: {obj.name}");
                Debug.Log($"   位置: {obj.transform.position}");
                Debug.Log($"   大小: {obj.transform.localScale}");
                Debug.Log($"   激活: {obj.activeInHierarchy}");

                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Debug.Log($"   渲染器启用: {renderer.enabled}");
                    Debug.Log($"   材质颜色: {renderer.material.color}");
                }
                else
                {
                    Debug.LogError($"   ❌ 没有渲染器！");
                }
            }
        }

        Debug.Log($"📊 总共找到 {ringCount} 个圆环对象");

        if (ringCount == 0)
        {
            Debug.LogWarning("⚠️ 没有找到任何圆环标记！");
            Debug.Log("   可能原因:");
            Debug.Log("   1. 圆环创建失败");
            Debug.Log("   2. 圆环已被销毁");
            Debug.Log("   3. 圆环名称不匹配");
        }
    }

    void FixAllRings()
    {
        Debug.Log("🔧 修复所有圆环可见性...");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int fixedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring"))
            {
                // 确保激活
                obj.SetActive(true);

                // 调整位置到地面上方
                Vector3 pos = obj.transform.position;
                pos.y = Mathf.Max(pos.y, 0.1f);
                obj.transform.position = pos;

                // 确保大小足够大
                Vector3 scale = obj.transform.localScale;
                if (scale.magnitude < 1f)
                {
                    scale = Vector3.one;
                    obj.transform.localScale = scale;
                }

                // 修复材质
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                    Material mat = renderer.material;

                    // 设置不透明
                    Color color = mat.color;
                    color.a = 1f;
                    mat.color = color;

                    // 增强发光
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", color * 3f);
                }

                fixedCount++;
                Debug.Log($"✅ 修复圆环: {obj.name}");
            }
        }

        Debug.Log($"✅ 已修复 {fixedCount} 个圆环");
    }
}