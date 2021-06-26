using UnityEngine;

public class PinkGhost : Ghost
{
    public Transform pinkTarget;

    void Start()
    {
        player = FindObjectOfType<Player>();
        color = new Color(255/255f,192/255f,203/255f);
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
        if (currentNode != pinkTarget)
            currentNode = pinkTarget;

        Vector2 targetPos = SetTarget();
        currentNode.position = targetPos;
        pinkTarget.position = targetPos;

        return ChooseClosestNode(targetPos).transform;
    }

    Vector2 SetTarget()
    {
        if (!player.isMoving)
            return player.transform.position;

        Vector2 targetPos = new Vector2(Mathf.Round(player.transform.position.x), Mathf.Round(player.transform.position.y));
        if (player.moveDir == Vector2.up)
            targetPos.y += 4;
        if (player.moveDir == Vector2.down)
            targetPos.y -= 4;
        if (player.moveDir == Vector2.left)
            targetPos.x -= 4;
        if (player.moveDir == Vector2.right)
            targetPos.x += 4;

        //Случай, когда второй конец отрезка за концом карты
        bool mirrored = false;
        if (targetPos.x > 15)
        {
            targetPos.x -= 8;
            mirrored = true;
        }
        else if (targetPos.x < -15)
        {
            targetPos.x += 8;
            mirrored = true;
        }
        else if (targetPos.y > 9)
        {
            targetPos.y -= 8;
            mirrored = true;
        }
        else if (targetPos.y < -9)
        {
            targetPos.y += 8;
            mirrored = true;
        }
        
        //проверяеем, не смотрим ли в стену
        Collider2D info = Physics2D.OverlapPoint(targetPos);
        while (info != null && info.tag == "Wall")
        {
            if (mirrored)
            {
                if (player.moveDir == Vector2.up)
                    targetPos.y += 1;
                if (player.moveDir == Vector2.down)
                    targetPos.y -= 1;
                if (player.moveDir == Vector2.left)
                    targetPos.x -= 1;
                if (player.moveDir == Vector2.right)
                    targetPos.x += 1;
            }
            else
            {
                if (player.moveDir == Vector2.up)
                    targetPos.y -= 1;
                if (player.moveDir == Vector2.down)
                    targetPos.y += 1;
                if (player.moveDir == Vector2.left)
                    targetPos.x += 1;
                if (player.moveDir == Vector2.right)
                    targetPos.x -= 1;
            }
            info = Physics2D.OverlapPoint(targetPos);
        }

        return targetPos;
    }
}
