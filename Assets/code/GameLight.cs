using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLight : GameBase
{
    public int[,] value;
    public int light_upper = 5;
    public float light_rate = 0.8f;
    private float[] level;
    private List<Block> source = new List<Block>();
    private Hashtable place = new Hashtable();
    private HashSet<(int, int)> lava_source = new HashSet<(int, int)>();

    public Shader light_shader;
    private SpriteRenderer sr;
    private Material m_mat;
    private Texture2D m_tex;
    private Color[] m_buf;
    private bool need_update;

    private void Start()
    {
        lights = this;
        m_tex = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        m_tex.filterMode = FilterMode.Point;
        m_mat = new Material(light_shader);
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.material = m_mat;
        sr.sortingOrder = 5;
    }

    public void start()
    {
        value = new int[W + 2, H + 2];
        m_tex.Resize(W, H);
        m_buf = new Color[W * H];
        for (int i = W * H - 1; 0 <= i; --i) m_buf[i] = new Color(0, 0, 0, 1);
        level = new float[light_upper + 1];
        for (int i = 0; i <= light_upper; i++)
            level[light_upper - i] = (float)i / light_upper;
        transform.position = new Vector2((W + 1) * 0.5f, -(H + 1) * 0.5f);
        m_tex.SetPixels(m_buf);
        m_tex.Apply();
        update_global_light(0);
        sr.sprite = Sprite.Create(m_tex, new Rect(0, 0, W, H), new Vector2(0.5f, 0.5f), 1);
    }
    public void update_global_light(float rate)
    {
        light_rate = rate;
        m_mat.SetFloat("buff", rate);
    }
    public void game_over()
    {
        lava_source.Clear();
    }
    public bool add_light_source(Block block)
    {
        if (place.ContainsKey(block)) return false;
        place.Add(block, (block.x, block.y));
        source.Add(block);
        return true;
    }
    public void del_light_source(Block block)
    {
        if (place.ContainsKey(block))
        {
            place.Remove(block);
            source.Remove(block);
            area_del(block.x, block.y, block.light_i);
        }
    }
    public void area_del(int x, int y, int len)
    {
        int Bx = Mathf.Max(x - len, 1), By = Mathf.Max(y - len, 1);
        int Ex = Mathf.Min(x + len, W), Ey = Mathf.Min(y + len, H);
        for (int i = Bx; i <= Ex; i++)
            for (int j = By; j <= Ey; j++)
            {
                value[i, j] = 0;
                m_buf[(H - j) * W + (i - 1)].a = level[0];
            }
        need_update = true;
    }
    public void area_add(int x, int y, int len)
    {
        if (value[x, y] >= len) return;
        value[x, y] = len;
        int Bx = Mathf.Max(x - len + 1, 1), By = Mathf.Max(y - len - 1, 1);
        int Ex = Mathf.Min(x + len + 1, W), Ey = Mathf.Min(y + len - 1, H);
        for (int i = Bx; i <= Ex; i++)
            for (int j = By; j <= Ey; j++)
                value[i, j] = Mathf.Max(value[i - 1, j], value[i, j - 1], value[i, j] + 1) - 1;
        for (int i = Ex; Bx <= i; i--)
            for (int j = Ey; By <= j; j--)
                value[i, j] = Mathf.Max(value[i + 1, j], value[i, j + 1], value[i, j] + 1) - 1;
        for (int i = Bx; i <= Ex; i++)
            for (int j = By; j <= Ey; j++)
                m_buf[(H - j) * W + (i - 1)].a = level[value[i, j]];
        need_update = true;
    }
    public void update_light()
    {
        need_update = false;
        foreach (WaterNode node in lava.node_list)
            for (int i = node.ty; i <= node.y; i++) lava_source.Add((node.x, i));
        List<(int, int)> tmp_del = new List<(int, int)>();
        foreach ((int x, int y) in lava_source)
            if (lava.value[x, y] == 0)
            {
                area_del(x, y, 5);
                tmp_del.Add((x, y));
            }
        foreach ((int, int) p in tmp_del) lava_source.Remove(p);
        foreach (Block b in source)
        {
            (int x, int y) = ((int, int))place[b];
            if (b.light_i == 0 && value[x, y] > 0) area_del(x, y, value[x, y]);
            else if (!(b.x == x && b.y == y))
            {
                area_del(x, y, b.light_i);
                place[b] = (b.x, b.y);
            }
        }
        foreach (Block b in source) area_add(b.x, b.y, b.light_i);
        foreach ((int x, int y) in lava_source) area_add(x, y, 5);
        if (Random.Range(0, 1f) < 0.02f)
        {
            foreach ((int ox, int oy) in lava_source)
                foreach ((int dx, int dy) in OUTSIDE_4)
                {
                    Block b = block[ox + dx, oy + dy];
                    if (b && b.canFire) b.catch_fire();
                }
        }
        if (need_update)
        {
            m_tex.SetPixels(m_buf);
            m_tex.Apply();
        }
    }
}