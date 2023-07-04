using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR

namespace QT.Map
{
    [ExecuteInEditMode]
    public class MapEditorSceneManager : MonoBehaviour
    {
        public MapCellData Target { get; private set; }

        private void Update()
        {
            CheckTarget();
        }

        private void CheckTarget()
        {
            var datas = FindObjectsOfType<MapCellData>();

            if (datas.Length == 1)
            {
                Target = datas[0];
            }
            else if (datas.Length > 1)
            {
                foreach (var data in datas)
                {
                    if (data == Target)
                    {
                        DestroyImmediate(data.gameObject);
                    }
                }
            }
        }
    }
}
#endif