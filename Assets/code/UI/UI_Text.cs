using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManageSystem
{
    public Dictionary<string, string> text_map;
    private Dictionary<string, string> eng, cn;
    public void choose_language(int i)
    {
        text_map = i == 0 ? eng : cn;
    }
    private Dictionary<string, string> load_text(string path)
    {
        Dictionary<string, string> tmp_map = new Dictionary<string, string>();
        string load = Resources.Load<TextAsset>(path).ToString();
        string[] list = load.Split('\n');
        foreach (string s in list)
        {
            string[] i = s.Split('\\');
            if (i.Length < 2) continue;
            tmp_map[i[0]] = i[1];
        }
        return tmp_map;
    }
    public TextManageSystem()
    {
        UI_Text.manage = this;
        eng = load_text("Text/english");
        cn = load_text("Text/chinese");
        choose_language(0);
    }
}
public class UI_Text : MonoBehaviour
{
    public static TextManageSystem manage;
    public string text_name;
    private Text text;
    void Start()
    {
        if (manage == null) new TextManageSystem();
        reset_text();
    }
    public void reset_text()
    {
        if (text == null) text = transform.Find("Text").GetComponent<Text>();
        if (manage.text_map.ContainsKey(text_name)) text.text = manage.text_map[text_name];
        else text.text = "%%" + text_name;
    }
}
