using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIcon : MonoBehaviour
{
    public Sprite[] damageSprites;

    public float lifetime;

    public GameObject effect;
    private void Start(){
        //延时执行函数
        Invoke("Destruction", lifetime);
    } 
    public void Setup(int damage){
        GetComponentInChildren<SpriteRenderer>().sprite = damageSprites[damage - 1];
    }

    void Destruction(){
        Destroy(gameObject);
        Instantiate(effect,transform.position,Quaternion.identity);
    }
}
