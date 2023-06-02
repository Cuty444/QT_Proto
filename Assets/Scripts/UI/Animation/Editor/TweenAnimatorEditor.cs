using System.Collections;
using System.Collections.Generic;
using DG.DOTweenEditor;
using UnityEngine;
using UnityEditor;

namespace QT
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TweenAnimator), true)]
    public class TweenAnimatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();

            bool isPlaying = EditorApplication.isPlaying;
            
            if (GUILayout.Button("���"))
            {
                if (!isPlaying)
                {
                    DOTweenEditorPreview.PrepareTweenForPreview((target as TweenAnimator).Sequence, true, false);
                    DOTweenEditorPreview.Start();
                }
                else
                {
                    (target as TweenAnimator).ReStart();
                }
            }
            if (GUILayout.Button(isPlaying ? "�����" : "ó��ó��"))
            {
                if (!isPlaying)
                {
                    DOTweenEditorPreview.Stop(true, true);
                }
                else
                {
                    (target as TweenAnimator).ReStart();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            
            base.OnInspectorGUI();
        }
    }
}