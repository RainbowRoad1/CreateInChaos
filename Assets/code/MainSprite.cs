using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSprite : MonoBehaviour
{
    public Texture2D[] blocks_tex, blocks_add_tex;
    public Texture2D water_tex, lava_tex, fire_tex;
    public Sprite white;
    [HideInInspector]
    public Sprite[] blocks, blocks_add;
    public DynamicSprite water, lava, fire, snow;
    public Shader boom_shader, ghost_shader, glitch_block_shader;
    [HideInInspector]
    public Coroutine update_dyna;

    void Awake()
    {
        GameBase.sprite = this;
        water = new DynamicSprite(water_tex, 2);
        lava = new DynamicSprite(lava_tex, 8);
        fire = new DynamicSprite(fire_tex, 4);
        snow = new DynamicSprite(white.texture, 16);
        set_dyna_active();
    }
    public void set_dyna_active()
    {
        if (update_dyna != null)
        {
            StopCoroutine(update_dyna);
            update_dyna = null;
        }
        else update_dyna = StartCoroutine(update());
    }
    IEnumerator update()
    {
        while (true)
        {
            water.update();
            lava.update();
            fire.update();
            yield return new WaitForSeconds(0.02f);
        }
    }
}
public class DynamicSprite
{
    public Sprite[] sprites;
    public int size, count, index, speed, tot;
    public DynamicSprite(Texture2D tex, int speed)
    {
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        this.speed = speed;
        size = tex.width;
        count = tex.height / size;
        sprites = new Sprite[count];
        for (int i = 0; i < count; ++i)
            sprites[count - 1 - i] = Sprite.Create(
                tex, new Rect(0, i * size, size, size),
                pivot, size, 0, SpriteMeshType.FullRect
            );
    }
    public Sprite get_sprite()
    {
        return sprites[index];
    }
    public void update()
    {
        if (++tot < speed) return;
        index = (index + 1) % count;
        tot = 0;
    }
}