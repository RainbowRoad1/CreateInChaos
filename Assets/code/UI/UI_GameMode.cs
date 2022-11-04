using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class UI_GameMode : MonoBehaviour
{
    private UI_Text info;
    public string[] option_info = new string[5];
    private GameObject go_save;
    private RectTransform go_save_rt;
    void Start()
    {
        info = gameObject.AddComponent<UI_Text>();
        info.text_name = "all_null";
        UI_GameMode_Option.menu = this;
        transform.Find("mode1").gameObject.AddComponent<UI_GameMode_Option>().start(0, option_info[0]);
        transform.Find("mode2").gameObject.AddComponent<UI_GameMode_Option>().start(1, option_info[1]);
        transform.Find("mode3").gameObject.AddComponent<UI_GameMode_Option>().start(2, option_info[2]);
        transform.Find("mode4").gameObject.AddComponent<UI_GameMode_Option>().start(3, option_info[3]);
        transform.Find("mode5").gameObject.AddComponent<UI_GameMode_Option>().start(4, option_info[4]);
    }
    void go_save_init()
    {
        if (go_save != null) return;
        go_save = transform.Find("save").gameObject;
        go_save.AddComponent<UI_GameMode_Option>().start(-1, "game_save_info");
        go_save_rt = go_save.GetComponent<RectTransform>();
        go_save_rt.localPosition = Vector2.left * (160 * (2 - GameBase.game_mode));
    }
    public void update_info(string text)
    {
        info.text_name = text;
        info.reset_text();
    }
    public void run_mode(int mode)
    {
        if (mode == -1) mode = GameBase.game_mode;
        else if (GameBase.main.save_data.Length > 0)
        {
            UI_PopUp.menu.set_display(UI_PopUp.display.OverwriteSave);
            UI_PopUp.run_event = () =>
            {
                GameBase.main.save_data = "";
                run_mode(mode);
            };
            return;
        }
        else
        {
            string path = Application.persistentDataPath + "\\save.txt";
            if (File.Exists(path)) File.Delete(path);
            PlayerPrefs.SetInt("Mode", mode);
            GameBase.game_mode = mode;
            go_save_rt.localPosition = Vector2.left * (160 * (2 - mode));
        }
        switch (mode)
        {
            case 0: GameBase.main_ui.game_start(10, 20); break;
            case 1: GameBase.main_ui.game_start(8, 16); break;
            case 2: GameBase.main_ui.game_start(12, 24); break;
            case 3:
                GameBase.main.fall_speed = 1f;
                GameBase.status.use_day = true;
                BlockData.create_func = BlockData.get_random_block_redstone;
                GameBase.main_ui.game_start(10, 20, true);
                break;
            case 4: GameBase.main_ui.show_menu_main(3); break;
        }
    }

    private void OnEnable()
    {
        go_save_init();
        string path = Application.persistentDataPath + "\\save.txt";
        if (GameBase.main.save_data.Length < 1 && File.Exists(path))
        {
            GameBase.main.save_data = File.ReadAllText(path);
        }
        go_save.SetActive(GameBase.main.save_data.Length > 0);
    }

    private void OnDisable()
    {
        update_info("all_null");
    }
}

public class UI_GameMode_Option : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    static public UI_GameMode menu;
    static private float time_max = 0.5f;
    private int mode_id;
    private string info_text;
    private float enter_time;
    private bool is_enter;
    private Image image;
    public void start(int mode_id, string info_text)
    {
        this.mode_id = mode_id;
        this.info_text = info_text;
        image = GetComponent<Image>();
        image.color = new Color(1, 1, 1, 0.3f);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.update_info(info_text);
        is_enter = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        is_enter = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        menu.run_mode(mode_id);
    }
    private void Update()
    {
        if (is_enter)
        {
            if (enter_time == time_max) return;
            enter_time = Mathf.Min(time_max, enter_time + Time.deltaTime);
        }
        else
        {
            if (enter_time == 0) return;
            enter_time = Mathf.Max(0, enter_time - Time.deltaTime);
        }
        image.color = new Color(1, 1, 1, Mathf.Lerp(0.3f, 1, enter_time / time_max));
    }
}