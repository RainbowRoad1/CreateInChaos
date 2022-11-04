using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Profiling;

public class StatInfo : MonoBehaviour
{
    private Text text;
    private int count;
    private float total;

    ProfilerRecorder totalReservedMemoryRecorder;
    ProfilerRecorder gcReservedMemoryRecorder;
    ProfilerRecorder systemUsedMemoryRecorder;

    void Start()
    {
        text = GetComponent<Text>();
        totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
        gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        systemUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
    }

    void Update()
    {
        count++;
        total += Time.deltaTime;
        if (total >= 0.5f)
        {
            long turn_size = 1048576;
            var sb = new System.Text.StringBuilder(500);
            sb.AppendLine($"FPS: {Mathf.Ceil(count / total)}");
            sb.AppendLine($"Block count: {Block.parent.childCount}");
            sb.AppendLine($"Water count: {GameBase.water.node_list.Count + GameBase.lava.node_list.Count}");
            GameUI sta = GameBase.status;
            sb.AppendLine(sta.w_power == 0 ? $"next weather: {sta.w_next}" : $"weather power: {sta.w_power}");
            if (totalReservedMemoryRecorder.Valid)
                sb.AppendLine($"Total Reserved Memory: {totalReservedMemoryRecorder.LastValue / turn_size}MB");
            if (gcReservedMemoryRecorder.Valid)
                sb.AppendLine($"GC Reserved Memory: {gcReservedMemoryRecorder.LastValue / turn_size}MB");
            if (systemUsedMemoryRecorder.Valid)
                sb.AppendLine($"System Used Memory: {systemUsedMemoryRecorder.LastValue / turn_size}MB");
            text.text = sb.ToString();
            count = 0;
            total = 0;
        }
    }
}
