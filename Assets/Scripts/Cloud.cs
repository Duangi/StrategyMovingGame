using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    private float speed;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    // Start is called before the first frame update
    void Start()
    {
        //随机一个速度
        speed = Random.Range(minSpeed,maxSpeed);
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed *Time.deltaTime);

        //如果移动到左边界,在右边重生
        if(transform.position.x < minX){
            Vector2 newPos = new Vector2(maxX,transform.position.y);
            transform.position = newPos;
        }
    }
}
