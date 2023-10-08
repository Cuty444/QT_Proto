using System;
using System.Collections;
using System.Collections.Generic;
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
        [field:SerializeField] public RectTransform CellTransform { get; private set; }
        [field:SerializeField] public Vector2 CellSize { get; private set; }
        [field:SerializeField] public Vector2 CellSpace { get; private set; }
        
        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator ReleaseAnimator { get; private set; }
    }


    public class MinimapCanvasModel : UIModelBase
    {
        private const string CellPath = "Prefabs/Map/MiniMap/Cell.prefab";
        
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "Minimap.prefab";
        
        
        private MinimapCanvas _minimapCanvas;
        
        private Dictionary<MiniMapCellData, Vector2> _cellMapDictionary = new ();

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
        

        private void ClearCells()
        {
            foreach (var cell in _cellMapDictionary.Keys)
            {
                cell.ClearListener();
                SystemManager.Instance.ResourceManager.ReleaseObject(CellPath, cell);
                //Destroy(cell.gameObject);
            }
            _cellMapDictionary.Clear();
        }

        public async void SetMiniMap(MapData mapData)
        {
            ClearCells();
            
            _startPos = mapData.StartPosition;
            
            foreach (var mapPos in mapData.MapNodeList)
            {
                var miniMapCell = await SystemManager.Instance.ResourceManager.GetFromPool<MiniMapCellData>(CellPath, _minimapCanvas.CellTransform);
                
                var pos = mapPos - _startPos;
                miniMapCell.RectTransform.anchoredPosition = new Vector2(pos.x, -pos.y) * _minimapCanvas.CellSpace;
                miniMapCell.RectTransform.sizeDelta = _minimapCanvas.CellSize;
                miniMapCell.RectTransform.localScale = Vector3.one;
                
                miniMapCell.CellPos = mapPos;
                //miniMapCell.SetRouteDirection(cellData.DoorDirection);

                miniMapCell.SetRoomType(mapData.Map[mapPos.y, mapPos.x].RoomType);
                miniMapCell.Setting();

                _cellMapDictionary.Add(miniMapCell, mapPos);
            }
        }
        
        public void ChangeCenter(Vector2Int position)
        {
            var pos = position - _startPos;
            _minimapCanvas.CellTransform.anchoredPosition = -(new Vector2(pos.x, -pos.y) * _minimapCanvas.CellSpace);
        }
        
    }
    
}
