using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_PopUp : MonoBehaviour
{
    static public UI_PopUp menu;
    static public UnityAction run_event;
    public enum display { OverwriteSave, NoUseBlock, HighAspect };
    private UI_Text text;
    private Button btn1, btn2;
    void Start()
    {
        if (menu == null) start();
    }
    public void start()
    {
        menu = this;
        text = gameObject.AddComponent<UI_Text>();
        text.text_name = "all_null";
        btn1 = transform.Find("LeftB").GetComponent<Button>();
        btn2 = transform.Find("RightB").GetComponent<Button>();
        btn1.onClick.AddListener(() => run_event());
        btn1.onClick.AddListener(() => gameObject.SetActive(false));
        btn2.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false);
    }
    public void set_display(display display)
    {
        btn2.interactable = true;
        switch (display)
        {
            case display.OverwriteSave:
                btn1.interactable = true;
                text.text_name = "pop_overwrite_save";
                break;
            case display.NoUseBlock:
                btn1.interactable = false;
                text.text_name = "pop_no_useblock";
                break;
            case display.HighAspect:
                btn1.interactable = true;
                text.text_name = "pop_high_aspect";
                break;
        }
        text.reset_text();
        gameObject.SetActive(true);
    }
}
