using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeLeft = 0;

    void Update()
    {
        timeLeft -= Time.deltaTime;
    }

    public bool Out()
    {
        return (timeLeft <= 0);
    }
    public void Set(float seconds)
    {
        timeLeft = seconds;
    }
}
