using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : GameBase
{
    public float bg_edge_width = 0.2f;

    private Queue<GroupBlock> next_list;
    public int next_list_upper = 5;
    private float next_space;
    private Transform dot;

    private Text score_text;
    private int score_value;

    private Text time_text, day_text;
    private int time_h, time_m, time_day;
    private float time_tot;

    public int w_next, w_power;

    public bool use_day;
    public Shader grid_shader, sky_shader;
    public List<Color> color_map = new List<Color>();
    private SpriteRenderer sky, grid;
    private Material grid_mat, bg_mat;
    private int color_length;
    private bool sky_active;

    private Transform info_root;
    private UI_PopInfo pop_template;

    private void Start()
    {
        status = this;
        next_list = new Queue<GroupBlock>();
        dot = new GameObject("next_list").transform;
        dot.parent = transform;
        gameObject.SetActive(false);
        string status_path = "Status";
        score_text = transform.Find(status_path + "/score_value").GetComponent<Text>();
        time_text = transform.Find(status_path + "/time_value").GetComponent<Text>();
        day_text = transform.Find(status_path + "/day_value").GetComponent<Text>();
        info_root = transform.Find(status_path + "/info");
        pop_template = info_root.Find("Template").GetComponent<UI_PopInfo>();
        GameObject go = new GameObject("Background");
        go.transform.parent = transform;
        sky = new GameObject("sky").AddComponent<SpriteRenderer>();
        grid = new GameObject("grid").AddComponent<SpriteRenderer>();
        sky.transform.parent = grid.transform.parent = go.transform;
        sky.sprite = grid.sprite = sprite.white;
        sky.sortingOrder = -10;
        grid.sortingOrder = -9;
        sky.material = bg_mat = new Material(sky_shader);
        grid.material = grid_mat = new Material(grid_shader);
        color_length = color_map.Count / 3;
        for (int i = 0; i < 3; i++) color_map.Add(color_map[i]);
        set_sky_active(true);
    }
    public void game_start(bool init_next)
    {
        gameObject.SetActive(true);
        score_text.text = "0";
        score_value = 0;
        time_h = 8;
        time_m = 0;
        time_day = 1;
        w_power = 0;
        w_next = Random.Range(6, 12);
        time_text.text = $"{time_h:D02}:{time_m:D02}";
        day_text.text = $"{time_day}day";
        float b_size = (H / 2f) / 10f;
        next_space = 15f / (next_list_upper + 1) * b_size;
        transform.position = new Vector2(W + 1, -1);
        transform.localScale = new Vector3(b_size, b_size, 1);
        dot.localPosition = new Vector2(0.5f, -20);
        for (int i = init_next ? next_list_upper : 0; 0 < i; i--)
        {
            GroupBlock n = new GameObject("next").AddComponent<GroupBlock>().start();
            n.transform.parent = dot;
            n.transform.localScale = Vector3.one;
            n.transform.localPosition = Vector2.zero;
            n.gameObject.AddComponent<d_shift>().start(new Vector2(0, next_space * i)).set(0.5f);
            next_list.Enqueue(n);
        }
        sky.transform.parent.position = new Vector2((W + 1) / 2f, -(H + 1) / 2f);
        sky.transform.parent.localScale = new Vector3(1 / b_size, 1 / b_size, 1);
        match_sky_size();
        grid.transform.localScale = new Vector2(W + bg_edge_width, H + bg_edge_width);
        grid_mat.SetInt("width", W);
        grid_mat.SetInt("height", H);
        grid_mat.SetFloat("side_width", bg_edge_width);
        update_sky_shader();
    }
    public void match_sky_size()
    {
        if (run_game) sky.transform.localScale = new Vector2((H + 2) * Camera.main.aspect, H + 3);
    }
    public void game_over()
    {
        while (next_list.Count > 0)
            Destroy(next_list.Dequeue().gameObject);
        var list = UI_PopInfo.list;
        foreach (var i in list) Destroy(i.gameObject);
        list.Clear();
        gameObject.SetActive(false);
    }
    public GroupBlock get_next()
    {
        GroupBlock pop = next_list.Dequeue();
        if (next_list.Count < next_list_upper)
        {
            GroupBlock push = new GameObject("next").AddComponent<GroupBlock>().start();
            foreach (var i in pop.GetComponents<d_shift>()) Destroy(i);
            push.transform.parent = dot;
            push.transform.localScale = Vector3.one;
            push.transform.localPosition = Vector2.zero;
            next_list.Enqueue(push);
            foreach (GroupBlock i in next_list)
                i.gameObject.AddComponent<d_shift>().start(new Vector2(0, next_space)).set(0.5f);
            foreach (Block i in push.sub)
                i.gameObject.AddComponent<FadeIn>().start(i.sr);
        }
        return pop;
    }
    public void update_light()
    {
        if (use_day)
        {
            if (time_h <= 6)
            {
                lights.update_global_light(Mathf.Cos(time_h / 12f * Mathf.PI) * 0.8f);
            }
            else if (time_h > 18)
            {
                lights.update_global_light(Mathf.Sin((time_h - 18) / 12f * Mathf.PI) * 0.8f);
            }
        }
    }
    public void add_score(int i)
    {
        score_value += i;
        score_text.text = string.Format("{0}", score_value);
    }
    public void add_time(int d)
    {
        time_m += d;
        if (time_m >= 60)
        {
            add_score(10);
            time_h++;
            time_m %= 60;
            if (time_h >= 24)
            {
                time_h %= 24;
                time_day++;
                day_text.text = $"{time_day}day";
            }
            update_light();
            if (w_power > 0)
            {
                if (++w_next == 3)
                {
                    w_next = 0;
                    w_power++;
                    if (Random.Range(0, 1f) < (1f / w_power))
                    {
                        weather.size_update(w_power);
                    }
                    else if (Random.Range(0, 1f) < 0.75f)
                    {
                        add_pop_info(weather.type == 1 ? "Rain\nEnd" : "Snow\nEnd");
                        weather.sleep();
                        w_power = 0;
                        w_next = Random.Range(12, 24);
                    }
                    else w_power--;
                }
            }
            else if (--w_next == 0)
            {
                w_power = 1;
                int mode = Random.Range(0, 2);
                add_pop_info(mode == 0 ? "Rain\nStart" : "Snow\nStart");
                weather.play(mode);
            }
        }
        time_text.text = $"{time_h:D02}:{time_m:D02}";
    }
    public string package_save()
    {
        string s = $"{time_h};{time_m};{time_day};{w_next};{w_power};{score_value};";
        s += main.group_float.package_save();
        foreach (var i in next_list) s += ";" + i.package_save();
        return s;
    }
    public void package_load(string data)
    {
        game_start(false);
        string[] s = data.Split(';');
        time_h = int.Parse(s[0]);
        time_m = int.Parse(s[1]);
        time_day = int.Parse(s[2]);
        w_next = int.Parse(s[3]);
        w_power = int.Parse(s[4]);
        add_score(int.Parse(s[5]));
        time_text.text = $"{time_h:D02}:{time_m:D02}";
        day_text.text = $"{time_day}day";
        update_light();
        for (int i = 6; i < s.Length; i++)
        {
            GroupBlock n = new GameObject("next").AddComponent<GroupBlock>();
            n.package_load(s[i]);
            n.transform.parent = dot;
            n.transform.localScale = Vector3.one;
            n.transform.localPosition = Vector2.zero;
            n.gameObject.AddComponent<d_shift>().start(new Vector2(0, next_space * (s.Length - i))).set(0.5f);
            next_list.Enqueue(n);
        }
        update_sky_shader();
    }
    public void set_sky_active(bool active)
    {
        if (sky_active != active)
        {
            sky_active = active;
            if (active) update_sky_shader();
            else
            {
                Color col = Tool.str2rgb("#333333");
                bg_mat.SetColor("top_color", col);
                bg_mat.SetColor("mid_color", col);
                bg_mat.SetColor("btm_color", col);
            }
        }
    }
    public void update_sky_shader()
    {
        if (!sky_active) return;
        float a = (time_h * 60 + time_m) / 1440f * color_length, f = a % 1;
        int i = (int)a * 3, j = a < color_length ? i + 3 : i;
        bg_mat.SetColor("top_color", Color.Lerp(color_map[i], color_map[j], f));
        bg_mat.SetColor("mid_color", Color.Lerp(color_map[i + 1], color_map[j + 1], f));
        bg_mat.SetColor("btm_color", Color.Lerp(color_map[i + 2], color_map[j + 2], f));
    }
    public void top_record()
    {
        RecordFile.main.update_top(game_mode, score_value, (time_day * 24 + time_h) * 60 + time_m);
    }
    public void add_pop_info(string info)
    {
        if (UI_PopInfo.list.Count >= 5) UI_PopInfo.delete();
        var i = Instantiate(pop_template, info_root);
        i.info = info;
        i.gameObject.SetActive(true);
    }
    private void Update()
    {
        time_tot += Time.deltaTime;
        if (time_tot >= 1f)
        {
            time_tot -= 1f;
            add_time(1);
            if (use_day) update_sky_shader();
        }
    }
}