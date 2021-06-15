using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public float xStart = -14, yStart = -8, xEnd = 14, yEnd = 8;
    static public float xSkipStart = -3, ySkipStart = -2, xSkipEnd = 3, ySkipEnd = 2;
    static public List<GameObject> nodes = new List<GameObject>();
    public GameObject nodeParent, pelletParent;

    static public int health = 2, score = 0, pellets;

    static public bool paused;
    static RedGhost redGhost;
    static BlueGhost blueGhost;
    static OrangeGhost orangeGhost;
    static PinkGhost pinkGhost;

    public Transform piss;
    static public Transform shit;

    void Start()
    {
        foreach(Transform child in nodeParent.transform)
            nodes.Add(child.gameObject);
        
        pellets = pelletParent.transform.childCount;

        redGhost = FindObjectOfType<RedGhost>();
        blueGhost = FindObjectOfType<BlueGhost>();
        orangeGhost = FindObjectOfType<OrangeGhost>();
        pinkGhost = FindObjectOfType<PinkGhost>();

        shit = piss; //One big error handler
    }

    void Update()
    {
        if (pellets == 0)
            SceneManager.LoadScene(2);
    }

    static public void ResetNodes()
    {
        foreach (GameObject n in nodes)
        {
            n.GetComponent<Node>().val = Mathf.Infinity;
        }
    }

    static public void ConnectNodes()
    {
        foreach (GameObject n in nodes)
        {
            Vector2 nodePosition = new Vector2(n.transform.position.x, n.transform.position.y);
            RaycastHit2D leftRay = Physics2D.Raycast(new Vector2(nodePosition.x - 0.11f, nodePosition.y), new Vector2(-1, 0));
            RaycastHit2D rightRay = Physics2D.Raycast(new Vector2(nodePosition.x + 0.11f, nodePosition.y), new Vector2(1, 0));
            RaycastHit2D upRay = Physics2D.Raycast(new Vector2(nodePosition.x, nodePosition.y + 0.11f), new Vector2(0, 1));
            RaycastHit2D downRay = Physics2D.Raycast(new Vector2(nodePosition.x, nodePosition.y - 0.11f), new Vector2(0, -1));
            if (leftRay.collider != null && (leftRay.collider.tag == "Node" || leftRay.collider.tag == "Left Teleporter"))
                n.GetComponent<Node>().LeftNode = leftRay.collider.gameObject;
            if (rightRay.collider != null && (rightRay.collider.tag == "Node" || rightRay.collider.tag == "Right Teleporter"))
                n.GetComponent<Node>().RightNode = rightRay.collider.gameObject;
            if (upRay.collider != null && upRay.collider.tag == "Node")
                n.GetComponent<Node>().UpNode = upRay.collider.gameObject;
            if (downRay.collider != null && downRay.collider.tag == "Node")
                n.GetComponent<Node>().DownNode = downRay.collider.gameObject;
        }
    }

    static public void Damaged()
    {
        if (health == 0)
        {
            GameOver();
            return ;
        }

        redGhost.transform.position = new Vector2(0, 2);
        pinkGhost.transform.position = new Vector2(0, 0);
        orangeGhost.transform.position = new Vector2(-1, 0);
        blueGhost.transform.position = new Vector2(1, 0);

        redGhost.Init();
        pinkGhost.Init();
        orangeGhost.Init();
        blueGhost.Init();
        
        Player player = FindObjectOfType<Player>();
        player.transform.position = new Vector2(1, -4);
        player.isMoving = false;
        player.nextNode = player.startNode;
        player.moveDir = Vector2.right;

        health--;

        Pause();
    }

    static public void Pause()
    {
        paused = true;

        redGhost.speed = 0;
        blueGhost.speed = 0;
        orangeGhost.speed = 0;
        pinkGhost.speed = 0;
    }

    static public void UnPause()
    {
        paused = false;

        redGhost.speed = 3.25f;
        blueGhost.speed = 2.5f;
        orangeGhost.speed = 3.5f;
        pinkGhost.speed = 2.75f;
    }

    static public void GameOver()
    {
        SceneManager.LoadScene(2);
    }

    static public void FixThisShit()
    {
        orangeGhost.transform.position = new Vector2(Mathf.Round(orangeGhost.transform.position.x), Mathf.Round(orangeGhost.transform.position.y)); 
        redGhost.transform.position = new Vector2(Mathf.Round(redGhost.transform.position.x), Mathf.Round(redGhost.transform.position.y)); 
        pinkGhost.transform.position = new Vector2(Mathf.Round(pinkGhost.transform.position.x), Mathf.Round(pinkGhost.transform.position.y)); 
        blueGhost.transform.position = new Vector2(Mathf.Round(blueGhost.transform.position.x), Mathf.Round(blueGhost.transform.position.y));

        foreach(Transform t in shit)
            Destroy(t.gameObject);
    }

    static public void PickEnergizer()
    {
        redGhost.BecomeFrighten();
        orangeGhost.BecomeFrighten();
        pinkGhost.BecomeFrighten();
        blueGhost.BecomeFrighten();
    }
}
