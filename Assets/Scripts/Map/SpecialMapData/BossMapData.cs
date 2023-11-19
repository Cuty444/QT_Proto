using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Map;
using QT.UI;
using UnityEngine;

namespace QT
{
    public class BossMapData : SpecialMapData
    {
        [field:SerializeField] public EnemyWave BossWave { get; private set; }
        
        private bool _isEntered = false;
        
        private void Awake()
        {
            SystemManager.Instance.PlayerManager.PlayerMapPosition.AddListener(ShowVidio);
        }

        private void OnDestroy()
        {
            SystemManager.Instance?.PlayerManager.PlayerMapPosition.RemoveListener(ShowVidio);
        }

        private void ShowVidio(Vector2Int position)
        {
            if (!_isEntered && position == MapPosition)
            {
                _isEntered = true;
                SystemManager.Instance.UIManager.Show<DullahanIntroVideoCanvas>();
            }
        }
    }
}
