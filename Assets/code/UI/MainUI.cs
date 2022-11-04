using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MainUI : MonoBehaviour
{
    private UI_Background background;
    private GameObject game_ui, menu_main, menu_more, menu_game, menu_custom, game_UL1, game_UL2, over_ui;
    private GameObject set_title, set_basic, set_gameplay, set_input, info_record, now_main;
    private UI_GetKey key_get1, key_get2, key_get3;
    private Image key_icon1, key_icon2, key_icon3;
    private Sprite key_tex1, key_tex2, key_tex3;
    private TextManageSystem text_stsyem;
    const float height_base = 540;
    private int last_width, last_height;
    private bool esc_up = false, esc_enabled = false, stop_game = false;
    private float esc_down_time = 0;
    public UnityEvent update_screen;
    void Start()
    {
        GameBase.main_ui = this;
        replace_size();
        text_stsyem = new TextManageSystem();
        background = transform.Find("background").GetComponent<UI_Background>();
        menu_main = transform.Find("main").gameObject;
        menu_main.transform.Find("Start").GetComponent<Button>().onClick.AddListener(() => show_menu_main(2));
        menu_main.transform.Find("More").GetComponent<Button>().onClick.AddListener(() => show_menu_main(1));
        menu_main.transform.Find("Quit").GetComponent<Button>().onClick.AddListener(() => Application.Quit());
        menu_more = transform.Find("more").gameObject;
        set_title = menu_more.transform.Find("title").gameObject;
        set_basic = menu_more.transform.Find("set_basic").gameObject;
        set_gameplay = menu_more.transform.Find("set_gameplay").gameObject;
        set_input = menu_more.transform.Find("set_input").gameObject;
        info_record = menu_more.transform.Find("info_record").gameObject;
        menu_more.transform.Find("back").GetComponent<Button>().onClick.AddListener(() => show_menu_main(0));
        set_title.transform.Find("basic").GetComponent<Button>().onClick.AddListener(() => show_menu_set(0));
        set_title.transform.Find("gameplay").GetComponent<Button>().onClick.AddListener(() => show_menu_set(1));
        set_title.transform.Find("input").GetComponent<Button>().onClick.AddListener(() => show_menu_set(2));
        set_title.transform.Find("record").GetComponent<Button>().onClick.AddListener(() => show_menu_set(3));
        game_ui = transform.Find("play").gameObject;
        game_UL1 = game_ui.transform.Find("UL1").gameObject;
        game_UL2 = game_ui.transform.Find("UL2").gameObject;
        game_UL1.GetComponent<Button>().onClick.AddListener(() => game_stop());
        game_UL2.transform.Find("icon1").GetComponent<Button>().onClick.AddListener(() => game_stop());
        game_UL2.transform.Find("icon2").GetComponent<Button>().onClick.AddListener(() =>
        {
            GameBase.status.add_pop_info("Test 1\nTest 2");
        });
        game_UL2.transform.Find("icon3").GetComponent<Button>().onClick.AddListener(() =>
        {
            esc_enabled = false;
            menu_more.SetActive(true);
            show_menu_set(0);
        });
        game_UL2.transform.Find("icon4").GetComponent<Button>().onClick.AddListener(() =>
        {
            game_stop();
            GameBase.main.game_over(false);
        });
        over_ui = game_ui.transform.Find("game over").gameObject;
        game_ui.transform.Find("game over/Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            GameBase.main.game_over(true);
            over_ui.SetActive(false);
        });
        GameObject vir_key = game_ui.transform.Find("Keys").gameObject;
        GameObject sta_info = game_ui.transform.Find("Text").gameObject;
        set_gameplay.transform.Find("Virtual key/Toggle").GetComponent<Toggle>().onValueChanged.AddListener(v => vir_key.SetActive(v));
        set_gameplay.transform.Find("Stat info/Toggle").GetComponent<Toggle>().onValueChanged.AddListener(v => sta_info.SetActive(v));
        UI_Slider slider = set_gameplay.transform.Find("Key size").GetComponent<UI_Slider>();
        slider.get_slider_call().AddListener(v =>
        {
            float width_size = GetComponent<RectTransform>().offsetMax.x * v * 0.01f;
            game_ui.transform.Find("Keys/Left").GetComponent<RectTransform>().offsetMax = new Vector2(width_size, width_size);
            RectTransform rt = game_ui.transform.Find("Keys/Right").GetComponent<RectTransform>();
            rt.offsetMin = new Vector2(-width_size, 0);
            rt.offsetMax = new Vector2(0, width_size);
        });
        slider = set_gameplay.transform.Find("Key opacity").GetComponent<UI_Slider>();
        slider.get_slider_call().AddListener(v =>
        {
            Color col = new Color(1, 1, 1, v / 100f);
            foreach (var i in game_ui.transform.Find("Keys").GetComponentsInChildren<UI_GetKey>())
                i.GetComponent<Image>().color = col;
        });
        slider.slider.value = 80;
        set_gameplay.transform.Find("Menu bg/Toggle").GetComponent<Toggle>().onValueChanged.AddListener(v => background.set_anim(v));
        set_gameplay.transform.Find("Use dyna/Toggle").GetComponent<Toggle>().onValueChanged.AddListener(v => GameBase.sprite.set_dyna_active());
        set_gameplay.transform.Find("Use sky/Toggle").GetComponent<Toggle>().onValueChanged.AddListener(v => GameBase.status.set_sky_active(v));
        set_gameplay.transform.Find("Use glitch/Toggle").GetComponent<Toggle>().onValueChanged.AddListener(v =>
        {
            background.get_stoped_glitch().gameObject.SetActive(v);
            CleanGlitch.use_glitch_effect = v;
        });
        slider = set_input.transform.Find("Input delay").GetComponent<UI_Slider>();
        slider.get_slider_call().AddListener(v =>
        {
            MonitorKey.start_cd = v;
        });
        key_get1 = game_ui.transform.Find("Keys/Left/top").GetComponent<UI_GetKey>();
        key_get2 = game_ui.transform.Find("Keys/Right/key1").GetComponent<UI_GetKey>();
        key_get3 = game_ui.transform.Find("Keys/Right/key2").GetComponent<UI_GetKey>();
        key_icon1 = set_input.transform.Find("Up/Image").GetComponent<Image>();
        key_icon2 = set_input.transform.Find("funA/Image").GetComponent<Image>();
        key_icon3 = set_input.transform.Find("funB/Image").GetComponent<Image>();
        key_tex1 = key_icon1.sprite;
        key_tex2 = key_icon2.sprite;
        key_tex3 = key_icon3.sprite;
        slider = set_input.transform.Find("Key mode").GetComponent<UI_Slider>();
        slider.get_slider_call().AddListener(v =>
        {
            if (v == 1)
            {
                key_icon1.sprite = key_tex1;
                key_icon2.sprite = key_tex2;
                key_icon3.sprite = key_tex3;
                key_get1.key = GameBase.input_key.up;
                key_get2.key = GameBase.input_key.funA;
                key_get3.key = GameBase.input_key.funB;
            }
            else if (v == 2)
            {
                key_icon1.sprite = key_tex3;
                key_icon2.sprite = key_tex2;
                key_icon3.sprite = key_tex1;
                key_get1.key = GameBase.input_key.funB;
                key_get2.key = GameBase.input_key.funA;
                key_get3.key = GameBase.input_key.up;
            }
            else if (v == 3)
            {
                key_icon1.sprite = key_tex2;
                key_icon2.sprite = key_tex3;
                key_icon3.sprite = key_tex1;
                key_get1.key = GameBase.input_key.funB;
                key_get2.key = GameBase.input_key.up;
                key_get3.key = GameBase.input_key.funA;
            }
        });

        menu_game = transform.Find("game").gameObject;
        menu_game.transform.Find("back").GetComponent<Button>().onClick.AddListener(() => show_menu_main(0));
        menu_custom = transform.Find("custom").gameObject;
        menu_custom.transform.Find("back").GetComponent<Button>().onClick.AddListener(() => show_menu_main(2));
        transform.Find("pop").GetComponent<UI_PopUp>().start();
        update_screen.AddListener(replace_size);
        update_screen.AddListener(GameBase.status.match_sky_size);
        now_main = menu_main;
        StartCoroutine(start_init());
    }
    IEnumerator start_init()
    {
        show_menu_main(1);
        yield return null;
        show_menu_set(1);
        yield return null;
        show_menu_set(2);
        yield return null;
        show_menu_set(3);
        yield return null;
        show_menu_main(0);
        background.transform.SetAsFirstSibling();
    }
    public void replace_size()
    {
        RectTransform rt = GetComponent<RectTransform>();
        float f = Screen.height / height_base;
        rt.offsetMax = new Vector2(height_base / Screen.height * Screen.width, height_base);
        rt.localScale = new Vector3(f, f, 1);
        last_width = Screen.width;
        last_height = Screen.height;
    }
    public void show_menu_main(int i)
    {
        now_main.SetActive(i == 0);
        menu_more.SetActive(i == 1);
        menu_game.SetActive(i == 2);
        menu_custom.SetActive(i == 3);
        if (i == 1) show_menu_set(0);
        if (now_main == game_ui) esc_enabled = stop_game;
    }
    public void show_menu_set(int i)
    {
        set_basic.SetActive(i == 0);
        set_gameplay.SetActive(i == 1);
        set_input.SetActive(i == 2);
        info_record.SetActive(i == 3);
    }
    public void choose_language(int i)
    {
        text_stsyem.choose_language(i);
        foreach (var t in GameBase.main_ui.GetComponentsInChildren<UI_Text>(true)) t.reset_text();
    }
    public void game_start(int w, int h, bool custom_func = false)
    {
        GameBase.W = w;
        GameBase.H = h;
        if (!custom_func)
        {
            GameBase.main.fall_speed = 1f;
            GameBase.status.use_day = true;
            BlockData.create_func = BlockData.get_random_block;
            GameBase.main.use_test_func = false;
        }
        GameBase.main.game_start();
        background.gameObject.SetActive(false);
        now_main = game_ui;
        show_menu_main(0);
    }
    public void game_over()
    {
        show_menu_main(2);
        background.gameObject.SetActive(true);
        now_main = menu_main;
    }
    public void game_stop()
    {
        if (GameBase.game_mode != 4) GameBase.main.save_file(false);
        if (stop_game) GameBase.main.need_save = true;
        GameBase.main.gameObject.SetActive(stop_game);
        GameBase.water.enabled = stop_game;
        GameBase.lava.enabled = stop_game;
        GameBase.snow.enabled = stop_game;
        GameBase.status.enabled = stop_game;
        game_UL1.SetActive(stop_game);
        game_UL2.SetActive(!stop_game);
        esc_enabled = stop_game = !stop_game;
    }
    public void game_over_menu()
    {
        GameBase.run_game = false;
        GameBase.water.enabled = false;
        GameBase.lava.enabled = false;
        GameBase.snow.enabled = false;
        GameBase.status.enabled = false;
        GameBase.weather.enabled = false;
        over_ui.SetActive(true);
    }
    private void Update()
    {
        if (last_width != Screen.width || last_height != Screen.height) update_screen.Invoke();
        if (!esc_enabled) return;
        if (esc_up)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) esc_down_time = Time.time;
            if (esc_down_time == 0) return;
            if (Input.GetKeyUp(KeyCode.Escape) || Time.time - esc_down_time > 2f)
            {
                game_stop();
                if (Time.time - esc_down_time > 2f) GameBase.main.game_over(false);
                esc_down_time = 0;
                esc_up = false;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Escape)) esc_up = true;
    }
}
