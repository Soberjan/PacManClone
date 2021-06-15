using UnityEngine;

public class Fruit : MonoBehaviour
{
    static public string fruitState;
    static public Timer t;
    public float invisibleTime, freshTime, rotTime; 
    public GameObject fruitPrefab;
    public GameObject fruit;
    
    void Start()
    {
        t = GetComponent<Timer>();
        t.Set(5);
        fruitState = "Invisible";
    }

    void Update()
    {
        if (t.Out())
        {
            if (fruitState == "Invisible")
            {
                t.Set(freshTime);
                fruitState = "Fresh";
                fruit = null;
                fruit = Instantiate(fruitPrefab, new Vector2(0,-2), Quaternion.identity);
            }
            else if (fruitState == "Fresh")
            {
                t.Set(invisibleTime); //добавить переход в рот
                fruitState = "Invisible";
                Destroy(fruit);
            }
        }
    }
}
