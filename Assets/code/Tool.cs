using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool
{
    private static int hex2dec(char c)
    {
        if (c >= 'A') return (c & 95) - 'A' + 10;
        return c - '0';
    }
    public static Color str2rgb(string s)
    {
        if (s[0] == '#') s = s.Substring(1);
        int r = hex2dec(s[0]) << 4 | hex2dec(s[1]);
        int g = hex2dec(s[2]) << 4 | hex2dec(s[3]);
        int b = hex2dec(s[4]) << 4 | hex2dec(s[5]);
        return new Color(r / 256f, g / 256f, b / 256f);
    }
    public static string array2str(int[] array)
    {
        string s = "";
        foreach (object o in array) s += o.ToString() + " ";
        return s;
    }
    public static string array2str(object[] array)
    {
        string s = "";
        foreach (object o in array) s += o.ToString() + " ";
        return s;
    }
}