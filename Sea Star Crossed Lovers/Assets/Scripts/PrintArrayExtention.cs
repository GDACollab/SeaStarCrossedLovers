using System.Collections.Generic;

public static class PrintExtentions
{
    public static string ToFString(this string[] array)
    {
        string result = string.Empty;
        int size = array.Length;
        if (size == 0) return result;

        for (int i = 0; i < size; i++)
        {
            result += "\"" + array[i] + "\"";
            if (i + 1 != size) result += ", ";
        }
        return result;
    }

    public static string ToFString(this List<string> list)
    {
        string result = string.Empty;
        int size = list.Count;
        if (size == 0) return result;

        for (int i = 0; i < size; i++)
        {
            result += "\"" + list[i] + "\"";
            if (i + 1 != size) result += ", ";
        }
        return result;
    }
}
