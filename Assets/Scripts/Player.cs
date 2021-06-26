using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public bool isMoving = false;
    public GameObject startNode, nextNode, leftTeleporter, leftPreTeleporter, rightTeleporter, rightPreTeleporter;
    public Vector2 moveDir = Vector2.right;

    public GameObject prevNode, preNode = null;

    void Update()
    {
        GameObject dir;
        dir = HandleInput();

        if (isMoving)
            ProcessMovingInput(dir);
        else
            ProcessStillInput(dir);

        MoveToNodeContiniously();
    }

    void MoveToNodeContiniously()
    {
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, nextNode.transform.position, speed * Time.deltaTime);
            if (transform.position == nextNode.transform.position)
                ArriveAtNode();
        }
    }

    GameObject HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            return ChoosePreNode("LeftNode");
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            return ChoosePreNode("RightNode");
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            return ChoosePreNode("DownNode");
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            return ChoosePreNode("UpNode");
        return null;
    }

    GameObject ChoosePreNode(string nodeType)
    {
        foreach(Node n in nextNode.GetComponent<Node>().neighbors)
        {
            if (nodeType == "LeftNode" && n.transform.position.x < nextNode.transform.position.x)
                return n.gameObject;
            else if (nodeType == "RightNode" && n.transform.position.x > nextNode.transform.position.x)
                return n.gameObject;
            else if (nodeType == "DownNode" && n.transform.position.y < nextNode.transform.position.y)
                return n.gameObject;
            else if (nodeType == "UpNode" && n.transform.position.y > nextNode.transform.position.y)
                return n.gameObject;
        }
        return null;
    }

    void ProcessMovingInput(GameObject dir)
    {
        if (dir == null)
            return ;
        if (dir == prevNode)
        {
            GameObject tmp = prevNode;
            prevNode = nextNode;
            nextNode = tmp;
            preNode = null; //иначе если в начале нажать стрелку вверх, потом влево, пакман поедет по диагонали 
            Rotate();
            return ;
        }
        preNode = dir;
    }

    void ProcessStillInput(GameObject dir)
    {
        if (dir == null)
            return ;
        
        if (GameManager.paused)
            GameManager.UnPause();

        prevNode = nextNode;
        nextNode = dir;
        isMoving = true;
        Rotate();
    }

    void ArriveAtNode()
    {
        if (nextNode == rightTeleporter)
        {
            transform.position = new Vector2(-16, 0);
            prevNode = leftTeleporter;
            nextNode = leftPreTeleporter;
            return ;
        }
        if (nextNode == leftTeleporter)
        {
            transform.position = new Vector2(16, 0);
            prevNode = rightTeleporter;
            nextNode = rightPreTeleporter;
            return ;
        }
        if (preNode == null)
        {
            Node prevInfo = prevNode.GetComponent<Node>(), nextInfo = nextNode.GetComponent<Node>();
            foreach (Node n in nextInfo.neighbors)
                if (n.transform.position.x != prevInfo.transform.position.x && n.transform.position.y == prevInfo.transform.position.y || n.transform.position.y != prevInfo.transform.position.y && n.transform.position.x == prevInfo.transform.position.x)
                {
                    prevNode = nextNode;
                    nextNode = n.gameObject;
                    return ;
                }
            
            prevNode = nextNode;
            isMoving = false;
            Rotate();
        }
        if (preNode != null)
        {
            prevNode = nextNode;
            nextNode = preNode;
            preNode = null;
            Rotate();
        }
    }

    void Rotate()
    {
        Node prevInfo = prevNode.GetComponent<Node>(), nextInfo = nextNode.GetComponent<Node>();
        
        if (nextInfo.transform.position.y > prevInfo.transform.position.y)
        {
            moveDir = Vector2.up;
            transform.rotation = Quaternion.Euler(0,0,90);
            return ;
        }
        if (nextInfo.transform.position.y < prevInfo.transform.position.y)
        {
            moveDir = Vector2.down;
            transform.rotation = Quaternion.Euler(0,0,-90);
            return ;
        }
        if (nextInfo.transform.position.x < prevInfo.transform.position.x)
        {
            moveDir = Vector2.left;
            transform.rotation = Quaternion.Euler(0,0,180);
            return ;
        }
        if (nextInfo.transform.position.x > prevInfo.transform.position.x)
        {
            moveDir = Vector2.right;
            transform.rotation = Quaternion.Euler(0,0,0);
            return ;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ghost")
        {
            Ghost ghost = col.gameObject.GetComponent<Ghost>();
            if (ghost.behaviourMode == "Chase" || ghost.behaviourMode == "Scatter")
                GameManager.Damaged();
            else if (col.gameObject.GetComponent<Ghost>().behaviourMode == "Frighten")
                ghost.BecomeEaten();
        }
        if (col.gameObject.tag == "Pellet")
        {
            GameManager.pellets--;
            GameManager.score += 10;
            Destroy(col.gameObject);
        }
        if (col.gameObject.tag == "Fruit")
        {
            GameManager.score += 250;
            Fruit.t.Set(15);
            Fruit.fruitState = "Invisible";
            Destroy(col.gameObject);
        }
        if (col.gameObject.tag == "Energizer")
        {
            GameManager.PickEnergizer();
            Destroy(col.gameObject);
        }
    }
}
