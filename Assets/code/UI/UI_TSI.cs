using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_TSI : MonoBehaviour
{
    // TSI: text + slider + inputfield
    private Text text;
    private Slider slider;
    public string attr_name;
    [HideInInspector]
    public InputField input;
    public Vector3 slider_range = Vector3.up;
    public bool upper_limit = true;
    void Start()
    {
        text = transform.Find("Text").GetComponent<Text>();
        slider = transform.Find("Slider").GetComponent<Slider>();
        input = transform.Find("InputField").GetComponent<InputField>();
        slider.minValue = slider_range.x;
        slider.maxValue = slider_range.y;
        slider.onValueChanged.AddListener(v => input.text = v.ToString());
        slider.value = slider_range.z;
        input.onEndEdit.AddListener(v =>
        {
            float t = v.Length == 0 ? 0 : float.Parse(v);
            if (t < slider_range.x) t = slider_range.x;
            if (upper_limit && t > slider_range.y) t = slider_range.y;
            slider.value = t;
            input.text = t.ToString();
        });
    }
}
