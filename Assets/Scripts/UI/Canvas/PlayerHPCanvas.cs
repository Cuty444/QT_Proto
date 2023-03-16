using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QT.UI;

public class PlayerHPCanvas : UIPanel
{
    [SerializeField] private Image _playerHPImage;
    public Image PlayerHPImage => _playerHPImage;
}
