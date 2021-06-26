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

    public GameObject BuildPathToTarget(GameObject startNode, GameObject endNode, Color color)
    {
        List<GameObject> queue = new List<GameObject>();
        List<GameObject> used = new List<GameObject>();

        startNode.GetComponent<Node>().val = 0;
        queue.Add(startNode);
        int govno = 0;
        while (queue.Count != 0 && govno < 100)
        {
            govno++;
            GameObject curNode = queue[0];

            foreach(Node n in curNode.GetComponent<Node>().neighbors)
                if (!used.Contains(n.gameObject))
                {
                    if (queue.Contains(n.gameObject) && n.tag == "Teleporter" && curNode.tag == "Teleporter")
                        n.gameObject.GetComponent<Node>().val = Mathf.Min(curNode.GetComponent<Node>().val, n.gameObject.GetComponent<Node>().val);
                    else if (queue.Contains(n.gameObject))
                        n.gameObject.GetComponent<Node>().val = Mathf.Min(curNode.GetComponent<Node>().val + Vector2.Distance(curNode.transform.position, n.gameObject.transform.position), n.gameObject.GetComponent<Node>().val);
                    else if (!queue.Contains(n.gameObject) && n.tag == "Teleporter" && curNode.tag == "Teleporter")
                    {
                        n.gameObject.GetComponent<Node>().val = curNode.GetComponent<Node>().val;
                        queue.Add(n.gameObject);                            
                    }
                    else if (!queue.Contains(n.gameObject))
                    {
                        n.gameObject.GetComponent<Node>().val = curNode.GetComponent<Node>().val + Vector2.Distance(curNode.transform.position, n.gameObject.transform.position);
                        queue.Add(n.gameObject);
                    }
                }

            used.Add(curNode);
            queue.Remove(curNode);
            queue.Sort(SortByNodeAscend);
        }

        used.Sort(SortByNodeDescend);
        GameObject currentNode = endNode;
        List<GameObject> path = new List<GameObject>();
        path.Add(currentNode);
        int mocha = 0;
        while (currentNode.GetComponent<Node>().val != 0 && mocha < 100)
        {
            mocha++;
            GameObject minNode = null;

            foreach (Node n in currentNode.GetComponent<Node>().neighbors)
            {
                if (minNode == null)
                    minNode = n.gameObject;
                else
                    minNode = minNode.GetComponent<Node>().val < n.val ? minNode : n.gameObject;
            }
            
            currentNode = minNode;
            path.Add(currentNode);
        }
        //path.Sort(SortByNodeAscend);

        if (!path.Contains(startNode))
            path.Add(startNode);

        // foreach (GameObject go in path)
        //     Debug.Log(go.transform.position);

        GameManager.ResetNodes();

        VisualizePath(path, color);
        if (path.Count == 1)
        {
            Debug.Log("Pizda");
            return path[0];
        }
        return path[path.Count-2];
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

    //есть ли смысл в с# передавать параметр по ссылке?
    void VisualizePath(List<GameObject> path, Color color)
    {
        for (int i = 0; i < path.Count - 1; i++)
            if (path[i+1].tag != "Teleporter")
                Debug.DrawLine(path[i].transform.position, path[i+1].transform.position, color, 0.5f);
    }
}
