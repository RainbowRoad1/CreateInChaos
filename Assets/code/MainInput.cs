using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInput : GameBase
{
    static private Dictionary<input_key, MonitorKey> key_map;
    private Dictionary<input_key, bool> input_buf;
    void Start()
    {
        key_map = new Dictionary<input_key, MonitorKey>();
        input_buf = new Dictionary<input_key, bool>();
        MainConfig config = MainConfig.main;
        key_map[input_key.left] = new MonitorKey((KeyCode)System.Enum.Parse(typeof(KeyCode), config.get_data("key1")), config.get_data("key1s")[0] == '1');
        key_map[input_key.right] = new MonitorKey((KeyCode)System.Enum.Parse(typeof(KeyCode), config.get_data("key2")), config.get_data("key2s")[0] == '1');
        key_map[input_key.down] = new MonitorKey((KeyCode)System.Enum.Parse(typeof(KeyCode), config.get_data("key3")), config.get_data("key3s")[0] == '1');
        key_map[input_key.up] = new MonitorKey((KeyCode)System.Enum.Parse(typeof(KeyCode), config.get_data("key4")), config.get_data("key4s")[0] == '1');
        key_map[input_key.funA] = new MonitorKey((KeyCode)System.Enum.Parse(typeof(KeyCode), config.get_data("key5")), config.get_data("key5s")[0] == '1');
        key_map[input_key.funB] = new MonitorKey((KeyCode)System.Enum.Parse(typeof(KeyCode), config.get_data("key6")), config.get_data("key6s")[0] == '1');
    }
    static public MonitorKey get_key(input_key key_name)
    {
        return key_map[key_name];
    }
    void run_keys()
    {
        foreach (input_key i in key_map.Keys)
        {
            input_buf[i] = key_map[i].get_key();
        }
    }
    void Update()
    {
        run_keys();
        main.input_func(input_buf);
    }
}
public class MonitorKey
{
    static public float start_cd = 0.1f;
    public KeyCode key;
    public float cd;
    public bool mode;
    private bool is_press, is_stop;
    public MonitorKey(KeyCode key, bool mode = false)
    {
        this.key = key;
        this.mode = mode;
    }
    public void press()
    {
        is_press = true;
    }
    public bool get_key()
    {
        return mode ? get_key_func2() : get_key_func1();
    }
    public bool get_key_func1()
    {
        if (cd > 0)
        {
            cd -= Time.deltaTime;
            is_press = false;
            return false;
        }
        if (Input.GetKey(key)) is_press = true;
        if (!is_press) return false;
        is_press = false;
        cd = start_cd;
        return true;
    }
    public bool get_key_func2()
    {
        if (Input.GetKey(key)) is_press = true;
        if (is_press)
        {
            if (!is_stop)
            {
                is_stop = true;
                return true;
            }
        }
        else is_stop = false;
        is_press = false;
        return false;
    }
}