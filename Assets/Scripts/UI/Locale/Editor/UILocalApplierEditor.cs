using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace QT
{
    [CustomEditor(typeof(UILocalApplier), true)]
    public class UILocalApplierEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var  targetApplier = target as UILocalApplier;

            if (EditorSystemManager.Instance.DataManager.IsInitialized)
            {
                var dataBase = EditorSystemManager.Instance.DataManager.GetDataBase<LocaleGameDataBase>();
                var result = dataBase.GetStrings(targetApplier.StringKey);

                if (result == null)
                {
                    EditorGUILayout.HelpBox("해당 번역키를 찾을 수 없습니다.", MessageType.Error);
                }
                else
                {
                    Rect rect = EditorGUILayout.GetControlRect(false, 1 );
                    rect.height = 30;
                    EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f, 1));
                    
                    EditorGUILayout.BeginHorizontal();
                    
                    GUILayout.Space(10);
                    GUILayout.Label("KR |", EditorStyles.whiteLargeLabel, GUILayout.Width(32), GUILayout.ExpandWidth(false));
                    GUILayout.Label(result[0], EditorStyles.largeLabel, GUILayout.Width(200), GUILayout.ExpandWidth(true));
                    GUILayout.Label("US |", EditorStyles.whiteLargeLabel, GUILayout.Width(32), GUILayout.ExpandWidth(false));
                    GUILayout.Label(result[1], EditorStyles.largeLabel, GUILayout.Width(200), GUILayout.ExpandWidth(true));
                    
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(10);

                    
                    Rect searchRect = EditorGUILayout.GetControlRect(false, 1 );
                    searchRect.height = 30;
                }
                
                
                if (GUILayout.Button(targetApplier.StringKey, EditorStyles.popup))
                {
                    SearchWindow.Open(
                        new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                        new StringListSearchProvider(dataBase.AllDataString, s =>
                        {
                            targetApplier.StringKey = s;
                            if (targetApplier.TryGetComponent<TextMeshProUGUI>(out var a))
                            {
                                a.text = dataBase.GetString(s);
                            }
                        }));
                }
                
                //dataBase.GetStrings()
            }
                
            
            base.OnInspectorGUI();
        }
        
    }
}
