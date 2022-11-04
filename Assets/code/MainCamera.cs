using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Shader GlitchRGB_shader;
    private Material glitch_mat, mat;
    private int type = 0;
    private float start_time, run_time;
    void Start()
    {
        GameBase.main_camera = this;
        mat = glitch_mat = new Material(GlitchRGB_shader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (type == 0) Graphics.Blit(source, destination);
        else Graphics.Blit(source, destination, mat);
    }

    public void glitch()
    {
        start_time = Anim.time;
        run_time = 1;
        type = 1;
    }

    public void shift_down()
    {
        StartCoroutine(shift_down_run());
    }

    IEnumerator shift_down_run()
    {
        float d = 0.5f - (transform.position.y + Camera.main.orthographicSize) / 2;
        transform.Translate(Vector2.up * d);
        for (int i = 0; i < 10; i++)
        {
            transform.Translate(Vector2.down * (d / 10));
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Update()
    {
        if (type == 0) return;
        float alpha = (Anim.time - start_time) / run_time;
        if (alpha >= 1) type = 0;
        else if (type == 1)
        {
            glitch_mat.SetFloat("_alpha", Anim.there_and_back_from(alpha));
        }
    }
}