using UnityEngine;

public class RedGhost : Ghost
{
    void Start()
    {
        player = FindObjectOfType<Player>();
        color = Color.red;
        timer = GetComponent<Timer>();
        pathBuilder = GetComponent<PathBuilder>();
        Init();
    }

    public void Init()
    {
        nextNode = cageNodeThree.gameObject;
        timer.Set(20);
        behaviourMode = "Chase";
    }

    protected override Transform HandleChase()
    {
        if (timer.Out())
        {
            timer.Set(scatterTime);
            behaviourMode = "Scatter";
            currentNode = scatterTarget;
        }
        return ChooseClosestNode(player.transform.position).transform;
    }
}
