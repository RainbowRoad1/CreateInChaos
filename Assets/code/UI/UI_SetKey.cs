using System.Collections;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SetKey : MonoBehaviour, IPointerClickHandler
{
    public GameBase.input_key key;
    public MonitorKey m_key;
    private bool is_press;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!is_press)
        {
            GetComponent<Image>().color = Color.gray;
            is_press = true;
        }
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Destroy(this);
            return;
        }
        m_key = MainInput.get_key(key);
        GetComponentInChildren<Text>().text = m_key.key.ToString();
        Toggle stop = transform.Find("../Toggle").GetComponent<Toggle>();
        stop.isOn = m_key.mode;
        stop.onValueChanged.AddListener(v => m_key.mode = v);
    }

    void Update()
    {
        if (is_press && Input.anyKeyDown)
        {
            is_press = false;
            GetComponent<Image>().color = Color.white;
            if (Input.GetKey(KeyCode.Escape)) return;
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keycode))
                {
                    GetComponentInChildren<Text>().text = keycode.ToString();
                    m_key.key = keycode;
                    MainConfig.main.set_data($"key{(int)key + 1}", keycode.ToString());
                    MainConfig.main.need_update = true;
                    return;
                }
            }
        }
    }
}