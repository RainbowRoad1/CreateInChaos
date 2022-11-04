using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SetBasic : MonoBehaviour
{
    static private int[] frame_rate_map = { -1, 120, 90, 60, 30 };
    static private string[] language_map = { "English", "¼òÌåÖÐÎÄ" };
    void Awake()
    {
        transform.Find("BGM volume/Slider").GetComponent<Slider>().onValueChanged.AddListener(v => GameBase.sounds.set_bgm_volume(v / 100f));
        transform.Find("Sound volume/Slider").GetComponent<Slider>().onValueChanged.AddListener(v => GameBase.sounds.effect = v / 100f);
        List<string> list = new List<string>();
        foreach (int i in frame_rate_map) list.Add(i.ToString());
        Dropdown dropdown = transform.Find("Frame rate/Dropdown").GetComponent<Dropdown>();
        dropdown.AddOptions(list);
        dropdown.onValueChanged.AddListener(v => { Application.targetFrameRate = frame_rate_map[v]; });
        list.Clear();
        foreach (var i in language_map) list.Add(i);
        dropdown = transform.Find("language/Dropdown").GetComponent<Dropdown>();
        dropdown.AddOptions(list);
        dropdown.onValueChanged.AddListener(v => GameBase.main_ui.choose_language(v));
        list.Clear();
        if (Application.platform == RuntimePlatform.Android)
        {
            transform.Find("Fill screen/Toggle").GetComponent<Toggle>().interactable = false;
        }
        else
        {
            Toggle toggle = transform.Find("Fill screen/Toggle").GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(v =>
            {
                if (v)
                {
                    Resolution data = Screen.resolutions[Screen.resolutions.Length - 1];
                    Screen.SetResolution(data.width, data.height, true);
                }
                else
                {
                    Screen.SetResolution(1280, 720, false);
                }
            });
            list.Clear();
        }
    }
}
