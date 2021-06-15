using UnityEngine;

public class OrangeGhost : Ghost
{
    public Transform scatterTargetTwo;

    void Start()
    {
        player = FindObjectOfType<Player>();
        color = new Color(1,127/255f,80/255f);
        timer = GetComponent<Timer>();
        pathBuilder = GetComponent<PathBuilder>();
        Init();
    }

    public void Init()
    {
        behaviourMode = "Caged";
        timer.Set(5);
        currentNode = cageNodeOne;
    }

    protected override Transform HandleChase()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > 4 && timer.Out())
            return player.transform;
        if (Vector2.Distance(transform.position, player.transform.position) <= 4)
            timer.Set(1);
        if (currentNode.position == transform.position && currentNode == scatterTarget)
            currentNode = scatterTargetTwo;
        if (currentNode.position == transform.position && currentNode == scatterTargetTwo)
            currentNode = scatterTarget;
        return currentNode;
    }
}
