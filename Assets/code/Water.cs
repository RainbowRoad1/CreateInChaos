using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaterBase : GameBase
{
    public static int cobble_id;
    public static int unit = 16, move_cnt = 0, v = 1;
    private static float re_start = 0, re_diff = 1;
    public static void v_update()
    {
        if (move_cnt == 0 && re_start + re_diff < Anim.time)
        {
            re_start = Anim.time;
            v = -v;
        }
        move_cnt = 0;
    }
}
public class WaterNode : WaterBase
{
    public Water root;
    public int x, y, n, len, ty, tn;
    public bool stop = false;
    public SpriteRenderer sr;
    static int id;
    public WaterNode start(Water root, int x, int y, int n)
    {
        name = $"w{id++}";
        len = 1;
        this.x = x;
        this.y = ty = y;
        this.n = tn = n;
        this.root = root;
        root.w[x, y, n] = this;
        root.value_add(x, y);
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = root.now_sprite;
        sr.color = root.color;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(1, 1f / unit);
        sr.transform.position = new Vector2(x, 1 - y - (unit / 2f + n + 0.5f) * (1f / unit));
        gameObject.transform.parent = root.transform;
        return this;
    }
    public void delete()
    {
        fill(null);
        root.node_list.Remove(this);
        for (int i = ty; i <= y; i++) root.value_sum(x, i);
        Destroy(gameObject);
    }
    public void sr_update(float x)
    {
        x /= unit;
        sr.size += new Vector2(0, x);
        sr.transform.Translate(Vector2.up * (x * 0.5f));
    }
    public void fill(WaterNode node)
    {
        int i = ty, j = tn;
        while (!(i == y && j == n))
        {
            root.w[x, i, j] = node;
            if (++j == unit) { ++i; j = 0; }
        }
        root.w[x, i, j] = node;
    }
    public void link()
    {
        WaterNode a = n + 1 == unit ? root.w[x, y + 1, 0] : root.w[x, y, n + 1];
        if (!a) return;
        delete();
        fill(a);
        for (int i = ty; i <= y; i++) root.value_sum(x, i);
        a.ty = ty;
        a.tn = tn;
        a.len += len;
        a.sr_update(len);
    }
    public void overflow()
    {
        for (int i = y; ty <= i; --i)
            if (block[x, i])
            {
                delete();
                root.add_more(x, y, len);
                return;
            }
    }
    public bool add_top()
    {
        if (--tn < 0) { tn = unit - 1; --ty; }
        if (block[x, ty] || root.w[x, ty, tn])
        {
            if (++tn == unit) { tn = 0; ++ty; }
            return false;
        }
        root.w[x, ty, tn] = this;
        return true;
    }
    public void del_top()
    {
        root.w[x, ty, tn] = null;
        if (++tn == unit) { ++ty; tn = 0; }
    }
    public void fall()
    {
        stop = n + 1 == unit && block[x, y + 1];
        if (stop) return;
        root.value_del(x, ty);
        del_top();
        if (++n == unit) { ++y; n = 0; }
        root.w[x, y, n] = this;
        root.value_add(x, y);
        sr.transform.Translate(Vector2.down * (1f / unit));
    }
    public int move_find(int ox, int diff)
    {
        WaterNode a;
        for (int i = y; ty <= i; --i)
        {
            if (block[ox, i]) continue;
            a = root.w[ox, i, n];
            if (!a) return i == ty && tn + diff == unit ? 0 : i;
            if (a.ty < ty || a.ty == ty && a.tn <= tn + diff) return 0;
            if (tn + diff == unit && a.ty == ty + 1) return 0;
            if (a.add_top())
            {
                a.del_top();
                return i;
            }
        }
        return 0;
    }
    public void move()
    {
        if (!stop) return;
        if (len == 1)
        {
            if (!block[x + v, y] && !root.w[x + v, y, n])
            {
                move_cnt++;
                root.w[x, y, n] = null;
                root.value_del(x, y);
                root.w[x += v, y, n] = this;
                root.value_add(x, y);
                sr.transform.Translate(new Vector2(v, 0));
            }
            return;
        }
        int a1 = move_find(x + v, 0), a2 = move_find(x - v, 1), t;
        WaterNode w1 = root.w[x + v, a1, n], w2 = root.w[x - v, a2, n];
        if (a1 == 0 && a2 == 0) return;
        else if (a2 == 0 && a1 != 0) t = 0;
        else if (a1 == 0 && a2 != 0) t = 1;
        else
        {
            int h1 = w1 ? w1.ty * unit + w1.tn : (a1 + 1) * unit;
            int h2 = w2 ? w2.ty * unit + w2.tn : (a2 + 1) * unit;
            t = h1 < h2 ? 1 : 0;
        }
        WaterNode w = t == 0 ? w1 : w2;
        if (w)
        {
            w.add_top();
            w.sr_update(1);
            w.len++;
            root.value_add(w.x, w.ty);
        }
        else if (t == 0) root.add(x + v, a1, n);
        else root.add(x - v, a2, n);
        root.value_del(x, ty);
        sr_update(-1);
        del_top();
        len--;
        move_cnt++;
    }
}
public class WaterTmpBlock : Block
{
    public WaterNode node;
    public bool del = false;
    public WaterTmpBlock start(WaterNode node, int x, int y)
    {
        this.node = node;
        this.x = x;
        this.y = y;
        block[x, y] = this;
        update_status();
        return this;
    }
    public override void delete()
    {
        block[x, y] = null;
        Destroy(gameObject);
        update_status();
    }
    public override void clear_up()
    {
        del = true;
        delete();
    }
}
public class Water : WaterBase
{
    public List<WaterNode> node_list = new List<WaterNode>();
    public WaterNode[,,] w;
    public int[,] value;
    public List<(int, int)> value_max;
    public DynamicSprite sprites;
    public Sprite now_sprite;
    public Color color;
    public int total = 0;
    public int total_upper = 2;
    public bool main_water = false;
    public void start(DynamicSprite sprites = null)
    {
        this.sprites = sprites;
        now_sprite = sprites.get_sprite();
        enabled = true;
        w = new WaterNode[W + 2, H + 2, unit];
        value = new int[W + 2, H + 2];
        value_max = new List<(int, int)>();
    }
    public void game_over()
    {
        foreach (WaterNode i in list_copy()) i.delete();
        enabled = false;
    }
    public void value_add(int x, int y)
    {
        if (++value[x, y] == unit) value_max.Add((x, y));
    }
    public void value_del(int x, int y)
    {
        if (value[x, y]-- == unit) value_max.Remove((x, y));
    }
    public void value_sum(int x, int y)
    {
        int cnt = 0;
        for (int i = 0; i < unit; i++) if (w[x, y, i]) cnt++;
        if (value[x, y] == unit && cnt != unit) value_max.Remove((x, y));
        else if (cnt == unit && value[x, y] != unit) value_max.Add((x, y));
        value[x, y] = cnt;
    }
    public void add(int x, int y, int i)
    {
        if (!w[x, y, i]) node_list.Add(new GameObject("water").AddComponent<WaterNode>().start(this, x, y, i));
    }
    public virtual void add_more(int x, int y, int len)
    {
        (x, y) = find_air(x, y, unit - 1);
        if (x == -1) return;
        add(x, y, unit - 1);
        WaterNode a = w[x, y, unit - 1];
        int i = 1;
        while (i < len && a.add_top()) i++;
        a.sr_update(i - 1);
        a.len = i;
        for (int j = a.ty; j <= a.y; j++) value_sum(a.x, j);
        if (i < len) add_more(x, y, len - i);
    }
    public (int, int) find_air(int x, int y, int n)
    {
        if (!block[x, y] && !w[x, y, n]) return (x, y);
        for (; 0 < y; --y)
        {
            if (!block[x, y - 1] && !w[x, y - 1, n]) return (x, y - 1);
            if (!block[x - 1, y] && !w[x - 1, y, n]) return (x - 1, y);
            if (!block[x + 1, y] && !w[x + 1, y, n]) return (x + 1, y);
        }
        return (-1, -1);
    }
    public void match_color(Color color)
    {
        this.color = color;
        foreach (WaterNode i in node_list) i.sr.color = color;
    }
    public List<WaterNode> list_copy() { return node_list.GetRange(0, node_list.Count); }
    public virtual void node_update()
    {
        foreach (WaterNode n in list_copy()) n.overflow();
        foreach (WaterNode n in list_copy()) n.link();
        foreach (WaterNode n in node_list) n.fall();
        foreach (WaterNode n in list_copy()) n.move();
    }
    public List<WaterTmpBlock> create_block()
    {
        List<WaterTmpBlock> list = new List<WaterTmpBlock>();
        WaterTmpBlock temptale = new GameObject("water block").AddComponent<WaterTmpBlock>();
        foreach ((int x, int y) in value_max)
            if (!block[x, y] && w[x, y, 0]) list.Add(Instantiate(temptale).start(w[x, y, 0], x, y));
        Destroy(temptale.gameObject);
        return list;
    }
    public string package_save()
    {
        string s = "";
        foreach (WaterNode a in node_list)
            s += $"{a.x} {a.y} {a.n} {a.len};";
        return s;
    }
    public virtual void package_load(string data)
    {
        foreach (string s in data.Split(';'))
        {
            if (s.Length < 2) continue;
            string[] a = s.Split(' ');
            int x = int.Parse(a[0]), y = int.Parse(a[1]), l = int.Parse(a[3]);
            add_more(x, y, l);
        }
    }
    void FixedUpdate()
    {
        if (now_sprite != sprites.get_sprite())
        {
            now_sprite = sprites.get_sprite();
            foreach (WaterNode n in node_list)
                n.sr.sprite = now_sprite;
        }
        if (total == total_upper)
        {
            node_update();
            total = 0;
            if (main_water)
            {
                v_update();
                main.water_check();
            }
        }
        else ++total;
    }
}
public class Snow : Water
{
    public override void add_more(int x, int y, int len) { }
    public override void node_update()
    {
        foreach (WaterNode n in list_copy()) n.overflow();
        foreach (WaterNode n in list_copy()) n.link();
        foreach (WaterNode n in node_list) n.fall();
    }
    public override void package_load(string data)
    {
        foreach (string s in data.Split(';'))
        {
            if (s.Length < 2) continue;
            string[] a = s.Split(' ');
            int x = int.Parse(a[0]), y = int.Parse(a[1]), l = int.Parse(a[3]);
            base.add_more(x, y, l);
        }
    }
}