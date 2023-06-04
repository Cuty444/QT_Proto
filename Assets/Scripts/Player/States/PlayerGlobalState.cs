using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using QT.UI;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace QT.InGame
{
    [FSMState((int)Player.States.Global, false)]
    public class PlayerGlobalState : FSMState<Player>
    {
        private readonly int AnimationMouseRotateHash = Animator.StringToHash("MouseRotate");
        private readonly int AnimationRigidHash = Animator.StringToHash("PlayerRigid");
        private const string TeleportLinePath = "Prefabs/TeleportLine.prefab";

        private PlayerHPCanvas _playerHpCanvas;

        private List<LineRenderer> _teleportLines = new();

        private Dictionary<LineRenderer, Enemy> _teleportEnemyCheckDictionary = new Dictionary<LineRenderer, Enemy>();

        private GlobalDataSystem _globalDataSystem;

        public PlayerGlobalState(IFSMEntity owner) : base(owner)
        {
            _playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            _playerHpCanvas.gameObject.SetActive(true);
            
            _playerHpCanvas.SetHp(_ownerEntity.GetStatus(PlayerStats.HP));
            SystemManager.Instance.ResourceManager.CacheAsset(TeleportLinePath);

            _ownerEntity.SetBatActive(false);
            SystemManager.Instance.PlayerManager.OnDamageEvent.AddListener(OnDamage);
            SystemManager.Instance.PlayerManager.CurrentRoomEnemyRegister.AddListener(arg0 => TeleportLineClear());
            SystemManager.Instance.PlayerManager.PlayerMapPosition.AddListener(arg0 => TeleportLineClear());
            SystemManager.Instance.PlayerManager.EnemyDeathStateChanged.AddListener(EnemyNotRigidState);
            SystemManager.Instance.PlayerManager.EnemyProjectileStateChanged.AddListener(EnemyNotRigidState);

            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _ownerEntity.OnAim.AddListener(OnAim);
        }


        private void OnAim(Vector2 aimPos)
        {
            if (_ownerEntity.CurrentStateIndex == (int)Player.States.Teleport)
            {
                return;
            }
            var aimDir = ((Vector2) _ownerEntity.transform.position - aimPos).normalized;

            if (_ownerEntity.IsReverseLookDir)
            {
                aimDir *= -1;
            }
            
            float flip = 180;
            if (_ownerEntity.IsDodge)
            {
                flip = _ownerEntity.IsFlip ? 180f : 0f;
                _ownerEntity.Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);
                return;
            }
            var angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90;
            
            if (angle < 0)
            {
                angle += 360;
            }
            
            _ownerEntity.EyeTransform.rotation = Quaternion.Euler(0, 0, angle + 270);

            if (angle > 180)
            {
                angle = 360 - angle;
                flip = 0;
            }
            
            _ownerEntity.Animator.SetFloat(AnimationMouseRotateHash, angle / 180 * 4);
            _ownerEntity.Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);
        }


        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            SetTeleportLines();
        }

        private void OnDamage(Vector2 dir, float damage)
        {
            _ownerEntity.Animator.SetTrigger(AnimationRigidHash);
            //_ownerEntity.PlayerHitEffectPlay();
            
            var hp = _ownerEntity.GetStatus(PlayerStats.HP);
            _ownerEntity.MaterialChanger.SetHitMaterial();
            hp.AddStatus(-damage);
            _playerHpCanvas.CurrentHpImageChange(hp);

            _ownerEntity.DamageImpulseSource.GenerateImpulse(dir);
            
            if (hp <= 0)
            {
                PlayerDead();
            }
        }

        private void PlayerDead()
        {
            SystemManager.Instance.PlayerManager.PlayerThrowProjectileReleased.RemoveAllListeners();
            SystemManager.Instance.PlayerManager.OnDamageEvent.RemoveAllListeners();
            _ownerEntity.ChangeState(Player.States.Dead);
        }

        private void TeleportLineClear()
        {
            foreach (var line in _teleportLines)
            {
                line.positionCount = 0;
                SystemManager.Instance.ResourceManager.ReleaseObject(TeleportLinePath,line);
            }
            
            _teleportLines.Clear();
            _teleportEnemyCheckDictionary.Clear();
            SetTeleportLineObjects();
            Debug.Log("Line Clear");
        }
        
        private async void SetTeleportLineObjects()
        {
            for (int i = 0; i < 10; i++)
            {
                var line = await SystemManager.Instance.ResourceManager.GetFromPool<LineRenderer>(TeleportLinePath,_ownerEntity.TeleportLineTransform);
                line.positionCount = 0;
                _teleportLines.Add(line);
            }
        }
        
        private void SetTeleportLines()
        {
            var list = _ownerEntity.GetRigidEnemyList(); 
            int enemyIndex = 0;
            int i = 0;
            for (; i < _teleportLines.Count; i++)
            {
                if (enemyIndex >= list.Item2.Count)
                    break;
                
                SetTeleportLine(_teleportLines[i],list.Item2[enemyIndex].Position, true);
                if (_teleportEnemyCheckDictionary.ContainsKey(_teleportLines[i]))
                {
                    _teleportEnemyCheckDictionary[_teleportLines[i]] = list.Item2[enemyIndex++];
                }
                else
                {
                    _teleportEnemyCheckDictionary.Add(_teleportLines[i],list.Item2[enemyIndex++]);
                }
            }

            for (enemyIndex = 0; i < _teleportLines.Count; i++)
            {
                if (enemyIndex >= list.Item1.Count)
                    break;
                
                SetTeleportLine(_teleportLines[i],list.Item1[enemyIndex].Position,false);
                if (_teleportEnemyCheckDictionary.ContainsKey(_teleportLines[i]))
                {
                    _teleportEnemyCheckDictionary[_teleportLines[i]] = list.Item1[enemyIndex++];
                }
                else
                {
                    _teleportEnemyCheckDictionary.Add(_teleportLines[i], list.Item1[enemyIndex++]);
                }
            }
        }

        private void EnemyNotRigidState(Enemy enemy)
        {
            if (_teleportEnemyCheckDictionary.ContainsValue(enemy))
            {
                foreach (var data in _teleportEnemyCheckDictionary)
                {
                    if (data.Value == enemy)
                    {
                        data.Key.positionCount = 0;
                        _teleportEnemyCheckDictionary.Remove(data.Key);
                        break;
                    }
                }
            }
        }
        
        private void SetTeleportLine(LineRenderer lineRenderer,Vector2 position,bool isDistance)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0,_ownerEntity.transform.position);
            lineRenderer.SetPosition(1,position);
            if (isDistance)
            {
                lineRenderer.startColor = _globalDataSystem.GlobalData.CloseColor;
                lineRenderer.endColor = _globalDataSystem.GlobalData.CloseColor;
                //lineRenderer.colorGradient.colorKeys[0] = new GradientColorKey(_globalDataSystem.GlobalData.CloseColor, 0f);
                //lineRenderer.colorGradient.colorKeys[1] = new GradientColorKey(_globalDataSystem.GlobalData.CloseColor, 1f);
                //lineRenderer.colorGradient.alphaKeys[0] = new GradientAlphaKey(_globalDataSystem.GlobalData.CloseColor.a, 0f);
                //lineRenderer.colorGradient.alphaKeys[1] = new GradientAlphaKey(_globalDataSystem.GlobalData.CloseColor.a, 1f);
            }
            else
            {
                lineRenderer.startColor = _globalDataSystem.GlobalData.FarColor;
                lineRenderer.endColor = _globalDataSystem.GlobalData.FarColor;
                //lineRenderer.colorGradient.colorKeys[0] = new GradientColorKey(_globalDataSystem.GlobalData.FarColor, 0f);
                //lineRenderer.colorGradient.colorKeys[1] = new GradientColorKey(_globalDataSystem.GlobalData.FarColor, 1f);
                //lineRenderer.colorGradient.alphaKeys[0] = new GradientAlphaKey(_globalDataSystem.GlobalData.FarColor.a, 0f);
                //lineRenderer.colorGradient.alphaKeys[1] = new GradientAlphaKey(_globalDataSystem.GlobalData.FarColor.a, 1f);
                
            }
        }
        
    }

}