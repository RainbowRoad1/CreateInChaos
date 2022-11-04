using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Record : MonoBehaviour
{
    private Text[] text;
    private UI_Text[] history_name;
    private RecordFile record;
    private int last_update;
    void Awake()
    {
        text = new Text[23];
        history_name = new UI_Text[3];
        record = RecordFile.main;
        last_update = -1;
        for (int i = 0; i < 4; i++)
        {
            Transform t = transform.Find($"mode{i + 1}");
            text[i * 3] = t.Find("score").GetComponent<Text>();
            text[i * 3 + 1] = t.Find("time").GetComponent<Text>();
            text[i * 3 + 2] = t.Find("count").GetComponent<Text>();
        }
        text[12] = transform.Find("title/time value").GetComponent<Text>();
        text[13] = transform.Find("title/count value").GetComponent<Text>();
        for (int i = 0; i < 3; i++)
        {
            Transform t = transform.Find($"save{i + 1}");
            history_name[i] = t.GetComponent<UI_Text>();
            text[14 + i * 3] = t.Find("score").GetComponent<Text>();
            text[15 + i * 3] = t.Find("time").GetComponent<Text>();
            text[16 + i * 3] = t.Find("date").GetComponent<Text>();
        }
    }
    private void OnEnable()
    {
        if (last_update == record.update_count) return;
        last_update = record.update_count;
        int[] data = record.top_data;
        for (int i = 0; i < 12; i += 3)
        {
            text[i].text = data[i].ToString();
            text[i + 2].text = data[i + 2].ToString();
            int a = data[i + 1], b = a / 60;
            text[i + 1].text = $"{b / 24}day {b % 24}h {a % 60}min";
        }
        int n = record.game_time, m = n / 60;
        text[12].text = $"{m / 60}h {m % 60}min {n % 60}s";
        text[13].text = record.game_count.ToString();
        string[] history_data = record.history_data;
        for (int i = 0; i < 3; i++)
        {
            history_name[i].text_name = "game_mode" + history_data[i * 4];
            history_name[i].reset_text();
            text[14 + i * 3].text = history_data[i * 4 + 1];
            int a = int.Parse(history_data[i * 4 + 2]), b = a / 60;
            text[15 + i * 3].text = $"{b / 24}day {b % 24}h {a % 60}min";
            text[16 + i * 3].text = history_data[i * 4 + 3];
        }
    }
}
