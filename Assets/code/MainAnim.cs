using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Anim
{
    public static float time = 0;
    public delegate float rate(float x);
    public delegate void call(float alpha);
    public static float default_run_time = 0.05f;
    public static rate default_rate = smooth;
    public static float linear(float t)
    {
        return t;
    }
    public static float smooth(float t)
    {
        float s = 1 - t;
        return (t * t * t) * (10 * s * s + 5 * s * t + t * t);
    }
    public static float into(float t)
    {
        return 2 * smooth(0.5f * t);
    }
    public static float from(float t)
    {
        return 2 * smooth(0.5f * (t + 1)) - 1;
    }
    public static float there_and_back(float t)
    {
        float new_t = t < 0.5f ? t : 1 - t;
        return smooth(new_t * 2);
    }
    public static float there_and_back_from(float t)
    {
        return t < 0.05f ? t * 20 : Mathf.Pow((1 - t) / 0.95f, 4);
    }
    public static float slow(float t)
    {
        return Mathf.Sqrt(1 - (1 - t) * (1 - t));
    }
    public static rate reverse(rate func)
    {
        return a => func(1 - a);
    }
}
public class simpleAnim : MonoBehaviour
{
    public float run_time, start_time;
    public Anim.rate func_rate;
    public virtual simpleAnim start()
    {
        start_time = Anim.time;
        run_time = Anim.default_run_time;
        func_rate = Anim.default_rate;
        return this;
    }
    public virtual void interpolation(float alpha) { }
    public virtual void finish()
    {
        interpolation(func_rate(1));
        Destroy(this);
    }
    public simpleAnim set(float run_time, Anim.rate func = null)
    {
        this.run_time = run_time;
        if (func != null) func_rate = func;
        return this;
    }
    public void stop_animation() { Destroy(this); }
    private void Update()
    {
        float alpha = (Anim.time - start_time) / run_time;
        if (alpha >= 1) finish();
        else interpolation(func_rate(alpha));
    }
}
public class blockMove : AnShift
{
    Block block;
    public simpleAnim start(Block b, Vector2 v)
    {
        block = b;
        block.move = this;
        return start(v);
    }
    public override void finish()
    {
        block.move = null;
        base.finish();
    }
}
public class AnShift : simpleAnim
{
    public Vector2 pos, v;
    public simpleAnim start(Vector2 v)
    {
        pos = gameObject.transform.localPosition;
        this.v = v;
        return start();
    }
    public override void interpolation(float alpha)
    {
        gameObject.transform.localPosition = pos + alpha * v;
    }
}
public class d_shift : simpleAnim
{
    public Vector2 v;
    public float last;
    public simpleAnim start(Vector2 v)
    {
        this.v = v;
        return start();
    }
    public override void interpolation(float alpha)
    {
        gameObject.transform.Translate((alpha - last) * v);
        last = alpha;
    }

}
public class FadeIn : simpleAnim
{
    public SpriteRenderer sr;
    public Color color;
    public simpleAnim start(SpriteRenderer sr)
    {
        this.sr = sr;
        color = sr.color;
        interpolation(0);
        return start();
    }
    public override simpleAnim start()
    {
        start_time = Anim.time;
        run_time = 0.5f;
        func_rate = Anim.into;
        return this;
    }
    public override void interpolation(float alpha)
    {
        color.a = alpha;
        sr.color = color;
    }
}
public class FadeOut : FadeIn
{
    public bool remove = true;
    public override void interpolation(float alpha)
    {
        base.interpolation(1 - alpha);
    }
    public override void finish()
    {
        if (remove) Destroy(gameObject);
        else Destroy(this);
    }
}