using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Utils {
    public static UnityObjectType ToUnityObjectEnum(string type)
    {
        if (type == "GameObject")
            return UnityObjectType.GameObject;

        if (type == "Transform")
            return UnityObjectType.Transform;

        if (type == "MonoBehaviour")
            return UnityObjectType.MonoBehaviour;

        return UnityObjectType.Other;
    }

    public static string GetVariableName(string variable)
    {
        string trimmedVariable = variable.Trim();
        
        if (trimmedVariable.IndexOf(':') >= 0)
            return trimmedVariable.Substring(0, trimmedVariable.IndexOf(':'));

        return "";
    }

    public static string GetVariableValue(string variable)
    {
        string trimmedVariable = variable.Trim();

        if (trimmedVariable.IndexOf(':') >= 0 && trimmedVariable.IndexOf(':') + 2 < trimmedVariable.Length)
            return trimmedVariable.Substring(trimmedVariable.IndexOf(':') + 2);

        return "";
    }

    public static string ParseValueByKey(string text, string key) {
        int stringStart = text.IndexOf(key) + key.Length + 1;

        int[] endingCandidates = { text.Substring(stringStart).IndexOf(','), text.Substring(stringStart).IndexOf('}') };
        endingCandidates = endingCandidates.Where(item => item >= 0).ToArray();
        int stringLength = endingCandidates.Min();

        return text.Substring(stringStart, stringLength);
    }

    public static string ParseFileId(string text) {
        return ParseValueByKey(text, "fileID:");
    }

    public static string ParseGuid(string text)
    {
        return ParseValueByKey(text, "guid:");
    }
}