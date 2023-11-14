using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public class BossWaveStartCollider : MonoBehaviour
    {
        [SerializeField] private BossWaitDoor _bossWaitDoor;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Player") ||
                col.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _bossWaitDoor.PlayerEnter();

            }
        }
    }
}
