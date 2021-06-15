using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float speed = 3;
    public float cagedTime, chaseTime, scatterTime;
    public Transform scatterTarget, cageNodeOne, cageNodeTwo, cageNodeThree;

    public string behaviourMode;
    public Transform currentTarget;
    protected Timer timer;
    protected PathBuilder pathBuilder;
    protected Player player;
    protected Color color;

    public GameObject nextNode;

    void Update()
    {
        UpdateTarget();
        nextNode = pathBuilder.BuildPathToTarget(transform, currentTarget, color);
        MoveToNode(nextNode);
    }

    void UpdateTarget()
    {
        if (behaviourMode == "Chase")
            currentTarget = HandleChase();
        else if (behaviourMode == "Scatter")
            currentTarget = HandleScatter();
        else if (behaviourMode == "Frighten")
            currentTarget = HandleFrighten();
        else if (behaviourMode == "Eaten")
            currentTarget = HandleEaten();
        else if (behaviourMode == "Caged")
            currentTarget = HandleCaged();
    }

    //ЗАМЕНИТЬ Transform на Vector2!!!!!!!!!!!!!!!!
    public Transform currentNode;
    protected virtual Transform HandleChase()
    {
        return transform;
    }
    protected virtual Transform HandleScatter()
    {
        if (timer.Out() || transform.position == scatterTarget.position)
        {
            timer.Set(20);
            behaviourMode = "Chase";
        }
        return scatterTarget;
    }
    Transform HandleFrighten() //переход в frighten и currentNode буду обрабатывать отдельной функцией
    {
        if (Vector2.Equals(transform.position, currentNode.position))
        {
            Node n = currentNode.gameObject.GetComponent<Node>();
            List<GameObject> l = new List<GameObject>();
            if (n.LeftNode != null)
                l.Add(n.LeftNode);
            if (n.RightNode != null)
                l.Add(n.RightNode);
            if (n.UpNode != null)
                l.Add(n.UpNode);
            if (n.DownNode != null)
                l.Add(n.DownNode);
            int a = Random.Range(0, l.Count);
            currentNode = l[a].transform;
        }
        if (timer.Out())
        {
            timer.Set(chaseTime);
            behaviourMode = "Chase";
        }
        return currentNode;
    }
    Transform HandleEaten()
    {
        if (transform.position == cageNodeThree.position)
        {
            timer.Set(chaseTime);
            behaviourMode = "Chase";
        }
        return cageNodeThree;
    }
    Transform HandleCaged()
    {
        if (transform.position == currentNode.position)
            currentNode = currentNode == cageNodeOne ? cageNodeTwo : cageNodeOne;
        if (timer.Out())
        {
            timer.Set(chaseTime);
            behaviourMode = "Chase";
            currentNode = scatterTarget;
        }
        return currentNode;
    }

    void MoveToNode(GameObject nextNode)
    {
        transform.position = Vector2.MoveTowards(transform.position, nextNode.transform.position, speed * Time.deltaTime);
    }

    public void BecomeFrighten() //-ed
    {
        if (behaviourMode != "Caged")
        {
            behaviourMode = "Frighten";
            timer.Set(10);

            RaycastHit2D leftRay = Physics2D.Raycast(transform.position, new Vector2(-1, 0));
            RaycastHit2D rightRay = Physics2D.Raycast(transform.position, new Vector2(1, 0));
            RaycastHit2D upRay = Physics2D.Raycast(transform.position, new Vector2(0, 1));
            RaycastHit2D downRay = Physics2D.Raycast(transform.position, new Vector2(0, -1));
            if (leftRay.collider != null && leftRay.collider.tag == "Node")
                currentNode = leftRay.collider.transform;
            else if (rightRay.collider != null && rightRay.collider.tag == "Node")
                currentNode = rightRay.collider.transform;
            else if (upRay.collider != null && upRay.collider.tag == "Node")
                currentNode = upRay.collider.transform;
            else if (downRay.collider != null && downRay.collider.tag == "Node")
                currentNode = downRay.collider.transform;

            //логика изменения текстуры
        }
    }
}