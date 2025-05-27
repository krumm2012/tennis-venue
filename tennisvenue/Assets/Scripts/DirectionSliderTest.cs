using UnityEngine;
using UnityEngine.UI;

public class DirectionSliderTest : MonoBehaviour
{
    public Slider directionSlider;
    
    void Start()
    {
        if (directionSlider != null)
        {
            Debug.Log("DirectionSliderTest: Slider found!");
            directionSlider.onValueChanged.AddListener(OnValueChanged);
        }
        else
        {
            Debug.LogError("DirectionSliderTest: Slider is null!");
        }
    }
    
    void OnValueChanged(float value)
    {
        Debug.Log($"DirectionSliderTest: Value changed to {value}");
    }
}