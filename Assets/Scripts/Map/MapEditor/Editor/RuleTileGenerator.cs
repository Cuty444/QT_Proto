//This tool is a part of the VinTools Unity Package: https://vinarkgames.itch.io/vintools
// 출처 : https://github.com/Vinark117/Tutorials/blob/main/RuleTileGenerator/RuleTileGenerator.cs

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
public class RuleTileGenerator : EditorWindow
{
    private const string DefaultTemplatePath = "Assets/Scripts/Map/MapEditor/Template.png";
    private const string DefaultWallTemplatePath = "Assets/Scripts/Map/MapEditor/Template_Wall.png";
    
    private Vector2 scrollpos;

    public string tileName = "NewRuleTile";

    //default neighbor positions, copied from the rule tile script so we don't need a reference to it
    public List<Vector3Int> NeighborPositions = new List<Vector3Int>()
    {
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, -1, 0),
    };

    [MenuItem("맵 에디터/Rule Tile Generator", false, 1)]
    public static void ShowWindow()
    {
        GetWindow<RuleTileGenerator>("Rule Tile Generator");
    }

    private void OnGUI()
    {
        scrollpos = GUILayout.BeginScrollView(scrollpos);

        //if no rule preset is set
        if (templ_neighbors.Count == 0)
        {
            EditorGUILayout.Space();
            GUILayout.Label("Template setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty prp = so.FindProperty("templateSprites");
            EditorGUILayout.PropertyField(prp, true); // True means show children
            so.ApplyModifiedProperties(); //apply modified properties

            GUILayout.Box("Shift 키를 누르며 모든 템플릿 스프라이트 조각을 선택한 후 여기로 끌어오세요. \n텍스처에서 색상을 가져오려면 텍스처를 읽기/쓰기할 수 있어야 합니다.", EditorStyles.helpBox);

            EditorGUILayout.Space();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("기본 템플릿 가져오기", GUILayout.Width(100), GUILayout.ExpandWidth(true)))
            {
                SetTemplate(DefaultTemplatePath);
            }

            if (GUILayout.Button("기본 벽 템플릿 가져오기", GUILayout.Width(100), GUILayout.ExpandWidth(true)))
            {
                SetTemplate(DefaultWallTemplatePath);
            }
            
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("템플릿 로드하기"))
            {
                LoadTemplate();
            }
        }

        //if there is a preset loaded 
        if (templ_neighbors.Count > 0)
        {
            if (tileSprites == null)
            {
                tileSprites = Array.Empty<Sprite>();
            }
            
            EditorGUILayout.Space();
            GUILayout.Label("Preview", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            collumns = EditorGUILayout.IntField("Number of collumns", collumns);
            collumns = Mathf.Clamp(collumns, 1, int.MaxValue);
            previewBG = EditorGUILayout.ColorField("Preview BG color", previewBG);

            EditorGUILayout.Space();

            //show textures
            if ((templateSprites.Length) != tileSprites.Length)
            {
                DisplayTilemapPreview(collumns, templateSprites);
            }
            else
            {
                if (!DisplayTilemapPreview(collumns, tileSprites))
                {
                    DisplayTilemapPreview(collumns, templateSprites);
                }
            }

            EditorGUILayout.Space();

            GUILayout.Label("Tile Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty prp = so.FindProperty("tileSprites");
            EditorGUILayout.PropertyField(prp, true); // True means show children
            so.ApplyModifiedProperties(); //apply modified properties

            EditorGUILayout.Space();

            if (tileSprites.Length == 0) GUILayout.Box("Set sprites to show other options", EditorStyles.helpBox);
            else if (tileSprites.Length != templateSprites.Length) GUILayout.Box("Amount of sprites needs to be the same as the template", EditorStyles.helpBox);
            else
            {
                if (setDefaultIndex)
                {
                    defaultSprite = tileSprites[defaultIndex];
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Default sprite");
                defaultSprite = (Sprite)EditorGUILayout.ObjectField(defaultSprite, typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();

                colliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Default collider", colliderType);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Default gameobject");
                defaultGameobject = (GameObject)EditorGUILayout.ObjectField(defaultGameobject, typeof(GameObject), false);
                EditorGUILayout.EndHorizontal();

                addGameobjectsToRules = EditorGUILayout.Toggle("Add gameobject to rules", addGameobjectsToRules);

                EditorGUILayout.Space();

                tileName = EditorGUILayout.TextField("Tile name", tileName);

                EditorGUILayout.Space();

                if (GUILayout.Button("Create tile!"))
                {
                    SaveTile(GenerateRuleTile(), tileName);
                }
            }

        }

        GUILayout.EndScrollView();
    }

    private void SetTemplate(string path)
    {
        var sprites = AssetDatabase.LoadAllAssetsAtPath(path);
        templateSprites = new Sprite[sprites.Length-1];
        for (int i = 1; i < sprites.Length; i++)
        {
            templateSprites[i - 1] = sprites[i] as Sprite;
        }
    }
    

    public Sprite[] templateSprites = new Sprite[0];

    public List<List<int>> templ_neighbors = new List<List<int>>();

    int defaultIndex = 0;
    bool setDefaultIndex = false;

    void LoadTemplate()
    {
        //reset lists
        templ_neighbors = new List<List<int>>();

        //loop through the template sprites
        int i = 0;
        foreach (var item in templateSprites)
        {
            //create a new list to store the rules in
            List<int> neighborRules = new List<int>();

            //get slice data
            Rect slice = item.rect;
            Color[] cols = item.texture.GetPixels((int)slice.x, (int)slice.y, (int)slice.width, (int)slice.height);

            //create texture
            Texture2D tex = new Texture2D((int)slice.width, (int)slice.height, TextureFormat.ARGB32, false);
            tex.SetPixels(0, 0, (int)slice.width, (int)slice.height, cols);
            tex.filterMode = FilterMode.Point;
            tex.Apply();

            //get the size of the texture
            Vector2Int size = new Vector2Int(tex.width, tex.height);

            bool def = true;

            //set rules based on the color of the pixels
            foreach (var neighbor in NeighborPositions)
            {
                int xPos = 0;
                int yPos = 0;

                //get x pixel coordinate
                switch (neighbor.x)
                {
                    case 0:
                        xPos = size.x / 2;
                        break;
                    case 1:
                        xPos = size.x - 1;
                        break;
                }

                //get y pixel coordinate
                switch (neighbor.y)
                {
                    case 0:
                        yPos = size.y / 2;
                        break;
                    case 1:
                        yPos = size.y - 1;
                        break;
                }

                //get the pixel color
                Color c = tex.GetPixel(xPos, yPos);

                //add the color to the array
                if (c == Color.white)
                {
                    neighborRules.Add(0);
                }
                else if (c == Color.green)
                {
                    neighborRules.Add(RuleTile.TilingRule.Neighbor.This);
                    def = false;
                }
                else if (c == Color.red)
                {
                    neighborRules.Add(RuleTile.TilingRule.Neighbor.NotThis);
                }
            }

            //set default index if available
            if (def)
            {
                defaultIndex = i;
                setDefaultIndex = true;
            }

            //add the list to the list of lists
            templ_neighbors.Add(neighborRules);

            i++;
        }
    }

    int collumns = 6;

    public Color previewBG;

    bool DisplayTilemapPreview(int collumns, Sprite[] tiles)
    {
        //set up values
        float sidePadding = position.width * .05f;
        float size = (position.width * .9f / (float)collumns) * .9f; 
        float fullSize = (position.width * .9f / (float)collumns);
        float space = (position.width * .9f / (float)collumns) * .05f;
        float yPos = GUILayoutUtility.GetLastRect().y + sidePadding + space;

        int rows = (tiles.Length / collumns) + (tiles.Length % collumns > 0 ? 1 : 0);

        //draw BG color
        Texture2D bg = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        bg.SetPixel(0, 0, previewBG);
        bg.filterMode = FilterMode.Point;
        bg.Apply();

        EditorGUI.DrawPreviewTexture(new Rect(0, yPos + 10 - sidePadding - space, position.width, rows * fullSize + 2 * sidePadding), bg);

        //draw grid
        for (int y = 0, i = 0; y < rows; y++)
        {
            for (int x = 0; x < collumns; x++)
            {
                if (i < tiles.Length)
                {
                    //get slice data
                    if (!tiles[i] || !tiles[i].texture.isReadable)
                    {
                        return false;
                    }
                    Rect slice = tiles[i].rect;
                    
                    Color[] cols = tiles[i].texture.GetPixels((int)slice.x, (int)slice.y, (int)slice.width, (int)slice.height);

                    //create texture
                    Texture2D texture = new Texture2D((int)slice.width, (int)slice.height, TextureFormat.ARGB32, false);
                    texture.SetPixels(0, 0, (int)slice.width, (int)slice.height, cols);
                    texture.filterMode = FilterMode.Point;
                    texture.Apply();

                    //draw picture
                    EditorGUI.DrawPreviewTexture(new Rect(sidePadding + space + x * fullSize, yPos + 10 + y * fullSize, size, size), texture);

                    i++;
                }
            }

            EditorGUILayout.Space(fullSize);
        }

        EditorGUILayout.Space(2 * (sidePadding + space));

        return true;
    }

    public Sprite[] tileSprites;

    public Sprite defaultSprite;
    public Tile.ColliderType colliderType = Tile.ColliderType.Sprite;
    public GameObject defaultGameobject;
    public bool addGameobjectsToRules;

    public RuleTile GenerateRuleTile()
    {
        RuleTile tile = ScriptableObject.CreateInstance<RuleTile>();

        //set default tile
        tile.m_DefaultSprite = defaultSprite;
        tile.m_DefaultColliderType = colliderType;
        tile.m_DefaultGameObject = defaultGameobject;

        //set tiling rules
        for (int i = 0; i < tileSprites.Length; i++)
        {
            RuleTile.TilingRule rule = new RuleTile.TilingRule();
            rule.m_Sprites[0] = tileSprites[i];
            rule.m_Neighbors = templ_neighbors[i];
            rule.m_ColliderType = colliderType;
            if (addGameobjectsToRules) rule.m_GameObject = defaultGameobject;

            tile.m_TilingRules.Add(rule);
        }

        return tile;
    }

    public static void SaveTile(RuleTile tile, string name)
    {
        var path = EditorUtility.SaveFilePanelInProject("룰타일 저장", $"{name}.asset", "asset", "룰티일 저장");
        
        AssetDatabase.CreateAsset(tile, path);
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        
        Selection.activeObject = tile;
    }
}
#endif