using UnityEngine;
using TMPro;
using System.Collections;

public class FlightTimeTracker : MonoBehaviour
{
    public TextMeshProUGUI flightTimeText;
    public TextMeshProUGUI totalFlightText;
    
    public bool isTrackingFlight = false;
    private float flightStartTime;
    private GameObject currentBall;
    
    void Start()
    {
        InitializeFlightTimeUI();
        StartCoroutine(MonitorBallLaunch());
    }
    
    void InitializeFlightTimeUI()
    {
        Debug.Log("初始化飞行时间追踪系统");
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        // 创建实时飞行时间显示
        GameObject textObj = new GameObject("FlightTimeText");
        textObj.transform.SetParent(canvas.transform, false);
        textObj.layer = 5;
        
        flightTimeText = textObj.AddComponent<TextMeshProUGUI>();
        flightTimeText.text = "飞行时间: 0.00s";
        flightTimeText.fontSize = 16;
        flightTimeText.color = Color.yellow;
        
        RectTransform rectTransform = flightTimeText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(20, -140f);
        rectTransform.sizeDelta = new Vector2(250, 30);
        
        // 创建总飞行时间显示
        GameObject totalObj = new GameObject("TotalFlightText");
        totalObj.transform.SetParent(canvas.transform, false);
        totalObj.layer = 5;
        
        totalFlightText = totalObj.AddComponent<TextMeshProUGUI>();
        totalFlightText.text = "上次飞行: --";
        totalFlightText.fontSize = 16;
        totalFlightText.color = Color.cyan;
        
        RectTransform totalRect = totalFlightText.GetComponent<RectTransform>();
        totalRect.anchorMin = new Vector2(0, 1);
        totalRect.anchorMax = new Vector2(0, 1);
        totalRect.anchoredPosition = new Vector2(20, -170f);
        totalRect.sizeDelta = new Vector2(250, 30);
        
        Debug.Log("飞行时间UI已创建");
    }
    
    IEnumerator MonitorBallLaunch()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                yield return new WaitForEndOfFrame();
                StartFlightTimeTracking();
            }
            
            if (isTrackingFlight)
            {
                UpdateFlightTimeDisplay();
                CheckBallStatus();
            }
            
            yield return null;
        }
    }
    
    public void StartFlightTimeTracking()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null && rb.velocity.magnitude > 1f)
                {
                    currentBall = obj;
                    isTrackingFlight = true;
                    flightStartTime = Time.time;
                    flightTimeText.color = Color.green;
                    Debug.Log("开始追踪网球飞行时间");
                    break;
                }
            }
        }
    }
    
    void UpdateFlightTimeDisplay()
    {
        if (flightTimeText != null)
        {
            float currentTime = Time.time - flightStartTime;
            flightTimeText.text = $"飞行时间: {currentTime:F2}s";
        }
    }
    
    void CheckBallStatus()
    {
        if (currentBall == null)
        {
            StopFlightTimeTracking();
            return;
        }
        
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float speed = rb.velocity.magnitude;
            float height = currentBall.transform.position.y;
            float flightTime = Time.time - flightStartTime;
            
            if (speed < 0.5f || height < 0.1f || flightTime > 10f)
            {
                StopFlightTimeTracking();
            }
        }
    }
    
    void StopFlightTimeTracking()
    {
        if (isTrackingFlight)
        {
            float totalFlightTime = Time.time - flightStartTime;
            isTrackingFlight = false;
            
            flightTimeText.text = "飞行时间: 0.00s";
            flightTimeText.color = Color.yellow;
            
            totalFlightText.text = $"上次飞行: {totalFlightTime:F2}s";
            
            Debug.Log($"飞行结束，总时间: {totalFlightTime:F2}s");
        }
    }
}