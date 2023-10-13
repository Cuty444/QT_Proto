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
            var isPlaying = EditorApplication.isPlaying;
            var targetAnimator = target as TweenAnimator;
            
            
            var check = targetAnimator.ChainAnimator;
            while (check != null)
            {
                if(check == targetAnimator)
                {
                    EditorGUILayout.HelpBox("순환참조가 발생했습니다.", MessageType.Error);
                    base.OnInspectorGUI();
                    return;
                }
                check = check.ChainAnimator;
            }
            
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("재생"))
            {
                if (!isPlaying)
                {
                    //DOTweenEditorPreview.Stop(true);
                    targetAnimator.BakeSequence(true);

                    do
                    {
                        var sequence = (Sequence)typeof(TweenAnimator).GetField("_sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default)?.GetValue(targetAnimator);
                        DOTweenEditorPreview.PrepareTweenForPreview(sequence, true, false);
                        DOTweenEditorPreview.Start();
                        
                        targetAnimator = targetAnimator.ChainAnimator;
                    } while (targetAnimator != null);
                    
                }
                else
                {
                    targetAnimator.ReStart();
                }
            }
            // if (GUILayout.Button("역 재생"))
            // {
            //     if (!isPlaying)
            //     {
            //         //DOTweenEditorPreview.Stop(true);
            //         targetAnimator.BakeSequence(true);
            //         
            //         var sequence = (Sequence)typeof(TweenAnimator).GetField("_sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default)?.GetValue(target);
            //         DOTweenEditorPreview.PrepareTweenForPreview(sequence, true, false);
            //         DOTweenEditorPreview.Start();
            //     }
            //     else
            //     {
            //         targetAnimator.PlayBackwards();
            //     }
            // }
            if (GUILayout.Button(isPlaying ? "재시작" : "처음처럼"))
            {
                if (!isPlaying)
                {
                    DOTweenEditorPreview.Stop(true);
                }
                else
                {
                    targetAnimator.BakeSequence(true);
                    targetAnimator.ReStart();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            
            base.OnInspectorGUI();
        }
    }
}