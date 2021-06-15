using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : MonoBehaviour
{
    public GameObject nodePrefab;
    List<float> xNodeRow = new List<float>();
    List<float> yNodeRow = new List<float>();

    void Start()
    {
        for (int i = -14; i < 15; i++)
            xNodeRow.Add(i);
        for (int i = -8; i < 9; i++)
            yNodeRow.Add(i);
    }

    public GameObject BuildPathToTarget(Transform start, Transform end, Color color)
    {
        GameObject startNode = SpawnNode(start.position, true);
        
        GameObject endNode = SpawnNode(end.position, false);

        List<GameObject> queue = new List<GameObject>();
        List<GameObject> used = new List<GameObject>();

        startNode.GetComponent<Node>().val = 0;
        queue.Add(startNode);
        int govno = 0;
        while (queue.Count != 0 && govno < 100)
        {
            govno++;
            GameObject curNode = queue[0];

            GameObject leftNode = curNode.GetComponent<Node>().LeftNode;
            GameObject rightNode = curNode.GetComponent<Node>().RightNode;
            GameObject upNode = curNode.GetComponent<Node>().UpNode;
            GameObject downNode = curNode.GetComponent<Node>().DownNode;

            if (leftNode != null && !used.Contains(leftNode))
            {
                if (queue.Contains(leftNode))
                    queue[queue.IndexOf(leftNode)].GetComponent<Node>().val = Mathf.Min(curNode.GetComponent<Node>().val + Vector2.Distance(curNode.transform.position, leftNode.transform.position), queue[queue.IndexOf(leftNode)].GetComponent<Node>().val);
                else
                {
                    leftNode.GetComponent<Node>().val = curNode.GetComponent<Node>().val + Vector2.Distance(curNode.transform.position, leftNode.transform.position);
                    queue.Add(leftNode);
                }
            }
            if (rightNode != null && !used.Contains(rightNode))
            {
                if (queue.Contains(rightNode))
                    queue[queue.IndexOf(rightNode)].GetComponent<Node>().val = Mathf.Min(curNode.GetComponent<Node>().val + Vector2.Distance(curNode.transform.position, rightNode.transform.position), queue[queue.IndexOf(rightNode)].GetComponent<Node>().val);
                else
                {
                    rightNode.GetComponent<Node>().val = curNode.GetComponent<Node>().val + Vector2.Distance(curNode.transform.position, rightNode.transform.position);
                    queue.Add(rightNode);
                }
            }
            if (upNode != null && !used.Contains(upNode))
            {
                if (queue.Contains(upNode))
                    queue[queue.IndexOf(upNode)].GetComponent<Node>().val = Mathf.Min(curNode.GetComponent<Node>().val + Vector2.Distance(curNode.transform.position, upNode.transform.position), queue[queue.IndexOf(upNode)].GetComponent<Node>().val);
                else
                {
                    upNode.GetComponent<Node>().val = curNode.GetComponent<Node>().val + Vector2.Distance(curNode.transform.position, upNode.transform.position);
                    queue.Add(upNode);
                }
            }
            if (downNode != null && !used.Contains(downNode))
            {
                if (queue.Contains(downNode))
                    queue[queue.IndexOf(downNode)].GetComponent<Node>().val = Mathf.Min(curNode.GetComponent<Node>().val + Vector2.Distance(curNode.transform.position, downNode.transform.position), queue[queue.IndexOf(downNode)].GetComponent<Node>().val);
                else
                {
                    downNode.GetComponent<Node>().val = curNode.GetComponent<Node>().val + Vector2.Distance(curNode.transform.position, downNode.transform.position);
                    queue.Add(downNode);
                }
            }

            used.Add(curNode);
            queue.Remove(curNode);
            queue.Sort(SortByNodeAscend);
        }

        if (!used.Contains(endNode) || govno >= 100)
        {
            GameManager.FixThisShit();
            return this.gameObject;
        }

        used.Sort(SortByNodeDescend);
        GameObject currentNode = endNode;
        List<GameObject> path = new List<GameObject>();
        path.Add(currentNode);

        int mocha = 0;
        while (used[used.IndexOf(currentNode)].GetComponent<Node>().val != 0 && mocha < 100)
        {
            mocha++;
            GameObject minNode = null;

            //от этой мерзости однозначно надо избавиться
            GameObject leftNode = currentNode.GetComponent<Node>().LeftNode;
            GameObject rightNode = currentNode.GetComponent<Node>().RightNode;
            GameObject upNode = currentNode.GetComponent<Node>().UpNode;
            GameObject downNode = currentNode.GetComponent<Node>().DownNode;

            if (leftNode != null && minNode == null)
                minNode = leftNode;
            else if (leftNode != null)
                minNode = used[used.IndexOf(minNode)].GetComponent<Node>().val < used[used.IndexOf(leftNode)].GetComponent<Node>().val ? minNode : leftNode;
            if (rightNode != null && minNode == null)
                minNode = rightNode;
            else if (rightNode != null)
                minNode = used[used.IndexOf(minNode)].GetComponent<Node>().val < used[used.IndexOf(rightNode)].GetComponent<Node>().val ? minNode : rightNode;
            if (upNode != null && minNode == null)
                minNode = upNode;
            else if (upNode != null)
                minNode = used[used.IndexOf(minNode)].GetComponent<Node>().val < used[used.IndexOf(upNode)].GetComponent<Node>().val ? minNode : upNode;
            if (downNode != null && minNode == null)
                minNode = downNode;
            else if (downNode != null)
                minNode = used[used.IndexOf(minNode)].GetComponent<Node>().val < used[used.IndexOf(downNode)].GetComponent<Node>().val ? minNode : downNode;
            
            currentNode = minNode;
            path.Add(currentNode);
        }
        path.Sort(SortByNodeAscend);
        VisualizePath(ref path, color);
        Destroy(startNode);
        Destroy(endNode);
        GameManager.ConnectNodes();
        GameManager.ResetNodes();
        return path[1];
    }

    int SortByNodeAscend(GameObject n1, GameObject n2)
    {
        float f1 = n1.GetComponent<Node>().val, f2 = n2.GetComponent<Node>().val;
        if (f1 > f2)
            return 1;
        if (f1 < f2)
            return -1;
        return 0;
    }
    int SortByNodeDescend(GameObject n1, GameObject n2)
    {
        float f1 = n1.GetComponent<Node>().val, f2 = n2.GetComponent<Node>().val;
        if (f1 > f2)
            return -1;
        if (f1 < f2)
            return 1;
        return 0;
    }

    bool ghostInsideNode = false;
    GameObject SpawnNode(Vector2 nodePosition, bool ghostNode)
    {
        GameObject node = Instantiate(nodePrefab, nodePosition, Quaternion.identity);
        node.transform.SetParent(GameManager.shit);
        node.GetComponent<Collider2D>().enabled = false;

        if (ghostNode)
        {
            RaycastHit2D testRay = Physics2D.Raycast(nodePosition, new Vector2(-1, 0));
            if (testRay.collider != null && testRay.transform.position == (Vector3)nodePosition && testRay.collider.tag == "Node")
                ghostInsideNode = true;
            if (testRay.collider != null && (testRay.collider.tag == "Wall" || ((testRay.transform.position.x+0.1f) < nodePosition.x) ) )
                ghostInsideNode = false;
        }

        RaycastHit2D leftRay = Physics2D.Raycast(nodePosition, new Vector2(-1, 0));
        RaycastHit2D rightRay = Physics2D.Raycast(nodePosition, new Vector2(1, 0));
        RaycastHit2D upRay = Physics2D.Raycast(nodePosition, new Vector2(0, 1));
        RaycastHit2D downRay = Physics2D.Raycast(nodePosition, new Vector2(0, -1));
        if (ghostInsideNode || !ghostNode)
        {
            leftRay = Physics2D.Raycast(new Vector2(nodePosition.x - 0.11f, nodePosition.y), new Vector2(-1, 0));
            rightRay = Physics2D.Raycast(new Vector2(nodePosition.x + 0.11f, nodePosition.y), new Vector2(1, 0));
            upRay = Physics2D.Raycast(new Vector2(nodePosition.x, nodePosition.y + 0.11f), new Vector2(0, 1));
            downRay = Physics2D.Raycast(new Vector2(nodePosition.x, nodePosition.y - 0.11f), new Vector2(0, -1));
        }

        if (leftRay.collider != null && (leftRay.collider.tag == "Node" || leftRay.collider.tag == "Special Node" && CheckRowY(leftRay.collider.gameObject) && CheckRowY(node) ))
        {
            node.GetComponent<Node>().LeftNode = leftRay.collider.gameObject;
            node.GetComponent<Node>().LeftNode.GetComponent<Node>().RightNode = node;
            if (!ghostNode)
                Debug.Log(leftRay.collider.tag);
        }
        if (rightRay.collider != null && rightRay.collider != leftRay.collider && (rightRay.collider.tag == "Node" || rightRay.collider.tag == "Special Node" && CheckRowY(rightRay.collider.gameObject) && CheckRowY(node)))
        {
            node.GetComponent<Node>().RightNode = rightRay.collider.gameObject;
            node.GetComponent<Node>().RightNode.GetComponent<Node>().LeftNode = node;
        }
        if (upRay.collider != null && upRay.collider != leftRay.collider && (upRay.collider.tag == "Node" || upRay.collider.tag == "Special Node" && CheckRowX(upRay.collider.gameObject) && CheckRowX(node)))
        {
            node.GetComponent<Node>().UpNode = upRay.collider.gameObject;
            node.GetComponent<Node>().UpNode.GetComponent<Node>().DownNode = node;
        }
        if (downRay.collider != null && downRay.collider != leftRay.collider && (downRay.collider.tag == "Node" || downRay.collider.tag == "Special Node" && CheckRowX(downRay.collider.gameObject) && CheckRowX(node)))
        {
            node.GetComponent<Node>().DownNode = downRay.collider.gameObject;
            node.GetComponent<Node>().DownNode.GetComponent<Node>().UpNode = node;
        }
        node.GetComponent<Collider2D>().enabled = true;
        return node;
    }
    
    //можно внести в условие
    bool CheckRowX(GameObject node)
    {
        if (xNodeRow.Contains(node.transform.position.x))
            return true;
        return false;
    }
    bool CheckRowY(GameObject node)
    {
        if (yNodeRow.Contains(node.transform.position.y))
            return true;
        return false;
    }

    //есть ли смысл в с# передавать параметр по ссылке?
    void VisualizePath(ref List<GameObject> path, Color color)
    {
        for (int i = 0; i < path.Count - 1; i++)
            Debug.DrawLine(path[i].transform.position, path[i+1].transform.position, color, Time.deltaTime);
    }
}
