using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Background : MonoBehaviour
{
    public Shader sky_shader;
    private Image bg_image, glitch_image, stoped_glitch;
    private Material bg_mat, glitch_mat;
    private List<Color> color_map;
    private int color_length;
    private Coroutine coroutine;
    private bool run_start;
    public bool run_anim;
    private void Start()
    {
        bg_image = GetComponent<Image>();
        glitch_image = transform.Find("../main/Image").GetComponent<Image>();
        bg_mat = new Material(sky_shader);
        glitch_image.material = glitch_mat = new Material(GameBase.sprite.glitch_block_shader);
        glitch_mat.SetFloat("_MaxRGBSplitX", 8);
        glitch_mat.SetFloat("_MaxRGBSplitY", 8);
        stoped_glitch = Instantiate(glitch_image, transform.Find("../play/UL2"));
        stoped_glitch.transform.SetAsFirstSibling();
        color_map = GameBase.status.color_map;
        color_length = color_map.Count / 3 - 1;
        run_start = true;
        set_anim(true);
    }

    public void set_anim(bool run)
    {
        if (!run_start) return;
        run_anim = run;
        if (gameObject.activeInHierarchy)
        {
            bg_image.material = run ? bg_mat : null;
            glitch_image.gameObject.SetActive(run);
            if (run) coroutine = StartCoroutine(glitch());
            else StopCoroutine(coroutine);
        }
    }
    public Image get_stoped_glitch()
    {
        return stoped_glitch;
    }
    IEnumerator glitch()
    {
        float wait = 3f, play = 1f;
        while (true)
        {
            glitch_mat.SetFloat("_Speed", 0);
            glitch_mat.SetFloat("_BlockSize", 0);
            yield return new WaitForSeconds(wait);
            glitch_mat.SetFloat("_Speed", 15);
            glitch_mat.SetFloat("_BlockSize", 30);
            yield return new WaitForSeconds(play);
            glitch_mat.SetFloat("_Speed", 0);
            glitch_mat.SetFloat("_BlockSize", 0);
            yield return new WaitForSeconds(wait);
            glitch_mat.SetFloat("_Speed", 15);
            glitch_mat.SetFloat("_BlockSize", 100);
            yield return new WaitForSeconds(play);
        }
    }

    private void OnEnable()
    {
        set_anim(run_anim);
    }

    private void OnDisable()
    {
        glitch_mat.SetFloat("_Speed", 6);
        glitch_mat.SetFloat("_BlockSize", 30);
    }

    private void Update()
    {
        if (run_anim)
        {
            float a = Time.time % 10 / 10 * color_length, f = a % 1;
            int i = (int)a * 3, j = a < color_length ? i + 3 : i;
            bg_mat.SetColor("top_color", Color.Lerp(color_map[i], color_map[j], f));
            bg_mat.SetColor("mid_color", Color.Lerp(color_map[i + 1], color_map[j + 1], f));
            bg_mat.SetColor("btm_color", Color.Lerp(color_map[i + 2], color_map[j + 2], f));
        }
    }
}
