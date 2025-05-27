using UnityEngine;
using System.Collections;

public class PlayerModel : MonoBehaviour
{
    public float playerHeight = 1.75f;
    public float racketHeight = 0.8f;
    public float swingDuration = 0.8f;
    
    public GameObject bodyObject;
    public GameObject headObject;
    public GameObject racketObject;
    
    private Transform racketTransform;
    private Vector3 initialRacketPosition;
    private Vector3 initialRacketRotation;
    private bool isSwinging = false;
    
    void Start()
    {
        CreatePlayerModel();
    }
    
    void CreatePlayerModel()
    {
        Debug.Log("创建175cm身高人物模型");
        
        transform.position = new Vector3(0, 0, 3);
        
        CreatePlayerBody();
        CreatePlayerHead();
        CreateTennisRacket();
        
        Debug.Log($"人物模型已创建，位置: {transform.position}");
    }
    
    void CreatePlayerBody()
    {
        bodyObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bodyObject.name = "PlayerBody";
        bodyObject.transform.SetParent(this.transform);
        bodyObject.transform.localPosition = new Vector3(0, 0.875f, 0);
        bodyObject.transform.localScale = new Vector3(0.3f, 0.875f, 0.3f);
        
        Renderer bodyRenderer = bodyObject.GetComponent<Renderer>();
        bodyRenderer.material = new Material(Shader.Find("Standard"));
        bodyRenderer.material.color = new Color(0.2f, 0.4f, 0.8f);
    }
    
    void CreatePlayerHead()
    {
        headObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        headObject.name = "PlayerHead";
        headObject.transform.SetParent(this.transform);
        headObject.transform.localPosition = new Vector3(0, 1.65f, 0);
        headObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        
        Renderer headRenderer = headObject.GetComponent<Renderer>();
        headRenderer.material = new Material(Shader.Find("Standard"));
        headRenderer.material.color = new Color(0.9f, 0.7f, 0.6f);
    }
    
    void CreateTennisRacket()
    {
        racketObject = new GameObject("TennisRacket");
        racketObject.transform.SetParent(this.transform);
        racketObject.transform.localPosition = new Vector3(0.3f, racketHeight, 0);
        
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        handle.name = "RacketHandle";
        handle.transform.SetParent(racketObject.transform);
        handle.transform.localPosition = Vector3.zero;
        handle.transform.localScale = new Vector3(0.03f, 0.15f, 0.03f);
        handle.transform.localRotation = Quaternion.Euler(90, 0, 0);
        
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        head.name = "RacketHead";
        head.transform.SetParent(racketObject.transform);
        head.transform.localPosition = new Vector3(0, 0, 0.15f);
        head.transform.localScale = new Vector3(0.12f, 0.01f, 0.15f);
        
        racketTransform = racketObject.transform;
        initialRacketPosition = racketTransform.localPosition;
        initialRacketRotation = racketTransform.localEulerAngles;
    }
    
    public void OnBallLanded(float flightTime)
    {
        Debug.Log($"球已落地，触发挥拍动作");
        TriggerSwing();
    }
    
    public void TriggerSwing()
    {
        if (!isSwinging && racketTransform != null)
        {
            StartCoroutine(SwingAnimation());
        }
    }
    
    IEnumerator SwingAnimation()
    {
        isSwinging = true;
        float elapsed = 0f;
        
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / swingDuration;
            float swingCurve = Mathf.Sin(progress * Mathf.PI);
            
            Vector3 swingOffset = new Vector3(-0.2f * swingCurve, 0.1f * swingCurve, 0.3f * swingCurve);
            racketTransform.localPosition = initialRacketPosition + swingOffset;
            
            Vector3 swingRotation = new Vector3(-30f * swingCurve, 20f * swingCurve, 0f);
            racketTransform.localEulerAngles = initialRacketRotation + swingRotation;
            
            yield return null;
        }
        
        racketTransform.localPosition = initialRacketPosition;
        racketTransform.localEulerAngles = initialRacketRotation;
        isSwinging = false;
        
        AnalyzeOptimalHitHeight();
    }
    
    void AnalyzeOptimalHitHeight()
    {
        float shoulderHeight = playerHeight * 0.8f;
        float optimalHitHeight = shoulderHeight + 0.42f;
        
        Debug.Log($"击球高度分析（175cm人物）");
        Debug.Log($"最适宜击球高度: {optimalHitHeight:F2}m");
        Debug.Log($"舒适击球范围: {optimalHitHeight - 0.3f:F2}m - {optimalHitHeight + 0.3f:F2}m");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TriggerSwing();
        }
    }
}