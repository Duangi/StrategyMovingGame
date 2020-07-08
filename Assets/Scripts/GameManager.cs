using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Tile[] tiles;

    public Unit selectedUnit;//被选中的角色
    private void Awake(){
        //如果一开始GameManager未赋值，则将该gameObject作为manager
        if(instance == null){
            instance = this;
        }

        else {
            //确保manager只有一个
            if(instance != this){
                Destroy(gameObject);
            }
        }
        //保证场景切换不会删掉manager
        DontDestroyOnLoad(gameObject);
    }
}
