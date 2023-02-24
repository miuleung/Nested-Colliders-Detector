#if UNITY_EDITOR
using UnityEngine;
using System.Reflection;
using System;
using UnityEditor;

public class ConsoleCleaner : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearConsole();
        }
    }

    static MethodInfo clearMethod = null;
    private static void ClearConsole()
    {
        if (clearMethod == null)
        {
            Type log = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries");
            clearMethod = log.GetMethod("Clear");
        }
        clearMethod.Invoke(null, null);
    }
}
#endif
