using UnityEngine;
using UnityEditor;

public class SpawnNodesWindow : EditorWindow
{
    Texture2D headerSectionTexture;
    Texture2D nodeSectionTexture;
    Texture2D pointSectionTexture;

    Color headerSectionColor = new Color(176f/255f, 248f/255f, 255f/255f, 1f);
    Color nodeSectionColor = new Color(210f/255f, 210f/255f, 210f/255f, 1f);
    Color pointSectionColor = new Color(255f/255f, 221f/255f, 176f/255f, 1f);

    Rect headerSection;
    Rect nodeSection;
    Rect pointSection;

    GUIStyle style;

    GameObject node, parent, pellet, pelletParent;

    [MenuItem("Window/Spawn Nodes")]
    static void OpenWindow()
    {
        SpawnNodesWindow window = (SpawnNodesWindow)GetWindow(typeof(SpawnNodesWindow));
        window.minSize = new Vector2(600, 300);
        window.Show();
    }

    void OnEnable()
    {
        style = new GUIStyle();
        style.normal.textColor = Color.white;

        InitTextures();
    }

    void InitTextures()
    {
        headerSectionTexture = new Texture2D(1,1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        nodeSectionTexture = new Texture2D(1,1);
        nodeSectionTexture.SetPixel(0, 0, nodeSectionColor);
        nodeSectionTexture.Apply();

        pointSectionTexture = new Texture2D(1,1);
        pointSectionTexture.SetPixel(0, 0, pointSectionColor);
        nodeSectionTexture.Apply();
    }

    void OnGUI()
    {
        GUI.backgroundColor = Color.white;
        GUI.contentColor = Color.black;
        DrawLayouts();
        DrawHeader();
        DrawNodesSettings();
        DrawPelletSettings();
    }

    void DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 50;
        GUI.DrawTexture(headerSection, headerSectionTexture);

        nodeSection.x = 0;
        nodeSection.y = 50;
        nodeSection.width = Screen.width / 2;
        nodeSection.height = Screen.height - 50;
        GUI.DrawTexture(nodeSection, nodeSectionTexture);

        pointSection.x = Screen.width / 2;
        pointSection.y = 50;
        pointSection.width = Screen.width / 2;
        pointSection.height = Screen.height - 50;
        GUI.DrawTexture(pointSection, pointSectionTexture);
    }
    void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);

        GUILayout.Label("Shit Spawner");

        GUILayout.EndArea();
    }
    void DrawNodesSettings()
    {
        GUILayout.BeginArea(nodeSection);

        GUILayout.Label("Node Spawner");

        GameManager.xStart = EditorGUILayout.FloatField("xStart", GameManager.xStart, style);
        GameManager.yStart = EditorGUILayout.FloatField("yStart", GameManager.yStart, style);
        GameManager.xEnd = EditorGUILayout.FloatField("xEnd", GameManager.xEnd, style);
        GameManager.yEnd = EditorGUILayout.FloatField("yEnd", GameManager.yEnd, style);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Node Prefab");
        node = (GameObject)EditorGUILayout.ObjectField(node, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Parent Object");
        parent = (GameObject)EditorGUILayout.ObjectField(parent, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Spawn Nodes", GUILayout.Height(40)))
        {
            SpawnNodes();
        }
        if (GUILayout.Button("Connect Nodes", GUILayout.Height(40)))
        {
            GameManager.ConnectNodes();
        }

        GUILayout.EndArea();
    }
    void DrawPelletSettings()
    {
        GUILayout.BeginArea(pointSection);

        GUILayout.Label("Pellet Spawner");
        GUILayout.Label("Start and End coordiantes are the same as in the Node spawner");

        GameManager.xSkipStart = EditorGUILayout.FloatField("xSkipStart", GameManager.xSkipStart, style);
        GameManager.ySkipStart = EditorGUILayout.FloatField("ySkipStart", GameManager.ySkipStart, style);
        GameManager.xSkipEnd = EditorGUILayout.FloatField("xSkipEnd", GameManager.xSkipEnd, style);
        GameManager.ySkipEnd = EditorGUILayout.FloatField("ySkipEnd", GameManager.ySkipEnd, style);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Pellet Prefab");
        pellet = (GameObject)EditorGUILayout.ObjectField(pellet, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Parent Object");
        pelletParent = (GameObject)EditorGUILayout.ObjectField(pelletParent, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Spawn Pellets", GUILayout.Height(40)))
        {
            SpawnPellets();
        }

        GUILayout.EndArea();
    }

    void SpawnNodes()
    {
        for (float i = GameManager.yStart; i <= GameManager.yEnd; i++)
            for (float j = GameManager.xStart; j <= GameManager.xEnd; j++)
            {
                Vector2 nodePosition = new Vector2(j, i);
                GameObject currentNode;
                RaycastHit2D leftRay = Physics2D.Raycast(nodePosition, new Vector2(-1, 0), 1f);
                RaycastHit2D rightRay = Physics2D.Raycast(nodePosition, new Vector2(1, 0), 1f);
                RaycastHit2D upRay = Physics2D.Raycast(nodePosition, new Vector2(0, 1), 1f);
                RaycastHit2D downRay = Physics2D.Raycast(nodePosition, new Vector2(0, -1), 1f);
                if ( (leftRay.collider == null || rightRay.collider == null) && (upRay.collider == null || downRay.collider == null) )
                {
                    currentNode = Instantiate(node, nodePosition, Quaternion.identity);
                    currentNode.transform.SetParent(parent.transform);
                    GameManager.nodes.Add(currentNode);
                }
            }
    }

    void SpawnPellets()
    {
        for (float i = GameManager.yStart; i <= GameManager.yEnd; i++)
            for (float j = GameManager.xStart; j <= GameManager.xEnd; j++)
            {
                Vector2 pelletPos = new Vector2(j, i);
                Collider2D info = Physics2D.OverlapPoint(pelletPos);
                if ( (j < GameManager.xSkipStart || j > GameManager.xSkipEnd || i < GameManager.ySkipStart || i > GameManager.ySkipEnd) && (info == null || info.tag != "Wall") )
                {
                    GameObject currentPellet = Instantiate(pellet, pelletPos, Quaternion.identity);
                    currentPellet.transform.SetParent(pelletParent.transform);
                }
            }
    }
}
