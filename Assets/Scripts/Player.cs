using UnityEngine;
//ИЗМЕНИТЬ НАЗВАНИЯ ПЕРЕМЕННЫХ И ФУНКЦИЙ!!!!!!!!
public class Player : MonoBehaviour
{
    public float speed = 5;
    public bool isMoving = false;
    public GameObject startNode, nextNode, leftTeleporter, leftPreTeleporter, rightTeleporter, rightPreTeleporter;
    public Vector2 moveDir = Vector2.right;

    public GameObject prevNode, preNode = null;

    void Update()
    {
        GameObject dir = HandleInput();

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
        // if (nextNode.tag != "Right Teleporter" && nextNode.tag != "Left Teleporter")
        // {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                return nextNode.GetComponent<Node>().DownNode;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                return nextNode.GetComponent<Node>().UpNode;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                return nextNode.GetComponent<Node>().LeftNode;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                return nextNode.GetComponent<Node>().RightNode;
        //}
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
        // if (nextNode.tag == "Right Teleporter")
        // {
        //     transform.position = new Vector2(-16, 0);
        //     prevNode = leftTeleporter;
        //     nextNode = leftPreTeleporter;
        //     return ;
        // }
        // if (nextNode.tag == "Left Teleporter")
        // {
        //     transform.position = new Vector2(16, 0);
        //     prevNode = rightTeleporter;
        //     nextNode = rightPreTeleporter;
        //     return ;
        // }
        if (preNode == null)
        {
            Node prevInfo = prevNode.GetComponent<Node>(), nextInfo = nextNode.GetComponent<Node>();
            if (prevInfo.UpNode != null && prevInfo.UpNode == nextNode && nextInfo.UpNode != null)
            {
                prevNode = nextNode;
                nextNode = nextInfo.UpNode;
                return ;
            }
            if (prevInfo.DownNode != null && prevInfo.DownNode == nextNode && nextInfo.DownNode != null)
            {
                prevNode = nextNode;
                nextNode = nextInfo.DownNode;
                return ;
            }
            if (prevInfo.LeftNode != null && prevInfo.LeftNode == nextNode && nextInfo.LeftNode != null)
            {
                prevNode = nextNode;
                nextNode = nextInfo.LeftNode;
                return ;
            }
            if (prevInfo.RightNode != null && prevInfo.RightNode == nextNode && nextInfo.RightNode != null)
            {
                prevNode = nextNode;
                nextNode = nextInfo.RightNode;
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
        if (prevInfo.UpNode != null && prevInfo.UpNode == nextNode)
        {
            moveDir = Vector2.up;
            transform.rotation = Quaternion.Euler(0,0,90);
            return ;
        }
        if (prevInfo.DownNode != null && prevInfo.DownNode == nextNode)
        {
            moveDir = Vector2.down;
            transform.rotation = Quaternion.Euler(0,0,-90);
            return ;
        }
        if (prevInfo.LeftNode != null && prevInfo.LeftNode == nextNode)
        {
            moveDir = Vector2.left;
            transform.rotation = Quaternion.Euler(0,0,180);
            return ;
        }
        if (prevInfo.RightNode != null && prevInfo.RightNode == nextNode)
        {
            moveDir = Vector2.right;
            transform.rotation = Quaternion.Euler(0,0,0);
            return ;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ghost")
            GameManager.Damaged();
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
