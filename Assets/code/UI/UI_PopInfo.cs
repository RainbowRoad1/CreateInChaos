using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PopInfo : MonoBehaviour
{
    static public Queue<UI_PopInfo> list = new Queue<UI_PopInfo>();
    static private float shift_x = 500, shift_y = 300;
    static private float run_fade_in = 0.3f;
    static private int run_frame_number = 20;
    static private float wait_time = 4f;
    private Image image;
    private Text text;
    private Vector2 pos;
    public string info;
    void Start()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
        text.text = info;
        transform.localPosition = new Vector2(shift_x, list.Count * -300);
        pos = transform.localPosition;
        list.Enqueue(this);
        StartCoroutine(run());
    }
    IEnumerator run()
    {
        Color image_c = image.color, text_c = text.color;
        float wait = run_fade_in / run_frame_number;
        for (int i = 0; i <= run_frame_number; i++)
        {
            float f = (float)i / run_frame_number;
            image_c.a = f * 0.5f;
            image.color = image_c;
            text_c.a = f;
            text.color = text_c;
            pos.x = shift_x * Anim.into(1 - f);
            transform.localPosition = pos;
            yield return new WaitForSeconds(wait);
        }
        yield return new WaitForSeconds(wait_time);
        for (int i = run_frame_number; 0 <= i; i--)
        {
            float f = (float)i / run_frame_number;
            image_c.a = f * 0.5f;
            image.color = image_c;
            text_c.a = f;
            text.color = text_c;
            yield return new WaitForSeconds(wait);
        }
        delete();
    }
    static public void delete()
    {
        Destroy(list.Dequeue().gameObject);
        foreach (var item in list) item.StartCoroutine(item.shift());
    }
    public IEnumerator shift()
    {
        float wait = run_fade_in / run_frame_number;
        float last = 0, a = 0;
        while (true)
        {
            a += wait;
            float f = Anim.smooth(a);
            pos.y += shift_y * (f - last);
            if (a >= 1) break;
            transform.localPosition = pos;
            last = f;
            yield return new WaitForSeconds(wait);
        }
    }
}
