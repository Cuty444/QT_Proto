using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Player;
using UnityEditor;
using UnityEngine;

public class PlayerChasingCamera : MonoBehaviour
{
    private Transform _player;

    private void Start()
    {
        _player = null;
        PlayerSystem playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
        playerSystem.PlayerCreateEvent.AddListener((obj) =>
        {
            _player = obj.transform;
        });
        playerSystem.OnPlayerCreate();
    }

    private void LateUpdate()
    {
        if (_player == null)
            return;
        transform.position = new Vector3(_player.position.x, _player.position.y, transform.position.z);
    }
}
