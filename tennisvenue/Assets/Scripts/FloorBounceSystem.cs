using UnityEngine;

/// <summary>
/// 地面反弹系统 - 聚氨酯地板6mm厚
/// </summary>
public class FloorBounceSystem : MonoBehaviour
{
    [Header("聚氨酯地板设置")]
    public PhysicMaterial polyurethaneFloorMaterial;
    public GameObject floorObject;

    void Start()
    {
        InitializeFloorBounce();
    }

    void InitializeFloorBounce()
    {
        Debug.Log("=== 初始化聚氨酯地板反弹系统 ===");

        // 创建聚氨酯地板物理材质
        CreatePolyurethaneFloorMaterial();

        // 应用到地面
        ApplyToFloor();

        // 改进网球物理属性
        ImproveballPhysics();
    }

    /// <summary>
    /// 创建聚氨酯地板6mm厚的物理材质
    /// </summary>
    void CreatePolyurethaneFloorMaterial()
    {
        polyurethaneFloorMaterial = new PhysicMaterial("PolyurethaneFloor_6mm");

        // 聚氨酯地板特性（6mm厚度）：
        polyurethaneFloorMaterial.dynamicFriction = 0.75f; // 动摩擦系数
        polyurethaneFloorMaterial.staticFriction = 0.8f;   // 静摩擦系数
        polyurethaneFloorMaterial.bounciness = 0.75f;      // 反弹系数（6mm聚氨酯，增加反弹）
        polyurethaneFloorMaterial.frictionCombine = PhysicMaterialCombine.Average;
        polyurethaneFloorMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

        Debug.Log("聚氨酯地板物理材质已创建 - 6mm厚度，反弹系数: 0.75，反弹组合: Maximum");
    }

    /// <summary>
    /// 应用物理材质到地面
    /// </summary>
    void ApplyToFloor()
    {
        if (floorObject == null)
        {
            floorObject = GameObject.Find("Floor");
        }

        if (floorObject != null)
        {
            Collider floorCollider = floorObject.GetComponent<Collider>();
            if (floorCollider != null)
            {
                floorCollider.material = polyurethaneFloorMaterial;
                Debug.Log("地面已应用聚氨酯物理材质");
            }
            else
            {
                Debug.LogWarning("地面对象缺少Collider组件");
            }
        }
        else
        {
            Debug.LogWarning("未找到地面对象 'Floor'");
        }
    }

    /// <summary>
    /// 改进网球物理属性以配合地面反弹
    /// </summary>
    void ImproveballPhysics()
    {
        // 查找网球预制体
        GameObject tennisBall = GameObject.Find("TennisBall");
        if (tennisBall != null)
        {
            // 确保网球有合适的物理属性
            Rigidbody rb = tennisBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = 0.057f; // 标准网球重量57克
                rb.drag = 0.02f;   // 减少空气阻力（原来0.1太高）
                rb.angularDrag = 0.02f; // 减少角阻力
            }

            // 创建网球物理材质
            Collider ballCollider = tennisBall.GetComponent<Collider>();
            if (ballCollider != null)
            {
                PhysicMaterial ballMaterial = new PhysicMaterial("TennisBall");
                ballMaterial.dynamicFriction = 0.6f;
                ballMaterial.staticFriction = 0.6f;
                ballMaterial.bounciness = 0.85f; // 增加网球本身的反弹（0.8 → 0.85）
                ballMaterial.frictionCombine = PhysicMaterialCombine.Average;
                ballMaterial.bounceCombine = PhysicMaterialCombine.Maximum; // 改为Maximum确保最佳反弹

                ballCollider.material = ballMaterial;
                Debug.Log("网球物理材质已优化 - 减少阻力，增强反弹效果");
            }
        }
        else
        {
            Debug.LogWarning("未找到网球对象 'TennisBall'");
        }
    }
}