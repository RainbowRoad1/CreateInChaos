using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GetKey : GameBase
{
    public input_key key;
    private UI_Button button;
    void Start()
    {
        button = GetComponent<UI_Button>();
        button.on_press.AddListener(() => MainInput.get_key(key).press());
    }

    void Update()
    {
        if (button.is_down) MainInput.get_key(key).press();
    }
}
