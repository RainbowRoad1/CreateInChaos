using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class BlockData : GameBase
{
    public delegate Block Create();
    static public Create create_func;
    public static float rate_range1 = 0.3f;
    public static float rate_range2 = 0.5f;
    public static bool[] block_ban;
    public static Sprite[] sprites;
    private static bool run_init = false;
    private static Color water_color = new Color(0.1f, 0.4f, 0.8f);
    private static Color[] wool_color;
    public static block_name[] use_block =
    {
        block_name.dirt, block_name.grass_dirt, block_name.send, block_name.stone, block_name.cobble,
        block_name.wood, block_name.leaf, block_name.plank, block_name.coal_ore, block_name.iron_ore,
        block_name.gold_ore, block_name.diamond_ore, block_name.red_ore, block_name.green_ore, block_name.blue_ore,
        block_name.bad_rock, block_name.iron_block, block_name.gold_block, block_name.diamond_block, block_name.brick,
        block_name.glow_stone, block_name.glass, block_name.ice, block_name.snow, block_name.worktable0,
        block_name.worktable1, block_name.bookshelf, block_name.hay_block, block_name.wool, block_name.pumpkin_head,
        block_name.furnace0, block_name.water, block_name.lava, block_name.redstone_block, block_name.tnt,
        block_name.lamp0, block_name.observer0, block_name.piston0, block_name.piston1, block_name.structure0,
    };
    public static block_name[] func_block_gorup =
    {
        block_name.wood, block_name.water, block_name.lava, block_name.redstone_block, block_name.tnt,
        block_name.lamp0, block_name.observer0, block_name.piston0, block_name.piston1,
    };
    public static block_name[] feat_block_gorup =
    {
        block_name.dirt, block_name.grass_dirt, block_name.send, block_name.leaf, block_name.plank,
        block_name.bad_rock, block_name.glow_stone, block_name.glass, block_name.ice, block_name.worktable0,
        block_name.worktable1, block_name.bookshelf, block_name.hay_block, block_name.wool, block_name.pumpkin_head,
        block_name.furnace0,
    };
    public static block_name[] easy_block_gorup =
    {
        block_name.stone, block_name.cobble, block_name.coal_ore, block_name.iron_ore, block_name.gold_ore,
        block_name.diamond_ore, block_name.red_ore, block_name.green_ore, block_name.blue_ore, block_name.iron_block,
        block_name.gold_block, block_name.diamond_block, block_name.brick, block_name.snow,
    };
    private static block_name[] redstone_mode_group =
    {
        block_name.redstone_block, block_name.piston0, block_name.piston1,
        block_name.wood, block_name.water, block_name.lava, block_name.tnt, block_name.lamp0,
        block_name.observer0, block_name.send, block_name.dirt, block_name.grass_dirt,
    };
    public static void init()
    {
        if (run_init) return;
        run_init = true;
        sprites = Resources.LoadAll<Sprite>("blocks");
        block_ban = new bool[sprites.Length];
        Fire.d_sprite = sprite.fire;
        wool_color = new Color[16];
        string[] rgb = {
            "#E9ECEC", "#F07613", "#BD44B3", "#3AAFD9",
            "#F8C627", "#70B919", "#ED8DAC", "#3E4447",
            "#8E8E86", "#158991", "#792AAC", "#35399D",
            "#724728", "#546D1B", "#A12722", "#141519",
        };
        for (int i = 0; i < 16; i++)
            wool_color[i] = Tool.str2rgb(rgb[i]);
    }
    public static Block name2block(block_name name)
    {
        GameObject go = new GameObject();
        Block b = name switch
        {
            block_name.dirt => go.AddComponent<Dirt>(),
            block_name.grass_dirt => go.AddComponent<Dirt>(),
            block_name.send => go.AddComponent<Send>(),
            block_name.wood => go.AddComponent<Wood>(),
            block_name.leaf => go.AddComponent<Combustibles>(),
            block_name.plank => go.AddComponent<Combustibles>(),
            block_name.bad_rock => go.AddComponent<BedRock>(),
            block_name.glow_stone => go.AddComponent<GlowStone>(),
            block_name.glass => go.AddComponent<Glass>(),
            block_name.ice => go.AddComponent<Ice>(),
            block_name.worktable0 => go.AddComponent<Combustibles>(),
            block_name.worktable1 => go.AddComponent<Combustibles>(),
            block_name.bookshelf => go.AddComponent<Combustibles>(),
            block_name.hay_block => go.AddComponent<Combustibles>(),
            block_name.wool => go.AddComponent<Combustibles>(),
            block_name.furnace0 => go.AddComponent<Furnace>(),
            block_name.water => go.AddComponent<WaterBlock>(),
            block_name.lava => go.AddComponent<LavaBlock>(),
            block_name.redstone_block => go.AddComponent<RedBlock>(),
            block_name.lamp0 => go.AddComponent<RedLamp>(),
            block_name.piston0 => go.AddComponent<Piston>(),
            block_name.piston1 => go.AddComponent<StickyPiston>(),
            block_name.tnt => go.AddComponent<TNT>(),
            block_name.structure0 => go.AddComponent<DataStructure>(),
            block_name.cmd0 => go.AddComponent<Block>(),
            block_name.observer0 => go.AddComponent<Observer>(),
            _ => go.AddComponent<Block>(),
        };
        b.id = (int)name;
        switch (name)
        {
            case block_name.leaf:
                b.start_sprite();
                b.sr.color = Color.green;
                break;
            case block_name.water:
                b.start_sprite();
                b.sr.color = water_color;
                break;
            case block_name.wool:
                b.start_sprite();
                b.sr.color = wool_color[Random.Range(0, 16)];
                break;
            case block_name.piston0:
            case block_name.piston1:
            case block_name.observer0:
                b.d = 0;
                break;
        }
        return b;
    }
    public static Sprite name2sprite(block_name name)
    {
        return sprites[(int)name];
    }
    public static Block get_random_block()
    {
        block_name[] group;
        if (Random.Range(0, 1f) < rate_range1) group = func_block_gorup;
        else if (Random.Range(0, 1f) < rate_range2) group = feat_block_gorup;
        else group = easy_block_gorup;
        return name2block(group[Random.Range(0, group.Length)]);
    }
    public static Block get_random_block_redstone()
    {
        if (Random.Range(0, 1f) < 0.4f) return name2block(redstone_mode_group[Random.Range(0, 3)]);
        return name2block(redstone_mode_group[Random.Range(0, redstone_mode_group.Length)]);
    }
    public static void lay_clip(int id)
    {
        int i = (block_name)id switch
        {
            block_name.wool => 0,
            block_name.grass_dirt => 1,
            block_name.lava => 3,
            block_name.send => 4,
            block_name.water => 7,
            block_name.plank => 8,
            block_name.wood => 8,
            _ => 6,
        };
        sounds.lay_clip(i);
    }
    public static string package_save()
    {
        string data = "";
        for (int x = 1; x <= W; ++x)
            for (int y = 1; y <= H; ++y)
                if (block[x, y]) data += block[x, y].package_save() + ";";
        return data;
    }
    public static void package_load(string data)
    {
        foreach (string s in data.Split(';'))
        {
            if (s.Length < 2) continue;
            int ind = s.IndexOf(':');
            name2block((block_name)int.Parse(s.Substring(0, ind))).package_load(s.Substring(ind + 1));
        }
    }
}
public class BlockCreate
{
    private GameBase.block_name[] func_block_gorup, feat_block_gorup, easy_block_gorup;
    public float range1, range2, range3;
    public void init(bool[] ban_state, float range1, float range2, float range3)
    {
        bool[] use_state = new bool[BlockData.sprites.Length];
        for (int i = 0; i < ban_state.Length; i++)
            use_state[(int)BlockData.use_block[i]] = !ban_state[i];
        List<GameBase.block_name> list = new List<GameBase.block_name>();
        DataStructure.ban = !use_state[(int)GameBase.block_name.structure0];
        foreach (var i in BlockData.func_block_gorup)
            if (use_state[(int)i]) list.Add(i);
        func_block_gorup = list.ToArray();
        list.Clear();
        foreach (var i in BlockData.feat_block_gorup)
            if (use_state[(int)i]) list.Add(i);
        feat_block_gorup = list.ToArray();
        list.Clear();
        foreach (var i in BlockData.easy_block_gorup)
            if (use_state[(int)i]) list.Add(i);
        easy_block_gorup = list.ToArray();
        if (func_block_gorup.Length == 0) range1 = 0;
        if (feat_block_gorup.Length == 0) range2 = 0;
        if (easy_block_gorup.Length == 0) range3 = 0;
        this.range1 = range1;
        this.range2 = range1 + range2;
        this.range3 = range1 + range2 + range3;
    }
    public Block get_random_block()
    {
        GameBase.block_name[] group;
        float i = Random.Range(0, range3);
        if (i < range1) group = func_block_gorup;
        else if (i < range2) group = feat_block_gorup;
        else group = easy_block_gorup;
        return BlockData.name2block(group[Random.Range(0, group.Length)]);
    }
}
public class BlockBase : GameBase
{
    public static Queue<(int, int, int)> signal_list;
    public static List<Block> updater_list;
    public static int[] block_top;
    public static int[] block_line;
    public static void init()
    {
        if (signal_list == null)
        {
            signal_list = new Queue<(int, int, int)>();
            updater_list = new List<Block>();
        }
        block_top = new int[W + 2];
        block_line = new int[H + 2];
        block_top[0] = block_top[W + 1] = 0;
        for (int i = 1; i <= W; ++i) block_top[i] = H;
    }
    public static void update()
    {
        List<Block> list = updater_list.GetRange(0, updater_list.Count);
        foreach (Block b in list) if (b && b.run && !b.move) b.updater();
        foreach ((int x, int y, int t) in signal_list)
            if (block[x, y]) block[x, y].play(t);
        signal_list.Clear();
        list.Clear();
    }
    public static void match_update_line()
    {
        // debug...
        for (int y = 1; y <= H; y++)
        {
            int cnt = 0;
            for (int x = 1; x <= W; x++) if (block[x, y]) cnt++;
            if (cnt != block_line[y])
            {
                //print($"line {y}: {block_line[y]}=>{cnt}");
                block_line[y] = cnt;
            }
        }
        //print(Tool.array2str(block_line));1
    }
}
public class Block : BlockBase
{
    public static Transform parent = null;
    public int x, y, d = -1, tot, light_i, id;
    public SpriteRenderer sr;
    public bool canPass = true, canPush = true, canFire = false, run = false;
    public blockMove move;
    public void rotate_sprite(int d)
    {
        if (this.d < 0) return;
        transform.Rotate(new Vector3(0, 0, 90 * (d - this.d)));
        this.d = d;
    }
    public void start_sprite()
    {
        if (sr) return;
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = BlockData.sprites[id];
    }
    public void update_status()
    {
        if (block[x, y])
        {
            if (block_top[x] >= y) block_top[x] = y - 1;
            block_line[y]++;
        }
        else
        {
            if (block_top[x] + 1 == y)
            {
                int t = y + 1;
                while (!block[x, t]) t++;
                block_top[x] = t - 1;
            }
            block_line[y]--;
        }
    }
    public virtual Block start(int x, int y)
    {
        start_sprite();
        name = ((block_name)id).ToString();
        transform.localPosition = new Vector2(x, -y);
        this.x = x;
        this.y = y;
        block[x, y] = this;
        transform.parent = parent;
        if (run) updater_list.Add(this);
        if (light_i > 0) lights.add_light_source(this);
        update_status();
        return this;
    }
    public virtual void delete()
    {
        block[x, y] = null;
        if (run) updater_list.Remove(this);
        lights.del_light_source(this);
        Destroy(gameObject);
        update_status();
    }
    public virtual void updater() { }
    public virtual void play(int d) { }
    public virtual void catch_fire() { }
    public virtual void heavy_load() { }
    public Block get_up_block()
    {
        int i = y - 1;
        while (!block[x, i]) --i;
        return block[x, i];
    }
    public void move_block(int dx, int dy, bool show_update = true, float run_time = 0.2f)
    {
        block[x, y] = null;
        update_status();
        x += dx;
        y += dy;
        block[x, y] = this;
        update_status();
        if (!show_update) return;
        Vector2 v = new Vector2(dx, -dy);
        if (move == null) gameObject.AddComponent<blockMove>().start(this, v).set(run_time);
        else move.v += v;
    }
    public virtual void clear_up()
    {
        Block fade = Instantiate(this);
        fade.gameObject.SendMessage("stop_animation", options: SendMessageOptions.DontRequireReceiver);
        fade.gameObject.AddComponent<FadeOut>().start(fade.sr);
        delete();
    }
    public virtual string package_save()
    {
        return string.Format("{0}:{1} {2}", id, x, y);
    }
    public virtual Block package_load(string s)
    {
        string[] a = s.Split(' ');
        return start(int.Parse(a[0]), int.Parse(a[1]));
    }
}
public class GroupBlock : GameBase
{
    static private int[,,,] n =
    {
        {{{1, 0}, {1, 1}, {2, 1}, {2, 0}}, {{2, 0}, {1, 0}, {1, 1}, {2, 1}}, {{2, 1}, {2, 0}, {1, 0}, {1, 1}}, {{1, 1}, {2, 1}, {2, 0}, {1, 0}}},
        {{{0, 1}, {1, 1}, {2, 1}, {3, 1}}, {{2, 0}, {2, 1}, {2, 2}, {2, 3}}, {{3, 2}, {2, 2}, {1, 2}, {0, 2}}, {{1, 3}, {1, 2}, {1, 1}, {1, 0}}},
        {{{0, 0}, {0, 1}, {1, 1}, {2, 1}}, {{2, 0}, {1, 0}, {1, 1}, {1, 2}}, {{2, 2}, {2, 1}, {1, 1}, {0, 1}}, {{0, 2}, {1, 2}, {1, 1}, {1, 0}}},
        {{{2, 0}, {0, 1}, {1, 1}, {2, 1}}, {{2, 2}, {1, 0}, {1, 1}, {1, 2}}, {{0, 2}, {2, 1}, {1, 1}, {0, 1}}, {{0, 0}, {1, 2}, {1, 1}, {1, 0}}},
        {{{0, 1}, {1, 1}, {1, 0}, {2, 0}}, {{1, 0}, {1, 1}, {2, 1}, {2, 2}}, {{2, 1}, {1, 1}, {1, 2}, {0, 2}}, {{1, 2}, {1, 1}, {0, 1}, {0, 0}}},
        {{{0, 0}, {1, 1}, {1, 0}, {2, 1}}, {{2, 0}, {1, 1}, {2, 1}, {1, 2}}, {{2, 2}, {1, 1}, {1, 2}, {0, 1}}, {{0, 2}, {1, 1}, {0, 1}, {1, 0}}},
        {{{1, 0}, {0, 1}, {1, 1}, {2, 1}}, {{2, 1}, {1, 0}, {1, 1}, {1, 2}}, {{1, 2}, {2, 1}, {1, 1}, {0, 1}}, {{0, 1}, {1, 2}, {1, 1}, {1, 0}}},
    };
    static private int[,,] SRS_data = {
        {{-1, 0}, {-1, -1}, {0,  2}, {-1,  2}}, {{ 1, 0}, { 1, -1}, {0,  2}, { 1,  2}},
        {{ 1, 0}, { 1,  1}, {0, -2}, { 1, -2}}, {{ 1, 0}, { 1,  1}, {0, -2}, { 1, -2}},
        {{ 1, 0}, { 1, -1}, {0,  2}, { 1,  2}}, {{-1, 0}, {-1, -1}, {0,  2}, {-1,  2}},
        {{-1, 0}, {-1,  1}, {0, -2}, {-1, -2}}, {{-1, 0}, {-1,  1}, {0, -2}, {-1, -2}},
        {{-2, 0}, { 1, 0}, {-2,  1}, { 1, -2}}, {{-1, 0}, { 2, 0}, {-1, -2}, { 2,  1}},
        {{-1, 0}, { 2, 0}, {-1, -2}, { 2,  1}}, {{ 2, 0}, {-1, 0}, { 2, -1}, {-1,  2}},
        {{ 2, 0}, {-1, 0}, { 2, -1}, {-1,  2}}, {{ 1, 0}, {-2, 0}, { 1,  2}, {-2, -1}},
        {{ 1, 0}, {-2, 0}, { 1,  2}, {-2, -1}}, {{-2, 0}, { 1, 0}, {-2,  1}, { 1, -2}},
    };
    static private Queue<int> next_type = new Queue<int>();
    public Block[] sub;
    public bool use_anim = true;
    public int x, y, t, d;
    public GroupBlock start()
    {
        x = y = d = 0;
        t = get_next_type();
        sub = new Block[4];
        Block template = BlockData.create_func();
        template.start_sprite();
        template.sr.sortingOrder = 9;
        for (int i = 0; i < 4; ++i)
        {
            Block b;
            if (!DataStructure.ban && Random.Range(0, 1f) < 0.08f)
            {
                DataStructure.ban = true;
                b = BlockData.name2block(block_name.structure0);
                b.start_sprite();
                b.sr.sortingOrder = 9;
            }
            else b = Instantiate(template);
            b.rotate_sprite(Random.Range(0, 4));
            b.transform.position = new Vector2(n[t, 0, i, 0], -n[t, 0, i, 1]);
            b.transform.parent = transform;
            sub[i] = b;
        }
        Destroy(template.gameObject);
        return this;
    }
    private int get_next_type()
    {
        while (next_type.Count < 8)
        {
            int[] n = new int[] { 0, 1, 2, 3, 4, 5, 6 };
            for (int i = 0; i < 7; ++i)
            {
                int j = Random.Range(0, 7), t = n[j];
                n[j] = n[i];
                n[i] = t;
            }
            foreach (int i in n) next_type.Enqueue(i);
        }
        return next_type.Dequeue();
    }
    public IEnumerable<(int, int, int)> get_position()
    {
        for (int i = 0; i < 4; ++i)
            yield return (i, x + n[t, d, i, 0], y + n[t, d, i, 1]);
    }
    public bool move_check(int dx, int dy)
    {
        x += dx;
        y += dy;
        for (int i = 0; i < 4; ++i)
        {
            int vx = x + n[t, d, i, 0], vy = y + n[t, d, i, 1];
            if (vx < 1 || W < vx || vy < 1 || H < vy || block[vx, vy])
            {
                x -= dx;
                y -= dy;
                return false;
            }
        }
        if (use_anim) StartCoroutine(move_anim(transform, dx, dy));
        else transform.position = new Vector2(x, -y);
        return true;
    }
    public bool spin_check(int dx, int dy, int q)
    {
        int od = d;
        d = (d + q) % 4;
        if (move_check(dx, dy))
        {
            for (int i = 0; i < 4; ++i)
                if (use_anim) StartCoroutine(move_anim(sub[i].transform, n[t, d, i, 0] - n[t, od, i, 0], n[t, d, i, 1] - n[t, od, i, 1]));
                else sub[i].transform.localPosition = new Vector2(n[t, d, i, 0], -n[t, d, i, 1]);
            return true;
        }
        d = od;
        return false;
    }
    public void SRS(bool reverse)
    {
        if (t == 0) return;
        int i1 = d * 2 + (reverse ? 1 : 0) + (t == 1 ? 8 : 0);
        int i2 = reverse ? 3 : 1;
        for (int i = 0; i < 4; ++i)
            if (spin_check(SRS_data[i1, i, 0], SRS_data[i1, i, 1], i2)) break;
    }
    public void SRS(bool reverse, int mode)
    {
        if (t == 0) return;
        int I = d * 2 + (reverse ? 1 : 0) + (t == 1 ? 8 : 0);
        spin_check(SRS_data[I, mode, 0], SRS_data[I, mode, 1], reverse ? 3 : 1);
    }
    public string package_save()
    {
        int only = -1;
        string s = "";
        for (int i = 0; i < 4; i++)
        {
            if (sub[i].id == (int)block_name.structure0) only = i;
            s += "," + sub[i].d;
        }
        return $"{t},{sub[only == 0 ? 1 : 0].id},{only}" + s;
    }
    public void package_load(string data)
    {
        string[] s = data.Split(',');
        x = y = d = 0;
        t = int.Parse(s[0]);
        sub = new Block[4];
        Block template = BlockData.name2block((block_name)int.Parse(s[1]));
        template.start_sprite();
        template.sr.sortingOrder = 9;
        int only = int.Parse(s[2]);
        for (int i = 0; i < 4; ++i)
        {
            Block b;
            if (i == only)
            {
                DataStructure.ban = true;
                b = BlockData.name2block(block_name.structure0);
                b.start_sprite();
                b.sr.sortingOrder = 9;
            }
            else b = Instantiate(template);
            b.rotate_sprite(int.Parse(s[i + 3]));
            b.transform.position = new Vector2(n[t, 0, i, 0], -n[t, 0, i, 1]);
            b.transform.parent = transform;
            sub[i] = b;
        }
        Destroy(template.gameObject);
    }
    IEnumerator move_anim(Transform target, int x, int y)
    {
        Vector2 vec = new Vector2(x, -y) / 10f;
        for (int i = 0; i < 10; i++)
        {
            target.Translate(vec, Space.World);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
class BedRock : Block
{
    public override Block start(int x, int y)
    {
        canPush = false;
        return base.start(x, y);
    }
}
class Dirt : Block
{
    private bool top_space;
    public override Block start(int x, int y)
    {
        run = true;
        top_space = id == (int)block_name.grass_dirt;
        return base.start(x, y);
    }
    public override void updater()
    {
        if (top_space)
        {
            if (block[x, y - 1])
            {
                sr.sprite = BlockData.name2sprite(block_name.dirt);
                top_space = false;
            }
        }
        else if (!block[x, y - 1])
        {
            sr.sprite = BlockData.name2sprite(block_name.grass_dirt);
            top_space = true;
        }
    }
}
class Combustibles : Block
{
    public override Block start(int x, int y)
    {
        canFire = true;
        return base.start(x, y);
    }
    public override void catch_fire()
    {
        if (!canFire) return;
        canFire = false;
        light_i = 5;
        lights.add_light_source(this);
        gameObject.AddComponent<Fire>().start(this);
        sounds.run_clip(MainAudio.run.fire);
    }
}
class GlowStone : Block
{
    public override Block start(int x, int y)
    {
        canPass = false;
        light_i = 5;
        return base.start(x, y);
    }
}
class Glass : Block
{
    public override Block start(int x, int y)
    {
        canPass = false;
        return base.start(x, y);
    }
    public override void heavy_load()
    {
        sounds.run_clip(MainAudio.run.glass);
        delete();
    }
}
class Furnace : Block
{
    public static float fire_buff = 4;
    public float last_time;
    public override Block start(int x, int y)
    {
        canFire = true;
        return base.start(x, y);
    }
    public override void updater()
    {
        if (Anim.time < last_time + fire_buff) return;
        last_time = Anim.time;
        foreach ((int dx, int dy) in OUTSIDE_4)
        {
            Block b = block[x + dx, y + dy];
            if (b && b.canFire) b.catch_fire();
        }
    }
    public override void catch_fire()
    {
        last_time = Anim.time;
        sr.sprite = BlockData.name2sprite(block_name.furnace1);
        run = true;
        canFire = false;
        light_i = 5;
        updater_list.Add(this);
        lights.add_light_source(this);
    }
}
class PumpkinHead : Block
{

} // no finish
class Ice : Block
{
    private float malt_time;
    public override Block start(int x, int y)
    {
        run = canFire = true;
        return base.start(x, y);
    }
    public override void updater()
    {
        int value = lights.value[x, y];
        if (weather.type != 2 && value > 3)
        {
            if (malt_time == 0) malt_time = Anim.time + Random.Range(5f, 10f);
            else if (malt_time < Anim.time) melt();
        }
        else malt_time = 0;
    }
    void melt()
    {
        for (int i = 0; i < WaterBase.unit; ++i)
        {
            if (water.w[x, y, i]) water.w[x, y, i].overflow();
            else water.add(x, y, i);
        }
        delete();
    }
    public override void catch_fire()
    {
        melt();
    }
}
class Piston : Block
{
    public const float move_run_time = 0.3f;
    public PistonArm arm;
    public bool arm_loss;
    public override Block start(int x, int y)
    {
        run = true;
        canPass = false;
        return base.start(x, y);
    }
    public override void updater()
    {
        if (arm_loss) delete();
        else if (tot == 0) return;
        else if (tot == 1)
        {
            tot = 0;
            sleep();
        }
        else
        {
            --tot;
        }
    }
    public override void play(int d)
    {
        if (move || (this.d ^ d) == 2) return;
        if (tot != 0) { tot = 3; return; }
        (int dx, int dy) = OUTSIDE_4[this.d];
        int p = 1;
        Block tmp;
        while ((tmp = block[x + dx * p, y + dy * p]) && tmp.canPush) ++p;
        if (tmp) return;
        tot = 3;
        canPush = false;
        for (int i = p - 1; 0 < i; --i)
            block[x + dx * i, y + dy * i].move_block(dx, dy, run_time: move_run_time);
        arm = new GameObject().AddComponent<PistonArm>().start(this);
        gameObject.AddComponent<blockMove>().start(this, Vector2.zero).set(move_run_time);
        sr.sprite = BlockData.name2sprite(block_name.piston2);
        sounds.run_clip(MainAudio.run.piston_in);
    }
    public virtual void sleep()
    {
        detele_arm();
        gameObject.AddComponent<blockMove>().start(this, Vector2.zero).set(move_run_time);
        canPush = true;
        sr.sprite = BlockData.name2sprite(block_name.piston0);
        sounds.run_clip(MainAudio.run.piston_out);
    }
    public void detele_arm()
    {
        if (!arm) return;
        arm.sleep();
        arm = null;
    }
    public override void clear_up()
    {
        if (arm) arm.arm_loss = true;
        base.clear_up();
    }
    public override string package_save()
    {
        return string.Format("{0}:{1} {2} {3}", id, x, y, d);
    }
    public override Block package_load(string s)
    {
        string[] a = s.Split(' ');
        start_sprite();
        d = 0;
        rotate_sprite(int.Parse(a[2]));
        return start(int.Parse(a[0]), int.Parse(a[1]));
    }
}
class PistonArm : Block
{
    public const float move_run_time = 0.3f;
    public Piston root;
    public bool arm_loss;
    public PistonArm start(Piston root)
    {
        this.root = root;
        d = root.d;
        (int dx, int dy) = OUTSIDE_4[d];
        x = root.x + dx;
        y = root.y + dy;
        transform.Rotate(new Vector3(0, 0, 90 * d));
        canPass = canPush = false;
        run = true;
        base.start(x, y);
        name = "arm";
        sr.sprite = BlockData.name2sprite(block_name.piston3);
        gameObject.AddComponent<AnShift>().start(new Vector2(-dx, dy)).set(move_run_time, Anim.reverse(Anim.smooth));
        return this;
    }
    public void sleep()
    {
        Block fade = Instantiate(this);
        (int dx, int dy) = OUTSIDE_4[d];
        fade.gameObject.SendMessage("stop_animation", options: SendMessageOptions.DontRequireReceiver);
        fade.gameObject.AddComponent<FadeOut>().start(fade.sr).set(move_run_time);
        fade.gameObject.AddComponent<AnShift>().start(new Vector2(-dx, dy)).set(move_run_time);
        delete();
    }
    public override void updater()
    {
        if (arm_loss) sleep();
    }
    public override void clear_up()
    {
        root.arm_loss = true;
        base.clear_up();
    }
    public override string package_save()
    {
        return "";
    }
}
class StickyPiston : Piston
{
    public override void play(int d)
    {
        base.play(d);
        if (arm) arm.sr.sprite = BlockData.name2sprite(block_name.piston4);
    }
    public override void sleep()
    {
        base.sleep();
        (int dx, int dy) = OUTSIDE_4[d];
        sr.sprite = BlockData.name2sprite(block_name.piston1);
        Block b = block[x + dx * 2, y + dy * 2];
        if (b && b.canPush) b.move_block(-dx, -dy, run_time: move_run_time);
    }
    public override string package_save()
    {
        return string.Format("{0}:{1} {2} {3}", id, x, y, d);
    }
    public override Block package_load(string s)
    {
        string[] a = s.Split(' ');
        start_sprite();
        d = 0;
        rotate_sprite(int.Parse(a[2]));
        return start(int.Parse(a[0]), int.Parse(a[1]));
    }
}
class RedBlock : Block
{
    public override Block start(int x, int y)
    {
        run = true;
        light_i = 2;
        return base.start(x, y);
    }
    public override void updater()
    {
        int i = 0;
        foreach ((int dx, int dy) in OUTSIDE_4)
        {
            signal_list.Enqueue((x + dx, y + dy, i));
            ++i;
        }
    }
}
class Send : Block
{
    public override Block start(int x, int y)
    {
        run = true;
        return base.start(x, y);
    }
    public override void updater()
    {
        if (!block[x, y + 1]) move_block(0, 1);
    }
}
class Wood : Combustibles
{
    public override Block start(int x, int y)
    {
        run = true;
        return base.start(x, y);
    }
    public override void updater()
    {
        foreach ((int dx, int dy) in OUTSIDE_4)
        {
            if (!block[x + dx, y + dy])
            {
                Block b = BlockData.name2block(block_name.leaf).start(x + dx, y + dy);
                b.gameObject.AddComponent<FadeIn>().start(b.sr);
            }
        }
    }
    public override void catch_fire()
    {
        if (!canFire) return;
        base.catch_fire();
        run = false;
        updater_list.Remove(this);
    }
}
class RedLamp : Block
{
    public override Block start(int x, int y)
    {
        run = true;
        lights.add_light_source(this);
        return base.start(x, y);
    }
    public override void updater()
    {
        if (tot > 0)
        {
            --tot;
            if (tot == 0)
            {
                sr.sprite = BlockData.name2sprite(block_name.lamp0);
                light_i = 0;
            }
        }
    }
    public override void play(int d)
    {
        if (tot != 0) { tot = 3; return; }
        tot = 3;
        sr.sprite = BlockData.name2sprite(block_name.lamp1);
        light_i = 5;
        sounds.run_clip(MainAudio.run.click);
    }
}
class Observer : Block
{
    public Block target;
    public int tar_tot;
    public override Block start(int x, int y)
    {
        run = true;
        canPass = false;
        return base.start(x, y);
    }
    public override void updater()
    {
        (int dx, int dy) = OUTSIDE_4[d];
        if (tot == 0)
        {
            sr.sprite = BlockData.name2sprite(block_name.observer0);
            Block new_target = block[x - dx, y - dy];
            if (target)
            {
                if (target.tot == tar_tot && target == new_target) return;
                target = null;
            }
            else
            {
                target = new_target;
                if (target == null) return;
                tar_tot = target.tot;
            }
            sr.sprite = BlockData.name2sprite(block_name.observer1);
            tot = 3;
        }
        --tot;
        (int ox, int oy) = (x + dx, y + dy);
        signal_list.Enqueue((ox, oy, d));
        if (block[ox, oy] && block[ox, oy].canPass)
        {
            int d1 = 0;
            foreach ((int ddx, int ddy) in OUTSIDE_4)
            {
                if (d1 != d) signal_list.Enqueue((ox + ddx, oy + ddy, d1));
                ++d1;
            }
        }
    }
}
class WaterBlock : Block
{
    public Water root;
    public override Block start(int x, int y)
    {
        run = true;
        root = water;
        return base.start(x, y);
    }
    public override void updater()
    {
        delete();
        root.add_more(x, y, WaterBase.unit);
    }
    private void Update()
    {
        sr.sprite = sprite.water.get_sprite();
    }
}
class LavaBlock : WaterBlock
{
    public override Block start(int x, int y)
    {
        base.start(x, y);
        root = lava;
        return this;
    }
    private void Update()
    {
        sr.sprite = sprite.lava.get_sprite();
    }
}
class DataStructure : Block
{
    public static bool world = false, ban = false;
    public static string data;
    public bool work = true;
    public static void turn_ban()
    {
        world = ban = false;
        data = null;
    }
    public override void delete()
    {
        if (work)
        {
            if (data != null)
            {
                work = false;
                world = true;
            }
            else turn_ban();
        }
        base.delete();
    }
    public override void play(int d)
    {
        if (data != null || !work) return;
        sr.sprite = BlockData.name2sprite(block_name.structure1);
        work = false;
        data = main.game_world_save();
        work = true;
        sounds.run_clip(MainAudio.run.click);
    }
    public override string package_save()
    {
        int sta = work ? (data == null ? 0 : 1) : 2;
        return string.Format("{0}:{1} {2} {3}", id, x, y, sta);
    }
    public override Block package_load(string s)
    {
        string[] a = s.Split(' ');
        int sta = a[2][0] - '0';
        work = sta < 2;
        if (work)
        {
            world = false;
            ban = true;
        }
        start_sprite();
        if (sta > 0) sr.sprite = BlockData.name2sprite(sta == 1 ? block_name.structure1 : block_name.structure2);
        return start(int.Parse(a[0]), int.Parse(a[1]));
    }
}
class TNT : Block
{
    public override Block start(int x, int y)
    {
        canFire = true;
        return base.start(x, y);
    }
    public override void catch_fire()
    {
        play(0);
    }
    public override void play(int d)
    {
        new GameObject("run tnt").AddComponent<RunTNT>().start(x, y);
        sounds.run_clip(MainAudio.run.fuse);
        delete();
    }
}
class RunTNT : GameBase
{
    public static int next_d = 1;
    public static float run_time = 5f;
    public float x, y, dx, dy, ddx;
    public int d;
    public SpriteRenderer sr;
    public float start_time;
    public void start(int x, int y)
    {
        this.x = x;
        this.y = y;
        start_time = Anim.time;
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.transform.position = new Vector2(x, -y);
        if (!block[x + next_d, y]) d = next_d;
        else if (!block[x - next_d, y]) d = -next_d;
        else d = 0;
        next_d = -next_d;
        dx = 0.05f * d;
        ddx = -0.01f * d;
        dy = -0.05f;
    }
    void Update()
    {
        if (!run_game)
        {
            Destroy(gameObject);
            return;
        }
        int x1 = (int)(x + dx + 0.5f), y1 = (int)Mathf.Ceil(y + dy);
        if (Anim.time >= start_time + run_time)
        {
            y1--;
            int endx = Mathf.Min(W, x1 + 3), endy = Mathf.Min(H, y1 + 3);
            Block b;
            for (int i = Mathf.Max(1, x1 - 3); i <= endx; ++i)
                for (int j = Mathf.Max(1, y1 - 3); j <= endy; ++j)
                    if ((b = block[i, j]) && b.canPush)
                    {
                        int dx = Mathf.Abs(i - x1), dy = Mathf.Abs(j - y1);
                        if (b.canFire) b.catch_fire();
                        if (Mathf.Max(dx, dy) < 3 || Random.Range(4, 8) > (dx + dy)) b.delete();
                    }
            sounds.run_clip(MainAudio.run.explode);
            Destroy(gameObject);
            new GameObject("boom").AddComponent<BoomCircle>().transform.position = transform.position;
        }
        else sr.sprite = BlockData.name2sprite((int)(Anim.time * 2) % 2 == 0 ? block_name.tnt : block_name.white);
        x += dx;
        y += dy;
        if (block[x1, y1] || block[x1 + d, y1])
        {
            if (dy == 0) return;
            y = y1 - 1;
            sr.transform.position = new Vector2(x, -y);
            dx = dy = 0;
            return;
        }
        sr.transform.Translate(new Vector2(dx, -dy));
        if (dy < 0.2f) dy += 0.01f;
        if (dx != 0) dx += ddx;
    }
}
class BoomCircle : MonoBehaviour
{
    private Material mat;
    private float start_time;
    private void Start()
    {
        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
        mat = sr.material = new Material(GameBase.sprite.boom_shader);
        sr.sprite = GameBase.sprite.white;
        transform.localScale = new Vector3(6, 6, 1);
        start_time = Anim.time;
        mat.SetFloat("_Cut", Random.Range(0, 2 * Mathf.PI));
    }
    private void Update()
    {
        float rate = Anim.time - start_time;
        if (rate >= 1) Destroy(gameObject);
        else mat.SetFloat("_Rate", rate);
    }
}
class Fire : MonoBehaviour
{
    public static DynamicSprite d_sprite;
    public static float run_time;
    public float start_time, hand_out;
    public Block block;
    SpriteRenderer sr;
    public void start(Block block)
    {
        GameObject go = new GameObject("fire");
        go.transform.parent = block.transform;
        go.transform.position = block.transform.position;
        sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 1;
        this.block = block;
        start_time = Anim.time;
        run_time = Random.Range(0, 10) + 10f;
        hand_out = Random.Range(run_time / 2, run_time) + start_time;
    }
    public void stop_animation() { Destroy(this); }

    public void Update()
    {
        sr.sprite = d_sprite.get_sprite();
        if (Anim.time >= start_time + run_time)
        {
            block.delete();
        }
        else if (Anim.time >= hand_out)
        {
            GameBase.sounds.run_clip(MainAudio.run.fire);
            int x = block.x, y = block.y;
            foreach ((int dx, int dy) in Block.OUTSIDE_4)
            {
                Block b = Block.block[x + dx, y + dy];
                if (b && b.canFire) b.catch_fire();
            }
            hand_out = start_time + run_time;
        }
    }
}
