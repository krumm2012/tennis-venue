using UnityEngine;

/// <summary>
/// 用于测试轨迹线功能的调试脚本
/// </summary>
public class TrajectoryTester : MonoBehaviour
{
    [Header("测试设置")]
    public BallLauncher ballLauncher;
    public bool showDebugInfo = true;
    
    void Start()
    {
        if (ballLauncher == null)
        {
            ballLauncher = FindObjectOfType<BallLauncher>();
        }
        
        if (ballLauncher != null)
        {
            Debug.Log("轨迹测试器启动成功");
            Debug.Log($"BallLauncher状态: {ballLauncher.name}");
            Debug.Log($"轨迹线: {(ballLauncher.trajectoryLine != null ? "已连接" : "未连接")}");
            Debug.Log($"障碍物图层: {ballLauncher.obstacleLayerMask.value}");
            Debug.Log($"虚线长度: {ballLauncher.dashLength}");
            Debug.Log($"间隙长度: {ballLauncher.gapLength}");
        }
        else
        {
            Debug.LogError("找不到BallLauncher组件!");
        }
    }
    
    void Update()
    {
        if (showDebugInfo && ballLauncher != null && Input.GetKeyDown(KeyCode.I))
        {
            PrintTrajectoryInfo();
        }
    }
    
    void PrintTrajectoryInfo()
    {
        Debug.Log("=== 轨迹线信息 ===");
        Debug.Log($"发球机位置: {ballLauncher.transform.position}");
        Debug.Log($"发球机旋转: {ballLauncher.transform.rotation.eulerAngles}");
        Debug.Log($"发射方向: {ballLauncher.transform.forward}");
        
        if (ballLauncher.trajectoryLine != null)
        {
            Debug.Log($"轨迹点数量: {ballLauncher.trajectoryLine.positionCount}");
            
            // 显示前几个轨迹点
            for (int i = 0; i < Mathf.Min(5, ballLauncher.trajectoryLine.positionCount); i++)
            {
                Vector3 point = ballLauncher.trajectoryLine.GetPosition(i);
                Debug.Log($"轨迹点[{i}]: {point}");
            }
        }
        
        // 测试碰撞检测
        TestCollisionDetection();
    }
    
    void TestCollisionDetection()
    {
        Vector3 launchPos = ballLauncher.transform.position;
        Vector3 launchDir = ballLauncher.transform.forward;
        
        RaycastHit hit;
        LayerMask obstacleLayer = ballLauncher.obstacleLayerMask;
        
        Debug.Log($"测试碰撞检测 - 图层掩码: {obstacleLayer.value}");
        
        if (Physics.Raycast(launchPos, launchDir, out hit, 20f, obstacleLayer))
        {
            Debug.Log($"检测到碰撞: {hit.collider.name} 在距离 {hit.distance:F2} 处");
            Debug.Log($"碰撞点: {hit.point}");
            Debug.Log($"碰撞对象图层: {hit.collider.gameObject.layer}");
        }
        else
        {
            Debug.Log("未检测到碰撞");
        }
    }
}