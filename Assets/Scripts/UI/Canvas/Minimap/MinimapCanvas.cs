using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Map;
using QT.Core.Map;
using QT.Util;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace QT.UI
{
    public class MinimapCanvas : UIPanel
    {
        [field:SerializeField] public MiniMapCellData CellPrefab { get; private set; }
        
        [field:SerializeField] public RectTransform CellTransform { get; private set; }
        [field:SerializeField] public Vector2 CellSpace { get; private set; }
        
        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator ReleaseAnimator { get; private set; }
    }


    public class MinimapCanvasModel : UIModelBase
    {
        private const float MiniMapMoveTime = 0.2f;
        private const string CellPath = "Prefabs/Map/MiniMap/Cell.prefab";
        
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "Minimap.prefab";
        
        
        private MinimapCanvas _minimapCanvas;

        private List<MiniMapCellData> _cells = new ();


        private Vector2Int _startPos;
        
        
        public override void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.InGame:
                    Show();
                    break;
                default:
                    ReleaseUI();
                    break;
            }
        }

        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);

            _minimapCanvas = UIView as MinimapCanvas;
            SystemManager.Instance.ResourceManager.CacheAsset(CellPath);
        }

        public override void Show()
        {
            if (_minimapCanvas.gameObject.activeInHierarchy)
            {
                return;
            }
            
            base.Show();
            _minimapCanvas.StopAllCoroutines();
            _minimapCanvas.PopAnimator.ReStart();
        }
        
        public override void ReleaseUI()
        {
            if (!_minimapCanvas.gameObject.activeInHierarchy)
            {
                return;
            }
            
            _minimapCanvas.ReleaseAnimator.ReStart();
            _minimapCanvas.StopAllCoroutines();
            _minimapCanvas.StartCoroutine(UnityUtil.WaitForFunc(() => base.ReleaseUI(), _minimapCanvas.ReleaseAnimator.SequenceLength));
        }
        
        public void SetMiniMap(MapData mapData)
        {
            _startPos = mapData.StartPosition;
            
            
            var count = Mathf.Max(mapData.MapNodeList.Count, _cells.Count);

            for (var i = 0; i < count; i++)
            {
                if (i >= _cells.Count)
                {
                    _cells.Add(GameObject.Instantiate(_minimapCanvas.CellPrefab.gameObject, _minimapCanvas.CellTransform).GetComponent<MiniMapCellData>());
                }
                
                if (i >= mapData.MapNodeList.Count)
                {
                    _cells[i].gameObject.SetActive(false);
                    continue;
                }
                
                _cells[i].gameObject.SetActive(true);
                
                var mapPos = mapData.MapNodeList[i];
                
                var pos = mapPos - _startPos;
                _cells[i].RectTransform.anchoredPosition = new Vector2(pos.x, -pos.y) * _minimapCanvas.CellSpace;
                _cells[i].RectTransform.localScale = Vector3.one;

                _cells[i].CellPos = mapPos;
                _cells[i].SetRouteDirection(mapData.Map[mapPos.y, mapPos.x].DoorDirection);

                _cells[i].SetRoomType(mapData.Map[mapPos.y, mapPos.x].RoomType);
                _cells[i].Setting(_startPos);
            }
        }
        
        public void ChangeCenter(Vector2Int position)
        {
            var pos = position - _startPos;
            _minimapCanvas.CellTransform
                .DOAnchorPos(-(new Vector2(pos.x, -pos.y) * _minimapCanvas.CellSpace), MiniMapMoveTime)
                .SetEase(Ease.OutSine);
        }
        
    }
    
}
