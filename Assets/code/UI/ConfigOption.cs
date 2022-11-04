using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MainConfig
{
    static public MainConfig main;
    private Dictionary<string, string> text_map;
    private string file_path;
    public bool need_update;
    public void start()
    {
        main = C_OPT.main_config = this;
        text_map = new Dictionary<string, string>();
        file_path = Application.persistentDataPath + "\\config.txt";
        if (File.Exists(file_path))
        {
            string[] file_data = File.ReadAllLines(file_path);
            if (file_data.Length >= 27)
            {
                foreach (string line in file_data)
                {
                    string[] s = line.Split('=');
                    if (s.Length < 2) continue;
                    text_map[s[0]] = s[1];
                }
                return;
            }
        }
        init_config_basic();
        init_config_gameplay();
        init_config_input();
    }
    public void init_config_basic()
    {
        text_map["sound volume"] = "50";
        text_map["BGM volume"] = "50";
        text_map["frame rate"] = "1";
        text_map["fill screen"] = "0";
        text_map["language"] = "0";
    }
    public void init_config_gameplay()
    {
        text_map["debug info"] = "0";
        text_map["vir key"] = "1";
        text_map["key size"] = "20";
        text_map["key opacity"] = "80";
        text_map["menu bg"] = "1";
        text_map["use dyna"] = "1";
        text_map["use sky"] = "1";
        text_map["use glitch"] = "0";
    }
    public void init_config_input()
    {
        text_map["key1"] = "A";
        text_map["key2"] = "D";
        text_map["key3"] = "S";
        text_map["key4"] = "Space";
        text_map["key5"] = "Q";
        text_map["key6"] = "W";
        text_map["key1s"] = "0";
        text_map["key2s"] = "0";
        text_map["key3s"] = "0";
        text_map["key4s"] = "1";
        text_map["key5s"] = "0";
        text_map["key6s"] = "0";
        text_map["key delay"] = "0.1";
        text_map["key mode"] = "1";
    }
    public void set_data(string key, string data, bool update = true)
    {
        text_map[key] = data;
        if (update) need_update = true;
    }
    public string get_data(string key)
    {
        return text_map[key];
    }
    public void save_config()
    {
        if (!need_update) return;
        need_update = false;
        var sb = new System.Text.StringBuilder(500);
        foreach (string i in text_map.Keys) sb.Append(i + "=" + text_map[i] + "\n");
        File.WriteAllText(file_path, sb.ToString());
    }
}
public class ConfigOption : MonoBehaviour
{
    public enum opt_type { Toggle, Slider, DropDown, Button };
    public opt_type opt_t;
    public string opt_name;
    private C_OPT opt;
    private void Start()
    {
        opt = opt_t switch
        {
            opt_type.Toggle => new C_OPT_Toggle(),
            opt_type.Slider => new C_OPT_Slider(),
            opt_type.DropDown => new C_OPT_DropDown(),
            opt_type.Button => new C_OPT_Button(),
            _ => null,
        };
        opt.start(this);
    }
    public void reset() { opt.reset(); }
}
internal abstract class C_OPT
{
    static public MainConfig main_config;
    public string opt_name;
    abstract public void start(ConfigOption root);
    abstract public void reset();
}
internal class C_OPT_Toggle : C_OPT
{
    Toggle toggle;
    override public void start(ConfigOption root)
    {
        opt_name = root.opt_name;
        toggle = root.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(v => main_config.set_data(opt_name, v ? "1" : "0"));
        bool need_update = main_config.need_update;
        reset();
        main_config.need_update = need_update;
    }
    public override void reset()
    {
        toggle.isOn = main_config.get_data(opt_name)[0] == '1';
    }
}
internal class C_OPT_Slider : C_OPT
{
    Slider silder;
    override public void start(ConfigOption root)
    {
        opt_name = root.opt_name;
        silder = root.GetComponent<Slider>();
        silder.onValueChanged.AddListener(v => main_config.set_data(opt_name, v.ToString()));
        bool need_update = main_config.need_update;
        reset();
        main_config.need_update = need_update;
    }
    public override void reset()
    {
        silder.value = float.Parse(main_config.get_data(opt_name));
    }
}
internal class C_OPT_DropDown : C_OPT
{
    Dropdown dropdown;
    public override void start(ConfigOption root)
    {
        opt_name = root.opt_name;
        dropdown = root.GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(v => main_config.set_data(opt_name, v.ToString()));
        bool need_update = main_config.need_update;
        reset();
        main_config.need_update = need_update;
    }
    public override void reset()
    {
        dropdown.value = int.Parse(main_config.get_data(opt_name));
    }
}
internal class C_OPT_Button : C_OPT
{
    Button button;
    public override void start(ConfigOption root)
    {
        opt_name = root.opt_name;
        button = root.GetComponent<Button>();
        if (root.opt_name.Equals("")) button.onClick.AddListener(() => main_config.save_config());
        else
        {
            if (root.opt_name.Equals("init basic")) button.onClick.AddListener(() => main_config.init_config_basic());
            else if (root.opt_name.Equals("init gameplay")) button.onClick.AddListener(() => main_config.init_config_gameplay());
            else if (root.opt_name.Equals("init input"))
            {
                button.onClick.AddListener(() => main_config.init_config_input());
                button.onClick.AddListener(() =>
                {
                    foreach (var i in button.transform.parent.GetComponentsInChildren<UI_SetKey>())
                    {
                        var keycode = MainConfig.main.get_data($"key{(int)i.key + 1}");
                        i.GetComponentInChildren<Text>().text = keycode.ToString();
                        i.m_key.key = (KeyCode)System.Enum.Parse(typeof(KeyCode), keycode);
                    }
                });
            }
            button.onClick.AddListener(() =>
            {
                foreach (var i in button.transform.parent.GetComponentsInChildren<ConfigOption>()) i.reset();
                main_config.need_update = true;
            });
        }
    }
    public override void reset() { }
}