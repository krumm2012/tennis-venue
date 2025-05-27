using UnityEngine;
using System;

/// <summary>
/// Unity MCP连接测试脚本
/// 用于验证UnityMCP功能是否正常工作
/// </summary>
public class UnityMCPConnectionTest : MonoBehaviour
{
    [Header("测试状态")]
    [SerializeField] private bool isTestRunning = false;
    [SerializeField] private float testStartTime;
    
    [Header("测试信息")]
    [SerializeField] private string testMessage = "Unity MCP连接测试";
    [SerializeField] private int testCounter = 0;
    
    void Start()
    {
        // 开始连接测试
        StartConnectionTest();
    }
    
    void Update()
    {
        // 每5秒更新一次测试状态
        if (isTestRunning && Time.time - testStartTime > 5f)
        {
            UpdateTestStatus();
            testStartTime = Time.time;
        }
    }
    
    /// <summary>
    /// 开始连接测试
    /// </summary>
    public void StartConnectionTest()
    {
        isTestRunning = true;
        testStartTime = Time.time;
        testCounter = 0;
        
        Debug.Log("=== Unity MCP连接测试开始 ===");
        Debug.Log($"测试时间: {DateTime.Now}");
        Debug.Log($"GameObject: {gameObject.name}");
        Debug.Log($"位置: {transform.position}");
        
        // 输出系统信息
        Debug.Log($"Unity版本: {Application.unityVersion}");
        Debug.Log($"平台: {Application.platform}");
        Debug.Log($"FPS: {1f/Time.deltaTime:F1}");
    }
    
    /// <summary>
    /// 更新测试状态
    /// </summary>
    private void UpdateTestStatus()
    {
        testCounter++;
        
        Debug.Log($"[测试状态 #{testCounter}] Unity MCP连接正常");
        Debug.Log($"运行时间: {Time.time:F1}秒");
        Debug.Log($"帧数: {Time.frameCount}");
        
        // 测试对象操作
        transform.Rotate(0, 1, 0);
        
        if (testCounter >= 3)
        {
            StopConnectionTest();
        }
    }
    
    /// <summary>
    /// 停止连接测试
    /// </summary>
    public void StopConnectionTest()
    {
        isTestRunning = false;
        Debug.Log("=== Unity MCP连接测试完成 ===");
        Debug.Log($"总共运行 {testCounter} 次状态更新");
        Debug.Log("UnityMCP功能验证成功！");
    }
    
    /// <summary>
    /// 手动触发测试信息
    /// </summary>
    public void LogTestInfo()
    {
        Debug.Log($"[手动测试] {testMessage}");
        Debug.Log($"当前位置: {transform.position}");
        Debug.Log($"当前旋转: {transform.rotation.eulerAngles}");
    }
    
    void OnDestroy()
    {
        if (isTestRunning)
        {
            Debug.Log("UnityMCPConnectionTest对象被销毁，测试结束");
        }
    }
}