using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QT.UI;

namespace QT.UI
{
    public class PlayerHPCanvas : UIPanel
    {
        [SerializeField] private Image _playerHPImage;
        [SerializeField] private Image _playerInvicibleImage;
        [SerializeField] private Image _playerBallStackImage;
        [SerializeField] private Image _playerDodgeCoolBarImage;
        [SerializeField] private Image _playerDodgeCoolBackgroundImage;
        public Image PlayerHPImage => _playerHPImage;
        public Image PlayerInvicibleImage => _playerInvicibleImage;

        public Image PlayerBallStackImage => _playerBallStackImage;

        public Image PlayerDodgeCoolBarImage => _playerDodgeCoolBarImage;
        public Image PlayerDodgeCoolBackgroundImage => _playerDodgeCoolBackgroundImage;

    }

}