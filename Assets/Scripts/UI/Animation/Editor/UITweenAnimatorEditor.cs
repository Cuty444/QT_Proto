using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using DG.DOTweenEditor;
using UnityEngine;
using UnityEditor;

namespace QT
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UITweenAnimator), true)]
    public class UITweenAnimatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();

            bool isPlaying = EditorApplication.isPlaying;
            
            
            if (GUILayout.Button("재생"))
            {
                if (!isPlaying)
                {
                    //DOTweenEditorPreview.Stop(true);
                    (target as UITweenAnimator).BakeSeqence();
                    
                     var sequence = (Sequence)typeof(UITweenAnimator).GetField("_sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default)?.GetValue(target);
                    DOTweenEditorPreview.PrepareTweenForPreview(sequence, true, false);
                    DOTweenEditorPreview.Start();
                }
                else
                {
                    (target as UITweenAnimator).ReStart();
                }
            }
            if (GUILayout.Button(isPlaying ? "재시작" : "처음처럼"))
            {
                if (!isPlaying)
                {
                    DOTweenEditorPreview.Stop(true);
                }
                else
                {
                    (target as UITweenAnimator).ReStart();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            
            base.OnInspectorGUI();
        }
    }
}