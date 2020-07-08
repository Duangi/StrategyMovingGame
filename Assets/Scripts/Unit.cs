using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] [Range(1,7)] 
    private int moveRange = 3;

    [SerializeField] private float moveSpeed = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown(){
        GameManager.instance.selectedUnit = this;
        ResetTiles();
        ShowWalkableTiles();
    }

    private void ShowWalkableTiles(){
        //判断当前角色和瓦片之间  x轴距离和y轴距离  x+y<=moveRange 是否成立，如果成立，则可以移动到该位置
        for(int i =0;i<GameManager.instance.tiles.Length;i++){
            float distX = Mathf.Abs(transform.position.x - GameManager.instance.tiles[i].transform.position.x);
            float distY = Mathf.Abs(transform.position.y - GameManager.instance.tiles[i].transform.position.y);
            if(distX + distY <= moveRange){
                //当瓦片上没有障碍物，才能移动
                if(!GameManager.instance.tiles[i].hasObstacles){
                    string s = "I'm in "+ (transform.position.x - GameManager.instance.tiles[i].transform.position.x) + ","+ (transform.position.y - GameManager.instance.tiles[i].transform.position.y) +" I can walk to";
                    //Debug.Log(s);
                    GameManager.instance.tiles[i].canWalk = true;
                    GameManager.instance.tiles[i].HighLightTile();
                }
                else GameManager.instance.tiles[i].canWalk = false;
            }
            else GameManager.instance.tiles[i].canWalk = false;
        }
    }
    //这个方法会在【Tile】脚本中OnMouseDown函数中被【调用】
    public void Move(Transform _transform){
        StartCoroutine(MoveCo(_transform));
        
    }

    IEnumerator MoveCo(Transform _transform){
        while(transform.position.x != _transform.position.x){
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(_transform.position.x,transform.position.y),moveSpeed * Time.deltaTime);
            //yield return null;
            yield return new WaitForSeconds(0);
        }

        while(transform.position.y != _transform.position.y){
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x,_transform.position.y),moveSpeed * Time.deltaTime);
            yield return null;
        }

        ResetTiles();
        ShowWalkableTiles();
    }

    public void ResetTiles(){
        for(int i = 0;i<GameManager.instance.tiles.Length;i++){
            GameManager.instance.tiles[i].ResetTile();
        }
    }
}
