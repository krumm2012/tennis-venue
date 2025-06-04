using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// 发射调用追踪器 - 专门用于诊断重复发射问题
/// </summary>
public class LaunchCallTracker : MonoBehaviour
{
    [Header("追踪配置")]
    public bool enableTracking = true;
    public bool showDetailedLogs = true;
    public int maxCallHistory = 20;
    
    private static LaunchCallTracker instance;
    private List<LaunchCallInfo> callHistory = new List<LaunchCallInfo>();
    private int frameCounter = 0;
    
    [System.Serializable]
    public class LaunchCallInfo
    {
        public string source;
        public string stackTrace;
        public float timeStamp;
        public int frameNumber;
        public Vector3 targetPosition;
        
        public LaunchCallInfo(string source, string stackTrace, Vector3 target)
        {
            this.source = source;
            this.stackTrace = stackTrace;
            this.timeStamp = Time.time;
            this.frameNumber = Time.frameCount;
            this.targetPosition = target;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        UnityEngine.Debug.Log("=== 发射调用追踪器启动 ===");
        UnityEngine.Debug.Log("快捷键:");
        UnityEngine.Debug.Log("  F12: 显示发射调用历史");
        UnityEngine.Debug.Log("  Shift+F12: 清除调用历史");
        UnityEngine.Debug.Log("  Ctrl+F12: 切换详细日志");
    }
    
    void Update()
    {
        frameCounter++;
        
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                ClearCallHistory();
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                ToggleDetailedLogs();
            }
            else
            {
                ShowCallHistory();
            }
        }
    }
    
    /// <summary>
    /// 记录发射调用
    /// </summary>
    public static void RecordLaunchCall(string source, Vector3 targetPosition)
    {
        if (instance == null || !instance.enableTracking) return;
        
        // 获取调用堆栈
        StackTrace stackTrace = new StackTrace(2, true);
        string stackInfo = stackTrace.ToString();
        
        LaunchCallInfo callInfo = new LaunchCallInfo(source, stackInfo, targetPosition);
        instance.callHistory.Add(callInfo);
        
        // 限制历史记录数量
        if (instance.callHistory.Count > instance.maxCallHistory)
        {
            instance.callHistory.RemoveAt(0);
        }
        
        if (instance.showDetailedLogs)
        {
            UnityEngine.Debug.Log($"🎾 [LaunchCall #{Time.frameCount}] 来源: {source}, 目标: {targetPosition}, 时间: {Time.time:F3}s");
        }
        
        // 检查是否有重复调用（同一帧内多次调用）
        instance.CheckForDuplicateCalls();
    }
    
    /// <summary>
    /// 检查重复调用
    /// </summary>
    void CheckForDuplicateCalls()
    {
        if (callHistory.Count < 2) return;
        
        var lastCall = callHistory[callHistory.Count - 1];
        var secondLastCall = callHistory[callHistory.Count - 2];
        
        // 检查同一帧内的重复调用
        if (lastCall.frameNumber == secondLastCall.frameNumber)
        {
            UnityEngine.Debug.LogWarning($"⚠️ 检测到同一帧内重复发射调用! 帧#{lastCall.frameNumber}");
            UnityEngine.Debug.LogWarning($"   第一次调用: {secondLastCall.source}");
            UnityEngine.Debug.LogWarning($"   第二次调用: {lastCall.source}");
        }
        
        // 检查短时间内的快速连续调用（0.1秒内）
        if (lastCall.timeStamp - secondLastCall.timeStamp < 0.1f)
        {
            UnityEngine.Debug.LogWarning($"⚠️ 检测到快速连续发射调用! 间隔: {(lastCall.timeStamp - secondLastCall.timeStamp) * 1000:F1}ms");
            UnityEngine.Debug.LogWarning($"   第一次调用: {secondLastCall.source} (帧#{secondLastCall.frameNumber})");
            UnityEngine.Debug.LogWarning($"   第二次调用: {lastCall.source} (帧#{lastCall.frameNumber})");
        }
    }
    
    /// <summary>
    /// 显示调用历史
    /// </summary>
    void ShowCallHistory()
    {
        UnityEngine.Debug.Log("=== 发射调用历史 ===");
        
        if (callHistory.Count == 0)
        {
            UnityEngine.Debug.Log("📋 暂无发射调用记录");
            return;
        }
        
        UnityEngine.Debug.Log($"📊 总计 {callHistory.Count} 次发射调用:");
        
        for (int i = callHistory.Count - 1; i >= 0; i--)
        {
            var call = callHistory[i];
            UnityEngine.Debug.Log($"   {callHistory.Count - i}. {call.source} | 帧#{call.frameNumber} | {call.timeStamp:F3}s | 目标:{call.targetPosition}");
        }
        
        // 分析调用模式
        AnalyzeCallPatterns();
    }
    
    /// <summary>
    /// 分析调用模式
    /// </summary>
    void AnalyzeCallPatterns()
    {
        if (callHistory.Count < 2) return;
        
        UnityEngine.Debug.Log("=== 调用模式分析 ===");
        
        // 统计调用来源
        Dictionary<string, int> sourceCounts = new Dictionary<string, int>();
        foreach (var call in callHistory)
        {
            if (!sourceCounts.ContainsKey(call.source))
                sourceCounts[call.source] = 0;
            sourceCounts[call.source]++;
        }
        
        UnityEngine.Debug.Log("📈 调用来源统计:");
        foreach (var kvp in sourceCounts)
        {
            UnityEngine.Debug.Log($"   {kvp.Key}: {kvp.Value} 次");
        }
        
        // 检查连续的相同来源调用
        int maxConsecutive = 1;
        int currentConsecutive = 1;
        string lastSource = callHistory[0].source;
        
        for (int i = 1; i < callHistory.Count; i++)
        {
            if (callHistory[i].source == lastSource)
            {
                currentConsecutive++;
                maxConsecutive = Mathf.Max(maxConsecutive, currentConsecutive);
            }
            else
            {
                currentConsecutive = 1;
                lastSource = callHistory[i].source;
            }
        }
        
        if (maxConsecutive > 1)
        {
            UnityEngine.Debug.LogWarning($"⚠️ 检测到最多 {maxConsecutive} 次连续的相同来源调用");
        }
        
        // 检查时间间隔
        List<float> intervals = new List<float>();
        for (int i = 1; i < callHistory.Count; i++)
        {
            intervals.Add(callHistory[i].timeStamp - callHistory[i-1].timeStamp);
        }
        
        if (intervals.Count > 0)
        {
            float avgInterval = 0f;
            foreach (float interval in intervals)
            {
                avgInterval += interval;
            }
            avgInterval /= intervals.Count;
            
            UnityEngine.Debug.Log($"⏱️ 平均调用间隔: {avgInterval:F3}秒");
            
            // 检查异常短的间隔
            int shortIntervals = 0;
            foreach (float interval in intervals)
            {
                if (interval < 0.1f) shortIntervals++;
            }
            
            if (shortIntervals > 0)
            {
                UnityEngine.Debug.LogWarning($"⚠️ 发现 {shortIntervals} 次异常短的调用间隔 (<0.1秒)");
            }
        }
    }
    
    /// <summary>
    /// 清除调用历史
    /// </summary>
    void ClearCallHistory()
    {
        callHistory.Clear();
        UnityEngine.Debug.Log("🧹 发射调用历史已清除");
    }
    
    /// <summary>
    /// 切换详细日志
    /// </summary>
    void ToggleDetailedLogs()
    {
        showDetailedLogs = !showDetailedLogs;
        UnityEngine.Debug.Log($"📝 详细日志: {(showDetailedLogs ? "开启" : "关闭")}");
    }
    
    /// <summary>
    /// GUI显示实时信息
    /// </summary>
    void OnGUI()
    {
        if (!enableTracking) return;
        
        GUILayout.BeginArea(new Rect(10, Screen.height - 120, 300, 110));
        
        GUILayout.Label("=== 发射调用追踪 ===", new GUIStyle() { fontSize = 12, normal = { textColor = Color.cyan } });
        
        GUILayout.Label($"总调用次数: {callHistory.Count}",
            new GUIStyle() { normal = { textColor = Color.white } });
        
        if (callHistory.Count > 0)
        {
            var lastCall = callHistory[callHistory.Count - 1];
            GUILayout.Label($"最后调用: {lastCall.source}",
                new GUIStyle() { normal = { textColor = Color.yellow } });
            GUILayout.Label($"调用时间: {lastCall.timeStamp:F3}s (帧#{lastCall.frameNumber})",
                new GUIStyle() { normal = { textColor = Color.gray } });
        }
        
        GUILayout.Space(5);
        GUILayout.Label("F12: 显示历史 | Shift+F12: 清除 | Ctrl+F12: 切换日志", 
            new GUIStyle() { fontSize = 10, normal = { textColor = Color.white } });
        
        GUILayout.EndArea();
    }
}