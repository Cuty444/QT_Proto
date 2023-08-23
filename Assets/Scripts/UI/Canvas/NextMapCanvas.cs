using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Map;
using QT.UI;
using UnityEngine;


namespace QT.NextMap
{
    public class NextMapCanvas : UIPanel
    {
        [SerializeField] private Transform[] panel;
        [SerializeField] private float _durationTime = 0.1f;
        private Vector2 panelSize = new Vector2(3000f, 1750f);
        
        private float _startTime;

        public override void PostSystemInitialize()
        {
            base.PostSystemInitialize();
            SystemManager.Instance.PlayerManager.PlayerDoorEnter.AddListener(OnDirection);
        }

        public void OnDirection(Vector2Int direction)
        {
            OnOpen();
            _startTime = Time.timeScale;
            Time.timeScale = 0f;
            
            if (direction == Vector2Int.up)
            {
                panel[1].gameObject.SetActive(true);
                StartCoroutine(MapBlur(panel[1], panelSize.y, -panelSize.y, _durationTime, false, () =>
                {
                    panel[1].gameObject.SetActive(false);
                    OnClose();
                }));
            }
            else if (direction == Vector2Int.down)
            {
                panel[1].gameObject.SetActive(true);
                StartCoroutine(MapBlur(panel[1], -panelSize.y, panelSize.y, _durationTime, false, () =>
                {
                    panel[1].gameObject.SetActive(false);
                    OnClose();
                }));
            }
            else if (direction == Vector2Int.left)
            {
                panel[0].gameObject.SetActive(true);
                StartCoroutine(MapBlur(panel[0], -panelSize.x, panelSize.x, _durationTime, true, () =>
                {
                    panel[0].gameObject.SetActive(false);
                    OnClose();
                }));
            }
            else if (direction == Vector2Int.right)
            {
                panel[0].gameObject.SetActive(true);
                StartCoroutine(MapBlur(panel[0], panelSize.x, -panelSize.x, _durationTime, true, () =>
                {
                    panel[0].gameObject.SetActive(false);
                    OnClose();
                }));
            }
        }

        private IEnumerator MapBlur(Transform target,float start,float end,float duration,bool isUPLR,Action func = null)
        {
            float startTime = Time.realtimeSinceStartup;
            if (isUPLR)
            {
                while (Time.realtimeSinceStartup - startTime < duration)
                {
                    target.transform.position =
                        new Vector2(Mathf.Lerp(start, end, (Time.realtimeSinceStartup - startTime) / duration), 0f);
                    yield return null;
                }
            }
            else
            {
                while (Time.realtimeSinceStartup - startTime < duration)
                {
                    target.transform.position =
                        new Vector2(0f, Mathf.Lerp(start, end, (Time.realtimeSinceStartup - startTime) / duration));
                    yield return null;
                }
            }
            Time.timeScale = _startTime;
            func?.Invoke();
        }
    }
}
