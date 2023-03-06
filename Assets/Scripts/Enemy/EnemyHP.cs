using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHP : MonoBehaviour
{
    [SerializeField] private int _maxHp = 300;
    [SerializeField] private RectTransform _hpRectTransform;
    private Image _hpImage;
    private int _currentHp;
    private Rigidbody2D _rigidbody2D;
    private void Awake()
    {
        _currentHp = _maxHp;
        _hpImage = _hpRectTransform.GetComponentsInChildren<Image>()[1];
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity = Vector3.zero;
    }

    public void HitDamage(int damage)
    {
        if (_currentHp <= 0)
            return;
        _currentHp -= damage;
        Debug.Log(QT.Util.Math.floatNormalization(_currentHp, _maxHp, 0));
        _hpImage.fillAmount = QT.Util.Math.floatNormalization(_currentHp, _maxHp, 0);
        if(_currentHp == 0)
        {
            Invoke("EnemyDead", 0.5f);
        }
    }
    
    private void EnemyDead()
    {
        Destroy(gameObject);
        Destroy(_hpRectTransform.gameObject);
    }
}
