using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameBase : MonoBehaviour
{
    public static readonly (int, int)[] OUTSIDE_4 = { (0, -1), (-1, 0), (0, 1), (1, 0) };

    public enum input_key { left, right, down, up, funA, funB }
    public enum block_name
    {
        dirt, grass_dirt, send, stone, cobble, wood, leaf, plank,
        coal_ore, iron_ore, gold_ore, diamond_ore, red_ore, green_ore, blue_ore, bad_rock,
        iron_block, gold_block, diamond_block, brick, glow_stone, glass, ice, snow,
        worktable0, worktable1, bookshelf, hay_block, wool, pumpkin_head, furnace0, furnace1,
        water, lava, redstone_block, tnt, lamp0, lamp1, observer0, observer1,
        piston0, piston1, piston2, piston3, piston4, structure0, structure1, structure2,
        cmd0, cmd1, cmd2, cmd3, white,
    }

    public static int W, H;
    public static Block[,] block;
    public static MainSprite sprite;
    public static MainAudio sounds;
    public static GameLight lights;
    public static Weather weather;
    public static Water water, lava, snow;
    public static GameUI status;
    public static MainGame main;
    public static MainUI main_ui;
    public static MainCamera main_camera;
    public static bool run_game = false;
    public static int game_mode;
}
public class MainGame : GameBase
{
    [HideInInspector]
    public GroupBlock group_float;
    private GroupBlock group_ghost;
    private Material ghost_mat;
    private float tick_time_diff = 0.05f;
    private float tick_time_total = 0;
    public float fall_speed = 1f;
    private float fall_speed_total = 0;
    public string save_data = "";
    public bool need_save, use_test_func;
    private float last_save_time;
    private float diff_save_time = 10f;
    private int combo_count = 0;
    private bool[] line_tmp;
    void Awake()
    {
        main = this;
        Screen.fullScreen = false;
        Application.targetFrameRate = -1;
        game_mode = PlayerPrefs.GetInt("Mode");
        new RecordFile().start();
        new MainConfig().start();
        Block.parent = new GameObject("group fixed").transform;
        water = new GameObject("water").AddComponent<Water>();
        lava = new GameObject("lava").AddComponent<Water>();
        snow = new GameObject("snow").AddComponent<Snow>();
        water.enabled = false;
        lava.enabled = false;
        snow.enabled = false;
        water.main_water = true;
        Color water_color = Tool.str2rgb("#1B64E5");
        water_color.a = 0.6f;
        water.color = water_color;
        snow.color = lava.color = Color.white;
        water.total_upper = 1;
        lava.total_upper = 2;
        snow.total_upper = 4;
    }
    private void Start()
    {
        BlockData.init();
        ghost_mat = new Material(sprite.ghost_shader);
        Material glitch_mat = new Material(sprite.glitch_block_shader);
        glitch_mat.SetFloat("_Speed", 50);
        glitch_mat.SetFloat("_BlockSize", 50);
        glitch_mat.SetFloat("_MaxRGBSplitX", 8);
        glitch_mat.SetFloat("_MaxRGBSplitY", 8);
        CleanGlitch.glitch_mat = glitch_mat;
        CleanGlitch.init();
        AddWhite.init();
    }
    private void OnApplicationQuit()
    {
        if (run_game) game_over(false);
    }
    public void game_start()
    {
        if (run_game) return;
        W = Mathf.Min(W, H);
        BlockBase.init();
        tick_time_total = fall_speed_total = last_save_time = Anim.time;
        block = new Block[W + 2, H + 2];
        line_tmp = new bool[H + 2];
        Block bad_rock = new GameObject("side block").AddComponent<BedRock>();
        bad_rock.canPush = false;
        need_save = run_game = true;
        for (int i = W + 1; 0 <= i; --i)
            block[i, 0] = block[i, H + 1] = bad_rock;
        for (int i = H + 1; 0 <= i; --i)
            block[0, i] = block[W + 1, i] = bad_rock;
        water.start(sprite.water);
        lava.start(sprite.lava);
        snow.start(sprite.snow);
        lights.start();
        Camera cam = Camera.main;
        cam.orthographicSize = H / 2 + 1;
        cam.transform.position = new Vector3((W + 1) / 2f, -(H + 1) / 2f, -10);
        weather.start();
        CleanGlitch.set_width(W);
        if (save_data.Length > 1)
        {
            package_load(save_data);
            save_data = "";
        }
        else
        {
            status.game_start(true);
        }
        add();
    }
    public void save_file(bool must_save, bool delete_save = false)
    {
        string path = Application.persistentDataPath + "\\save.txt";
        if (delete_save)
        {
            if (File.Exists(path)) File.Delete(path);
        }
        else if (need_save && (must_save || last_save_time + diff_save_time < Anim.time))
        {
            if (!must_save) status.add_pop_info("Auto\nSave");
            RecordFile.main.update_time();
            last_save_time = Anim.time;
            need_save = false;
            using var file = File.CreateText(path);
            file.Write(package_save());
        }
    }
    public void game_over(bool true_end)
    {
        save_file(true, game_mode == 4 || true_end);
        if (true_end) status.top_record();
        foreach (Block i in block)
            if (i != null) i.delete();
        DataStructure.ban = false;
        Destroy(group_float.gameObject);
        Destroy(group_ghost.gameObject);
        water.game_over();
        lava.game_over();
        snow.game_over();
        lights.game_over();
        status.game_over();
        main_ui.game_over();
        weather.game_over();
        run_game = use_test_func = false;
        GC.Collect();
    }
    public string game_world_save()
    {
        string tmp, s = "Block\n" + BlockData.package_save();
        tmp = water.package_save();
        if (tmp.Length > 2) s += "\nWater\n" + tmp;
        tmp = lava.package_save();
        if (tmp.Length > 2) s += "\nLava\n" + tmp;
        tmp = snow.package_save();
        if (tmp.Length > 2) s += "\nSnow\n" + tmp;
        return s;
    }
    public string package_save()
    {
        string tmp, s = game_world_save();
        tmp = status.package_save();
        if (tmp.Length > 2) s += "\nStatus\n" + tmp;
        tmp = weather.package_save();
        if (tmp.Length > 2) s += "\nWeather\n" + tmp;
        if (DataStructure.data != null)
        {
            s += "\nDataStructureStart\n" + DataStructure.data + "\nDataStructureEnd";
        }
        return s;
    }
    public void package_load(string data)
    {
        string[] s = data.Split('\n');
        for (int i = 0; i < s.Length; i += 2)
        {
            if (s[i].Equals("Block")) BlockData.package_load(s[i + 1]);
            else if (s[i].Equals("Water")) water.package_load(s[i + 1]);
            else if (s[i].Equals("Lava")) lava.package_load(s[i + 1]);
            else if (s[i].Equals("Snow")) snow.package_load(s[i + 1]);
            else if (s[i].Equals("Status")) status.package_load(s[i + 1]);
            else if (s[i].Equals("Weather")) weather.package_load(s[i + 1]);
            else if (s[i].Equals("DataStructureStart"))
            {
                var sb = new System.Text.StringBuilder(500);
                i++;
                while (!s[i].Equals("DataStructureEnd")) sb.Append(s[i++] + "\n");
                DataStructure.data = sb.ToString();
            }
        }
    }
    void add()
    {
        GroupBlock next = status.get_next();
        next.SendMessage("stop_animation", options: SendMessageOptions.DontRequireReceiver);
        next.name = "group_float";
        next.x = W / 2;
        next.y = 1;
        next.transform.parent = null;
        next.transform.localScale = Vector3.one;
        next.transform.position = new Vector2(next.x, -1);
        group_float = next;
        group_ghost = Instantiate(next);
        group_ghost.use_anim = false;
        if (next.sub[0].id == 14 || next.sub[0].id == 15)
        {
            foreach (Block b in group_ghost.sub)
                b.sr.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            foreach (Block b in group_ghost.sub)
                b.sr.material = ghost_mat;
        }
        ghost_update();
        foreach (Block i in group_float.sub)
            i.gameObject.AddComponent<FadeIn>().start(i.sr);
    }
    void fall()
    {
        foreach ((int _1, int x, int y) in group_float.get_position())
            if (block[x, y + 1]) block[x, y + 1].heavy_load();
        foreach ((int i, int x, int y) in group_float.get_position())
        {
            Block b = group_float.sub[i];
            if (!block[x, y])
            {
                b.transform.parent = Block.parent;
                b.start(x, y);
                b.sr.sortingOrder = 0;
                AddWhite.add_white(x, y);
            }
            else Destroy(b.gameObject);
        }
        BlockData.lay_clip(group_float.sub[0].id);
        Destroy(group_float.gameObject);
        Destroy(group_ghost.gameObject);
        add();
        foreach ((int i, int x, int y) in group_float.get_position())
            if (block[x, y])
            {
                main_ui.game_over_menu();
                DataStructure.turn_ban();
                return;
            }
        combo_count = 0;
        status.add_score(1);
        fall_check();
        BlockBase.match_update_line();
        main_camera.shift_down();
        status.add_time(10);
    }
    public bool fall_check()
    {
        bool has_clean = false;
        int add_score = 0;
        for (int y = H; y > 0; --y)
        {
            if (BlockBase.block_line[y] == W)
            {
                for (int i = 1; i <= W; ++i) if (block[i, y]) block[i, y].clear_up();
                add_score += 100 + (combo_count * 50);
                combo_count++;
                sounds.opt_clip(MainAudio.opt.clean2);
                line_tmp[y] = true;
                has_clean = true;
                CleanGlitch.add_clean_glitch(y);
            }
        }
        if (add_score > 0)
        {
            status.add_score(add_score);
            status.add_pop_info($"{combo_count} Combo\n+{add_score}");
        }
        if (has_clean)
        {
            for (int x = 1; x <= W; ++x)
            {
                int d = 0;
                for (int y = H; y > 0; --y)
                {
                    if (line_tmp[y]) d++;
                    else if (!block[x, y]) continue;
                    else if (!block[x, y].canPush) d = 0;
                    else block[x, y].move_block(0, d);
                }
            }
            for (int y = 1; y <= H; ++y) line_tmp[y] = false;
        }
        if (DataStructure.world)
        {
            for (int x = 1; x <= W; ++x)
                for (int y = 1; y <= H; ++y)
                    if (block[x, y]) block[x, y].delete();
            foreach (WaterNode i in water.list_copy()) i.delete();
            foreach (WaterNode i in lava.list_copy()) i.delete();
            package_load(DataStructure.data);
            DataStructure.turn_ban();
            main_camera.glitch();
            return false;
        }
        return true;
    }
    public void water_check()
    {
        if (snow.node_list.Count > 0)
        {
            foreach ((int x, int y) in snow.value_max)
            {
                if (!block[x, y]) BlockData.name2block(block_name.snow).start(x, y);
            }
            foreach (WaterNode n in snow.list_copy())
            {
                if (lava.w[n.x, n.y, n.n] || lava.w[n.x, n.ty, n.tn])
                {
                    n.delete();
                }
                else if (water.w[n.x, n.y, n.n] || water.w[n.x, n.ty, n.tn])
                {
                    n.delete();
                }
            }
        }
        HashSet<(int, int)> del = new HashSet<(int, int)>();
        HashSet<WaterNode> del_w = new HashSet<WaterNode>();
        foreach (WaterNode n in water.node_list)
        {
            if (lava.w[n.x, n.y, n.n]) del.Add((n.x, n.y));
            if (lava.w[n.x, n.ty, n.tn]) del.Add((n.x, n.ty));
        }
        foreach ((int x, int y) in del)
        {
            for (int i = 0; i < WaterBase.unit; ++i)
            {
                if (water.w[x, y, i])
                {
                    water.w[x, y, i].len--;
                    del_w.Add(water.w[x, y, i]);
                }
                if (lava.w[x, y, i])
                {
                    lava.w[x, y, i].len--;
                    del_w.Add(lava.w[x, y, i]);
                }
            }
            sounds.run_clip(MainAudio.run.fizz);
            BlockData.name2block(block_name.cobble).start(x, y);
        }
        foreach (WaterNode n in del_w)
        {
            if (n.len > 0) n.overflow();
            else n.delete();
        }
        List<WaterTmpBlock> list = new List<WaterTmpBlock>();
        foreach (WaterTmpBlock i in water.create_block()) list.Add(i);
        foreach (WaterTmpBlock i in lava.create_block()) list.Add(i);
        if (list.Count > 0)
        {
            if (!fall_check()) return;
            foreach (WaterTmpBlock b in list) if (!b.del) b.delete();
            foreach (WaterTmpBlock b in list)
            {
                if (b.del)
                {
                    WaterNode n = b.node;
                    n.len -= WaterBase.unit;
                    n.delete();
                    if (n.len > 0) n.root.add_more(n.x, n.y, n.len);
                }
            }
        }
    }
    public void ghost_update()
    {
        group_ghost.x = group_float.x;
        group_ghost.y = group_float.y;
        if (group_ghost.d != group_float.d)
            group_ghost.spin_check(0, 0, group_float.d - group_ghost.d);
        group_ghost.transform.position = group_float.transform.position;
        while (group_ghost.move_check(0, 1)) ;
    }
    public void input_func(Dictionary<input_key, bool> key)
    {
        if (!run_game) return;
        bool is_move = false;
        if (key[input_key.funA])
        {
            if (!group_float.spin_check(0, 0, 3)) group_float.SRS(true);
            is_move = group_ghost.d != group_float.d;
        }
        else if (key[input_key.funB])
        {
            if (!group_float.spin_check(0, 0, 1)) group_float.SRS(false);
            is_move = group_ghost.d != group_float.d;
        }
        if (key[input_key.up])
        {
            while (group_float.move_check(0, 1)) ;
            fall();
        }
        if (key[input_key.down])
        {
            if (group_float.move_check(0, 1)) is_move = true;
            else fall();
            fall_speed_total = Anim.time;
        }
        if (key[input_key.left] && group_float.move_check(-1, 0)) is_move = true;
        if (key[input_key.right] && group_float.move_check(1, 0)) is_move = true;
        if (is_move)
        {
            sounds.opt_clip(MainAudio.opt.move);
            ghost_update();
        }
    }
    public void test_func()
    {
        if (Input.GetKey(KeyCode.O))
        {
            if (Input.GetKey(KeyCode.U)) group_float.move_check(0, -1);
            if (Input.GetKeyDown(KeyCode.I))
            {
                for (int x = 1; x <= W; ++x)
                    for (int y = 1; y <= H; ++y)
                        if (block[x, y] && block[x, y].canFire)
                        {
                            block[x, y].catch_fire();
                            x = W + 1;
                            break;
                        }
            }
            if (Input.GetKey(KeyCode.J))
            {
                status.add_time(1);
                status.update_sky_shader();
            }
            if (Input.GetKey(KeyCode.K))
            {
                for (int x = 1; x <= W; ++x)
                    for (int y = 1; y <= H; ++y)
                        if (block[x, y]) block[x, y].play(4);
            }
            if (Input.GetKey(KeyCode.P))
            {
                int i = water.w[1, 2, 0] ? 2 : 1;
                for (; i < W; i += 2) water.add(i, 1, 0);
            }
            if (Input.GetKey(KeyCode.L)) lava.add(W, 1, 0);
            if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.S))
            {
                for (int y = H / 3; y <= H; ++y)
                    for (int x = y & 1; x <= W; x += 2)
                        if (!block[x, y]) BlockData.create_func().start(x, y);
            }
        }
        if (Input.GetKey(KeyCode.Alpha0))
            for (int x = 0; x < 8; ++x)
                if (Input.GetKeyDown((KeyCode)(49 + x)))
                    group_float.SRS(x >= 4, x % 4);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 m_pos = Input.mousePosition;
            Vector3 w_pos = Camera.main.ScreenToWorldPoint(new Vector3(m_pos.x, m_pos.y, Camera.main.transform.position.z));
            (int x, int y) = ((int)(w_pos.x + 0.5f), (int)(0.5f - w_pos.y));
            if (0 < x && x <= W && 0 < y && y <= H)
            {
                if (block[x, y]) block[x, y].delete();
                else BlockData.name2block(block_name.white).start(x, y);
            }
        }
    }
    void Update()
    {
        if (!run_game) return;
        Anim.time += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            main_ui.game_stop();
            return;
        }
        if (tick_time_total + tick_time_diff < Anim.time)
        {
            sounds.update_bgm();
            lights.update_light();
            tick_time_total = Anim.time;
            BlockBase.update();
            fall_check();
        }
        if (use_test_func) test_func();
        if (fall_speed > 0 && fall_speed_total + fall_speed < Anim.time)
        {
            if (group_float.move_check(0, 1))
            {
                sounds.opt_clip(MainAudio.opt.move);
                ghost_update();
            }
            else fall();
            fall_speed_total = Anim.time;
        }
    }
}
public class RecordFile
{
    static public RecordFile main;
    private string file_path;
    public int game_time, game_count;
    public int[] top_data;
    private int total_time;
    public string[] history_data;
    public int update_count;
    public void start()
    {
        main = this;
        file_path = Application.persistentDataPath + "\\record.txt";
        top_data = new int[12];
        history_data = new string[12];
        if (File.Exists(file_path))
        {
            string[] file_data = File.ReadAllLines(file_path);
            string[] s = file_data[0].Split(';');
            for (int i = 0; i < 12; i++)
                top_data[i] = int.Parse(s[i]);
            game_time = total_time = int.Parse(s[12]);
            game_count = int.Parse(s[13]);
            s = file_data[1].Split(';');
            for (int i = 0; i < 12; i++)
                history_data[i] = s[i];
        }
        else
        {
            for (int i = 0; i < 12; i++)
                history_data[i] = "0";
        }
    }
    public void update_top(int mode, int score, int time)
    {
        if (mode < 4)
        {
            if (score > top_data[mode * 3]) top_data[mode * 3] = score;
            if (time > top_data[mode * 3 + 1]) top_data[mode * 3 + 1] = time;
            top_data[mode * 3 + 2]++;
        }
        for (int i = 11; 4 <= i; i--)
            history_data[i] = history_data[i - 4];
        history_data[0] = (mode + 1).ToString();
        history_data[1] = score.ToString();
        history_data[2] = time.ToString();
        history_data[3] = DateTime.Now.ToString();
        game_count++;
        update_time();
    }
    public void update_time()
    {
        game_time = total_time + (int)Time.time;
        save_record();
        update_count++;
    }
    public void save_record()
    {
        var sb = new System.Text.StringBuilder(500);
        foreach (int i in top_data) sb.Append(i + ";");
        sb.Append($"{game_time};{game_count}\n");
        foreach (string i in history_data) sb.Append(i + ";");
        File.WriteAllText(file_path, sb.ToString());
    }
}
public class CleanGlitch : MonoBehaviour
{
    static public Material glitch_mat;
    static public SpriteRenderer template;
    static public bool use_glitch_effect;
    SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(run());
    }
    IEnumerator run()
    {
        yield return new WaitForSeconds(0.1f);
        if (use_glitch_effect)
        {
            sr.material = glitch_mat;
            for (int i = 1 << 8; 0 < i; i >>= 1)
            {
                glitch_mat.SetFloat("_Speed", i);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
    static public void init()
    {
        GameObject go = new GameObject("Clean");
        go.AddComponent<CleanGlitch>();
        go.SetActive(false);
        template = go.AddComponent<SpriteRenderer>();
        template.sprite = BlockData.name2sprite(GameBase.block_name.white);
        template.sortingOrder = 10;
        use_glitch_effect = true;
    }
    static public void set_width(int w)
    {
        template.transform.localPosition = new Vector2(w / 2f + 0.5f, 0);
        template.transform.localScale = new Vector2(w, 1);
    }
    static public void add_clean_glitch(int h)
    {
        var go = Instantiate(template);
        go.transform.Translate(Vector2.down * h);
        go.gameObject.SetActive(true);
    }
}
public class AddWhite : MonoBehaviour
{
    static public SpriteRenderer template;
    private SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(run());
    }
    IEnumerator run()
    {
        Color color = Color.white;
        for (int i = 10; 0 <= i; i--)
        {
            color.a = Anim.into(i / 10f);
            sr.color = color;
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }
    static public void init()
    {
        GameObject go = new GameObject("White");
        go.AddComponent<AddWhite>();
        go.SetActive(false);
        template = go.AddComponent<SpriteRenderer>();
        template.sprite = BlockData.name2sprite(GameBase.block_name.white);
        template.sortingOrder = 10;
    }

    static public void add_white(int x, int y)
    {
        var go = Instantiate(template);
        go.transform.Translate(new Vector2(x, -y));
        go.gameObject.SetActive(true);
    }
}