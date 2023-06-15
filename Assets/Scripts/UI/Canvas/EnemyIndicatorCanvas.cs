using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Util;
using UnityEngine;

namespace QT.UI
{
    public class EnemyIndicatorCanvas : UIPanel
    {
        [Range(0.5f, 0.9f)]
        [SerializeField] private float _screenBoundOffset = 0.9f;

        [SerializeField] private Transform _indicatorPoolTransform;
        [SerializeField] private GameObject _indicatorObject;
        
        private Vector3 _screenCentre;
        private Vector3 _screenBounds;

        private Dictionary<IHitable,EnemyIndicator> _hitableList = new Dictionary<IHitable,EnemyIndicator>();
        
        private bool isSync = false;
        
        public override void PostSystemInitialize()
        {
            RoomEnemyRegister();
            _screenCentre = new Vector3(Screen.width, Screen.height, 0) / 2;
            _screenBounds = _screenCentre * _screenBoundOffset;
            OnOpen();
        }

        private void RoomEnemyRegister()
        {
            SystemManager.Instance.PlayerManager.CurrentRoomEnemyRegister.AddListener((hitables) =>
            {
                isSync = false;
                foreach (var hit in _hitableList)
                {
                    Destroy(hit.Value.gameObject);
                }
                _hitableList.Clear();
                foreach(var hit in hitables)
                {
                    _hitableList.Add(hit, Instantiate(_indicatorObject,_indicatorPoolTransform).GetComponent<EnemyIndicator>());
                }
                isSync = true;
            });
        }
        
        private void LateUpdate()
        {
            if (!isSync)
                return;
            
            DrawIndicator();
        }

        private void DrawIndicator()
        {
            int enemyCount = 0;
            foreach (var hp in _hitableList)
            {
                if (hp.Key.GetHp() > 0)
                {
                    enemyCount++;
                    if (enemyCount > 2)
                    {
                        return;
                    }
                }
            }
            foreach (var target in _hitableList)
            {
                if (target.Key.GetHp() <= 0)
                {
                    continue;
                }

                Vector3 screenPosition =
                    Util.ScreenMath.GetScreenPosition(Camera.main, target.Key.GetPosition());
                bool isTargetVisible = Util.ScreenMath.IsTargetVisible(screenPosition);
                if (isTargetVisible)
                {
                    target.Value.gameObject.SetActive(false);
                }
                else
                {
                    float angle = float.MinValue;
                    target.Value.gameObject.SetActive(true);
                    ScreenMath.GetIndicatorPositionAndAngle(ref screenPosition,ref angle,_screenCentre,_screenBounds);
                    target.Value.transform.position = screenPosition;
                    target.Value.transform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
                }
            }
        }
    }
}
