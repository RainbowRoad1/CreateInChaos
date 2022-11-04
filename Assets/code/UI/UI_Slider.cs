using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Slider : MonoBehaviour
{
    private const string text_path = "Number";
    private const string slider_path = "Slider";
    public Text text;
    public Slider slider;
    public string end_sign = "%";
    public bool pass_init;
    void Start()
    {
        run_init();
    }

    public void run_init()
    {
        if (pass_init) return;
        pass_init = true;
        if (text == null) text = transform.Find(text_path).GetComponent<Text>();
        if (slider == null) slider = transform.Find(slider_path).GetComponent<Slider>();
        if (slider.wholeNumbers)
            slider.onValueChanged.AddListener(v => text.text = v + end_sign);
        else slider.onValueChanged.AddListener(v => text.text = Mathf.Round(v * 100) / 100f + end_sign);
        slider.onValueChanged.Invoke(slider.value);
    }
    
    public Slider.SliderEvent get_slider_call()
    {
        run_init();
        return slider.onValueChanged;
    }
}
