using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudio : GameBase
{
    public float volume = 1, effect = 1;
    private AudioSource bgm_source, effect_source;
    public AudioClip[] bgm_audio, lay_audio, run_audio, opt_audio;
    public enum lay { cloth, grass, gravel, lava, sand, snow, stone, water, wood }
    public enum run { click, explode, fire, fizz, fuse, glass, piston_in, piston_out }
    public enum opt { move, turn, fall1, fall2, fall3, clean1, clean2 }
    void Awake()
    {
        sounds = this;
        bgm_source = gameObject.AddComponent<AudioSource>();
        effect_source = gameObject.AddComponent<AudioSource>();
    }
    public void set_bgm_volume(float f)
    {
        bgm_source.volume = volume = f;
    }

    public void update_bgm()
    {
        if (bgm_audio.Length == 0) return;
        if (!bgm_source.isPlaying)
        {
            bgm_source.volume = volume;
            bgm_source.clip = bgm_audio[Random.Range(0, bgm_audio.Length)];
            bgm_source.Play();
        }
    }
    public void use_clip(AudioClip clip)
    {
        effect_source.PlayOneShot(clip, effect);
    }
    public void lay_clip(int i)
    {
        use_clip(lay_audio[Random.Range(i * 4, i * 4 + 4)]);
    }
    public void run_clip(run i)
    {
        use_clip(run_audio[i switch
        {
            run.click => 0,
            run.explode => Random.Range(1, 5),
            run.fire => 5,
            run.fizz => 6,
            run.fuse => 7,
            run.glass => Random.Range(8, 11),
            run.piston_in => 11,
            run.piston_out => 12,
            _ => 0,
        }]);
    }

    public void opt_clip(opt i)
    {
        use_clip(opt_audio[i switch
        {
            opt.move => 4,
            opt.turn => 0,
            opt.fall1 => 1,
            opt.fall2 => 2,
            opt.fall3 => 3,
            opt.clean1 => 5,
            opt.clean2 => 6,
            _ => 0,
        }]);
    }
}
