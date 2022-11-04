using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_GameCustom : MonoBehaviour
{
    public Transform use_group, ban_group;
    private bool[] ban_state;
    private GridLayoutGroup ban_layout1, ban_layout2;
    private UI_TSI i_width, i_height;
    private UI_Slider i_fall;
    private Toggle i_use_day, i_use_test;
    private BlockCreate block_create;
    void Start()
    {
        ban_layout1 = use_group.GetComponent<GridLayoutGroup>();
        ban_layout2 = ban_group.GetComponent<GridLayoutGroup>();
        i_width = transform.Find("width").GetComponent<UI_TSI>();
        i_height = transform.Find("height").GetComponent<UI_TSI>();
        i_fall = transform.Find("Fall speed").GetComponent<UI_Slider>();
        i_use_day = transform.Find("use day/Toggle").GetComponent<Toggle>();
        i_use_test = transform.Find("use test/Toggle").GetComponent<Toggle>();
        block_create = new BlockCreate();
        i_fall.get_slider_call().AddListener(v => { if (v == 0) i_fall.text.text = "-"; });
        transform.Find("start").GetComponent<Button>().onClick.AddListener(() =>
        {
            block_create.init(ban_state, 0.4f, 0.5f, 0.5f);
            if (block_create.range3 == 0)
            {
                UI_PopUp.menu.set_display(UI_PopUp.display.NoUseBlock);
                return;
            }
            BlockData.create_func = block_create.get_random_block;
            GameBase.main.fall_speed = i_fall.slider.value;
            GameBase.status.use_day = i_use_day.isOn;
            GameBase.main.use_test_func = i_use_test.isOn;
            int w = int.Parse(i_width.input.text), h = int.Parse(i_height.input.text);
            if (w > h)
            {
                UI_PopUp.menu.set_display(UI_PopUp.display.HighAspect);
                UI_PopUp.run_event = () => GameBase.main_ui.game_start(h, h, true);
            }
            else GameBase.main_ui.game_start(w, h, true);
        });
        var tmp_use_block = BlockData.use_block;
        var temp = new GameObject().AddComponent<Image>();
        temp.gameObject.AddComponent<Button>();
        ban_state = new bool[tmp_use_block.Length];
        for (int i = 0; i < tmp_use_block.Length; i++)
        {
            var sr = Instantiate(temp);
            int id = i;
            sr.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                ban_state[id] = !ban_state[id];
                sr.rectTransform.SetParent(ban_state[id] ? ban_group : use_group);
            });
            sr.name = tmp_use_block[i].ToString();
            sr.sprite = BlockData.name2sprite(tmp_use_block[i]);
            sr.rectTransform.SetParent(use_group);
            sr.transform.localScale = Vector3.one;
        }
        Destroy(temp);
        reset_gird_layout();
        GameBase.main_ui.update_screen.AddListener(reset_gird_layout);
    }

    void reset_gird_layout()
    {
        RectTransform rt1 = ban_layout1.GetComponent<RectTransform>(), rt2 = ban_layout2.GetComponent<RectTransform>();
        float i = rt1.rect.size.x / 8f;
        ban_layout1.cellSize = ban_layout2.cellSize = Vector2.one * i;
        rt1.offsetMax = rt2.offsetMax = new Vector2(0, Mathf.Ceil(BlockData.use_block.Length / 8f) * i);
        rt1.offsetMin = rt2.offsetMin = Vector2.zero;
        rt1.localPosition = rt2.localPosition = Vector3.zero;
    }
}