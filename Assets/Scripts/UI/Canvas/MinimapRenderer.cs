using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core.Map;
using QT.Map;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class MinimapRenderer : MonoBehaviour
    {
        private const float MiniMapMoveTime = 0.2f;
        
        [field:SerializeField] public MiniMapCellData CellPrefab { get; private set; }
        
        [field:SerializeField] public RectTransform ViewportTransform { get; private set; }
        [field:SerializeField] public RectTransform ScrollTransform { get; private set; }
        [field:SerializeField] public RectTransform CellTransform { get; private set; }
        [field:SerializeField] public Vector2 CellSpace { get; private set; }
        
        private List<MiniMapCellData> _cells = new ();
        private List<MiniMapCellData> _activeCells = new ();
        private Vector2Int _startPos;
        
        public void SetMiniMap(MapData mapData)
        {
            _activeCells.Clear();
            _startPos = mapData.StartPosition;
            
            var count = Mathf.Max(mapData.MapNodeList.Count, _cells.Count);
            
            for (var i = 0; i < count; i++)
            {
                if (i >= _cells.Count)
                {
                    _cells.Add(Instantiate(CellPrefab.gameObject, CellTransform).GetComponent<MiniMapCellData>());
                }
                
                if (i >= mapData.MapNodeList.Count)
                {
                    _cells[i].gameObject.SetActive(false);
                    continue;
                }
                
                _cells[i].gameObject.SetActive(true);

                
                var mapPos = mapData.MapNodeList[i];
                var cellData = mapData.Map[mapPos.y, mapPos.x];
                
                var rPos = mapPos - _startPos;
                _cells[i].RectTransform.anchoredPosition = new Vector2(rPos.x, -rPos.y) * CellSpace;
                _cells[i].RectTransform.localScale = Vector3.one;

                _cells[i].CellPos = mapPos;
                _cells[i].SetRouteDirection(cellData.DoorDirection);
                _cells[i].SetRoomType(cellData.RoomType);
                _cells[i].Setting(_startPos);
                
                _activeCells.Add(_cells[i]);
            }
        }
        
        public void ChangeCenter(Vector2Int position)
        {
            var rPos = _startPos - position;
            var aPos = (new Vector2(rPos.x, -rPos.y) * CellSpace) - CellTransform.anchoredPosition;
            
            ScrollTransform.DOAnchorPos(aPos, MiniMapMoveTime).SetEase(Ease.OutSine);
        }

        public void ResizeSize()
        {
            var size = new Rect();
            foreach (var cell in _activeCells)
            {
                if (!cell.IsIconVisible)
                {
                    continue;
                }
                
                var pos = cell.RectTransform.anchoredPosition;
                
                if(pos.x <= size.xMin)
                    size.xMin = pos.x;
                else if(pos.x >= size.xMax)
                    size.xMax = pos.x;
                
                if(pos.y <= size.yMin)
                    size.yMin = pos.y;
                else if(pos.y >= size.yMax)
                    size.yMax = pos.y;
            }

            ScrollTransform.sizeDelta = size.size + ViewportTransform.rect.size;
            CellTransform.anchoredPosition = -size.center;
        }
    }
}
