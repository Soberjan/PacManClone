using UnityEngine;

public class BlueGhost : Ghost
{
    public Transform blueTarget;
    Transform redGhost;

    void Start()
    {
        pathBuilder = GetComponent<PathBuilder>();
        timer = GetComponent<Timer>();
        redGhost = FindObjectOfType<RedGhost>().transform;
        player = FindObjectOfType<Player>();
        color = new Color(0,66/255f,1);
        Init();
    }

    public void Init()
    {
        timer.Set(15);
        behaviourMode = "Caged";
        currentNode = cageNodeOne;
    }

    protected override Transform HandleChase()
    {
        if (timer.Out())
        {
            timer.Set(scatterTime);
            behaviourMode = "Scatter";
            currentNode = scatterTarget;
        }
        if (currentNode != blueTarget)
            currentNode = blueTarget;
        
        Vector2 targetPos = SetTarget();
        currentNode.position = targetPos;
        blueTarget.position = targetPos;
        return currentNode;
    }

    Vector2 SetTarget()
    {
        Vector2 target = new Vector2();

        float distX = Mathf.Round(Mathf.Abs(player.transform.position.x - redGhost.position.x));
        target.x = Mathf.Round( player.transform.position.x > redGhost.position.x ? player.transform.position.x + distX :  player.transform.position.x - distX);
        float distY = Mathf.Round(Mathf.Abs(player.transform.position.y - redGhost.position.y));
        target.y = Mathf.Round( player.transform.position.y > redGhost.position.y ? player.transform.position.y + distY :  player.transform.position.y - distY);

        if (!WithinBoundaries(target))
        {
            distX = Mathf.Abs( (player.transform.position.x - redGhost.position.x) / 2);
            target.x = Mathf.Round( player.transform.position.x > redGhost.position.x ? player.transform.position.x - distX :  player.transform.position.x + distX);
            distY = Mathf.Abs( (player.transform.position.y - redGhost.position.y) / 2);
            target.y = Mathf.Round( player.transform.position.y > redGhost.position.y ? player.transform.position.y - distY :  player.transform.position.y + distY);
        }

        //БАГ - СМ. координаты (15, 6)
        Collider2D info = Physics2D.OverlapPoint(target);
        if (info != null && info.tag == "Wall")
        {
            Collider2D infoLeft = Physics2D.OverlapPoint(new Vector2(target.x - 1, target.y));
            Collider2D infoRight = Physics2D.OverlapPoint(new Vector2(target.x + 1, target.y));
            Collider2D infoUp = Physics2D.OverlapPoint(new Vector2(target.x, target.y + 1));
            Collider2D infoDown = Physics2D.OverlapPoint(new Vector2(target.x, target.y - 1));
            Collider2D infoLU = Physics2D.OverlapPoint(new Vector2(target.x - 1, target.y + 1));
            Collider2D infoRU = Physics2D.OverlapPoint(new Vector2(target.x + 1, target.y + 1));
            Collider2D infoLD = Physics2D.OverlapPoint(new Vector2(target.x - 1, target.y - 1));
            Collider2D infoRD = Physics2D.OverlapPoint(new Vector2(target.x + 1, target.y - 1));

            //Debug.Log(target);

            if (infoLeft == null && WithinBoundaries(new Vector2(target.x - 1, target.y)) || infoLeft != null && infoLeft.tag != "Wall")
                target.x -= 1;
            else if (infoRight == null && WithinBoundaries(new Vector2(target.x + 1, target.y)) || infoRight != null && infoRight.tag != "Wall")
                target.x += 1;
            else if (infoUp == null && WithinBoundaries(new Vector2(target.x, target.y + 1)) || infoUp != null && infoUp.tag != "Wall")
                target.y += 1;
            else if (infoDown == null  && WithinBoundaries(new Vector2(target.x, target.y - 1)) || infoDown != null && infoDown.tag != "Wall")
                target.y -= 1;
            else if (infoLU == null  && WithinBoundaries(new Vector2(target.x - 1, target.y + 1)) || infoLU != null && infoLU.tag != "Wall")
            {
                target.x -= 1;
                target.y += 1;
            }
            else if (infoRU == null  && WithinBoundaries(new Vector2(target.x + 1, target.y + 1)) || infoRU != null && infoRU.tag != "Wall")
            {
                target.x += 1;
                target.y += 1;
            }
            else if (infoLD == null  && WithinBoundaries(new Vector2(target.x - 1, target.y - 1)) || infoLD != null && infoLD.tag != "Wall")
            {
                target.x -= 1;
                target.y -= 1;
            }
            else if (infoRD == null  && WithinBoundaries(new Vector2(target.x + 1, target.y - 1)) || infoRD != null && infoDown.tag != "Wall")
            {
                target.x += 1;
                target.y -= 1;
            }
            
        }

        return target;
    }

    bool WithinBoundaries(Vector2 t)
    {
        return (t.x <= 15 && t.x >= -15 && t.y <= 9 && t.y >= -9);
    }
}
