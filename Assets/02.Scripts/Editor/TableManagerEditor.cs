using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TableManager))]
[CanEditMultipleObjects]
public class TableManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 대상 가져오기
        TableManager my = (TableManager)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        // 버튼 추가
        if(GUILayout.Button("데이터 불러오기"))
        {
            my.LoadTables();
        }
        if(GUILayout.Button("데이터 초기화"))
        {
            my.ClearTables();
        }
    }
}
