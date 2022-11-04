using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : GameBase
{
    public ParticleSystem snowy, rainy;
    ParticleSystem.Particle[] mp;
    private ParticleSystem ps;
    public int type;
    private int basic_power;
    private float start_time, diff_time;
    void Start()
    {
        weather = this;
        mp = new ParticleSystem.Particle[snowy.main.maxParticles];
    }
    public void start()
    {
        enabled = true;
        gameObject.SetActive(true);
        var shape = rainy.shape;
        shape.scale = new Vector3(W, 0, 0);
        shape = snowy.shape;
        shape.scale = new Vector3(W, 0, 0);
        transform.position = new Vector3(0.5f + W / 2f, -0.5f, 0);
    }
    public void game_over()
    {
        if (ps) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        gameObject.SetActive(false);
        type = 0;
    }
    public void play(int mode = 1)
    {
        if (type > 0) return;
        type = mode + 1;
        ps = type == 1 ? rainy : snowy;
        basic_power = (type == 1 ? 5 : 1) * W;
        diff_time = type == 1 ? 1 : 2;
        start_time = Anim.time;
        ps.gameObject.AddComponent<EmissionRateTransition>().start(ps, basic_power);
    }
    public void size_update(float power)
    {
        ps.gameObject.AddComponent<EmissionRateTransition>().start(ps, basic_power * power);
        diff_time *= 0.75f;
    }
    public void sleep()
    {
        if (type == 0) return;
        ps.gameObject.AddComponent<EmissionRateTransition>().start(ps, 0);
        type = 0;
    }
    private void particle_side(ParticleSystem ps)
    {
        int[] top = BlockBase.block_top;
        int num = ps.GetParticles(mp);
        bool has_update = false;
        for (int i = 0; i < num; i++)
        {
            Vector3 p = mp[i].position;
            if (p.y < -top[(int)(p.x + 0.5f)] - 0.5f)
            {
                mp[i].remainingLifetime = -1;
                has_update = true;
            }
        }
        if (has_update) ps.SetParticles(mp, num);
    }
    public string package_save()
    {
        return type == 0 ? "" : $"{type};{ps.time}";
    }
    public void package_load(string data)
    {
        string[] s = data.Split(';');
        type = int.Parse(s[0]);
        ps = type == 1 ? rainy : snowy;
        basic_power = (type == 1 ? 5 : 1) * W;
        diff_time = type == 1 ? 1 : 2;
        start_time = Anim.time;
        ps.Play();
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = basic_power;
        ps.time = float.Parse(s[1]);
    }
    void Update()
    {
        if (snowy.particleCount > 0) particle_side(snowy);
        if (rainy.particleCount > 0) particle_side(rainy);
        if (type == 0) return;
        int[] top = BlockBase.block_top;
        while (start_time + diff_time < Anim.time)
        {
            start_time += diff_time;
            if (type == 1)
            {
                water.add(Random.Range(0, W) + 1, 1, 0);
            }
            else if (type == 2)
            {
                int x = Random.Range(0, W) + 1;
                snow.add(x, 1, 0);
                snow.add(x, 1, 1);
                for (int i = 1; i <= W; i++)
                {
                    WaterNode n = water.w[i, top[i], WaterBase.unit - 1];
                    if (n)
                    {
                        if (n.ty == n.y) n.delete();
                        else n.len -= WaterBase.unit - n.tn;
                        BlockData.name2block(block_name.ice).start(i, n.ty);
                    }
                }
            }
        }
    }
}

public class EmissionRateTransition : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;
    private float start_time, diff_time;
    private float start_value, end_value;
    public void start(ParticleSystem ps, float value)
    {
        this.ps = ps;
        emission = ps.emission;
        start_time = Time.time;
        diff_time = 5f;
        start_value = emission.rateOverTime.constant;
        end_value = value;
        if (end_value > 0) ps.Play();
    }
    private void Update()
    {
        float alpha = ((int)Time.time - start_time) / diff_time;
        if (alpha >= 1)
        {
            emission.rateOverTime = end_value;
            if (end_value == 0) ps.Stop();
            Destroy(this);
        }
        else emission.rateOverTime = Mathf.Lerp(start_value, end_value, alpha);
    }
}
