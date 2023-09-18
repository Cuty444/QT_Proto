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
    [CustomEditor(typeof(TweenAnimator), true)]
    public class TweenAnimatorEditor : Editor
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
                    (target as TweenAnimator).BakeSeqence();
                    
                     var sequence = (Sequence)typeof(TweenAnimator).GetField("_sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default)?.GetValue(target);
                    DOTweenEditorPreview.PrepareTweenForPreview(sequence, true, false);
                    DOTweenEditorPreview.Start();
                }
                else
                {
                    (target as TweenAnimator).ReStart();
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
                    (target as TweenAnimator).ReStart();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            
            base.OnInspectorGUI();
        }
    }
}