using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public class ChangeCursor : MonoBehaviour
    {

        [SerializeField] Texture2D cursorImg;

        // Start is called before the first frame update
        void Start()
        {

            Cursor.SetCursor(cursorImg, Vector2.zero, CursorMode.ForceSoftware);
        
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
