using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using QT.Core;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    public partial class Enemy
    {
        private SoundManager _soundManager;
        private EventReference _walkSound;
        private EventReference _deadSound;

        private void LoadSound()
        {
            _soundManager = SystemManager.Instance.SoundManager;

            if (gameObject.name.IndexOf("bat", StringComparison.Ordinal) >= 0)
            {
                _walkSound = _soundManager.SoundData.Bat_WalkSFX;
                _deadSound = _soundManager.SoundData.Bat_DeadSFX;
            }
            if (gameObject.name.IndexOf("Catcher", StringComparison.Ordinal) >= 0)
            {
                _walkSound = _soundManager.SoundData.Catcher_WalkSFX;
                _deadSound = _soundManager.SoundData.Catcher_DeadSFX;
            }
            if (gameObject.name.IndexOf("Earring", StringComparison.Ordinal) >= 0)
            {
                _walkSound = _soundManager.SoundData.Catcher_WalkSFX;
                _deadSound = _soundManager.SoundData.Catcher_DeadSFX;
            }
            if (gameObject.name.IndexOf("Flow", StringComparison.Ordinal) >= 0)
            {
                _walkSound = _soundManager.SoundData.Telekinesisz_WalkSFX;
                _deadSound = _soundManager.SoundData.Telekinesisz_DeadSFX;
            }
            if (gameObject.name.IndexOf("Ghost", StringComparison.Ordinal) >= 0)
            {
                _walkSound = _soundManager.SoundData.Ghost_WalkSFX;
                _deadSound = _soundManager.SoundData.Ghost_DeadSFX;
            }
            if (gameObject.name.IndexOf("Slime", StringComparison.Ordinal) >= 0)
            {
                _walkSound = _soundManager.SoundData.Slime_WalkSFX;
                _deadSound = _soundManager.SoundData.Slime_DeadSFX;
            }
        }


        public void WalkSound()
        {
            _soundManager.PlayOneShot(_walkSound);
        }

        public void DeadSound()
        {
            _soundManager.PlayOneShot(_deadSound);
        }
    }
}
