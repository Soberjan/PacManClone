using System.Collections.Generic;
using UnityEngine;

public abstract class Ghost : MonoBehaviour
{
    public float speed = 3;
    public float cagedTime, chaseTime, scatterTime;
    public Transform scatterTarget, cageNodeOne, cageNodeTwo, cageNodeThree;

    public string behaviourMode;
    protected Timer timer;
    protected PathBuilder pathBuilder;
    protected Player player;
    protected Color color;

    public GameObject nextNode;
    public GameObject prevNode;

    public GameObject forbiddenNode;

    void Update()
    {
        if (transform.position == nextNode.transform.position)
        {
            Transform target = UpdateTarget();
            prevNode = nextNode;
            nextNode = pathBuilder.BuildPathToTarget(nextNode, target.gameObject, color);
        }
        MoveToNode(nextNode);
    }

    Transform UpdateTarget()
    {
        if (behaviourMode == "Chase")
            return HandleChase();
        else if (behaviourMode == "Scatter")
            return HandleScatter();
        else if (behaviourMode == "Frighten")
            return HandleFrighten();
        else if (behaviourMode == "Eaten")
            return HandleEaten();
        else if (behaviourMode == "Caged")
            return HandleCaged();
        return transform; //как избавиться от этой ошибки?
    }

    public Transform currentNode; //переименовать в currentTarget?
    protected abstract Transform HandleChase();
    protected Transform HandleScatter()
    {
        if (timer.Out() || transform.position == scatterTarget.position)
        {
            timer.Set(20);
            behaviourMode = "Chase";
            return HandleChase();
        }
        return scatterTarget;
    }
    Transform HandleFrighten()
    {
        if (Vector2.Equals(transform.position, nextNode.transform.position))
        {
            Node n = nextNode.GetComponent<Node>();
            List<GameObject> l = new List<GameObject>();
            foreach (Node node in n.neighbors)
                if (node.transform != prevNode.transform && node != forbiddenNode)
                    l.Add(node.gameObject);
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
            return HandleChase();
        }
        return currentNode;
    }

    protected GameObject ChooseClosestNode(Vector2 pos)
    {
        Collider2D checkPos = Physics2D.OverlapPoint(pos);
        if (checkPos != null && (Vector2)checkPos.transform.position == pos && checkPos.transform != nextNode.transform)
            return checkPos.gameObject;
        
        if (checkPos != null)
            checkPos.enabled = false;

        List<Collider2D> raysInfo = new List<Collider2D>();
        raysInfo.Add(Physics2D.Raycast(pos, new Vector2(-1, 0)).collider);
        raysInfo.Add(Physics2D.Raycast(pos, new Vector2(1, 0)).collider);
        raysInfo.Add(Physics2D.Raycast(pos, new Vector2(0, 1)).collider);
        raysInfo.Add(Physics2D.Raycast(pos, new Vector2(0, -1)).collider);
        
        float minDist = Mathf.Infinity;
        GameObject minNode = null;
        foreach (Collider2D n in raysInfo)
            if (n != null && (n.tag == "Node" || n.tag == "Teleporter") && Vector2.Distance(pos, n.transform.position) < minDist && n.gameObject.transform != nextNode.transform)
            {
                minDist = Vector2.Distance(pos, n.transform.position);
                minNode = n.gameObject;
            }

        if (checkPos != null)
            checkPos.enabled = true;
        
        return minNode;
    }

    void MoveToNode(GameObject nextNode)
    {
        if (prevNode.tag == "Teleporter" && nextNode.tag == "Teleporter")
            transform.position = nextNode.transform.position;
        else
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
    public void BecomeEaten()
    {
        GameManager.score += 100;

        behaviourMode = "Eaten";
        currentNode = cageNodeThree;

        //логика изменения текстуры
    }
}