using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    
    [SerializeField] private Sprite[] sprites;
    public bool hasObstacles;

    public bool canWalk;

    public LayerMask obLayerMask;
    public LayerMask playerLayerMask;
    public Color highLightColor;
    // Start is called before the first frame update
    void Start()
    {
        //为了获取每个tile的sprite组件
         spriteRenderer = GetComponent<SpriteRenderer>();
        //给每一个tile赋值一个随机的sprite
         int randomNum = Random .Range(0,sprites.Length);
         spriteRenderer.sprite = sprites[randomNum];
    }

    // Update is called once per frame
    void Update()
    {
        CheckObstacles();
        CheckPlayers();
    }

    //鼠标进入瓦片，放大，并调整渲染层顺序
    private void OnMouseEnter(){
        transform.localScale += Vector3.one *0.02f;
        spriteRenderer.sortingOrder = 25;
    }

    private void OnMouseExit(){
        transform.localScale -= Vector3.one *0.02f;
        
        spriteRenderer.sortingOrder = 0;
    }

    //如果当有一个player行走到tile上面时，应该将这个tile的layer设置成IgnoreRaycast，避免collider重叠
    private void CheckPlayers(){
        Collider2D collider =  Physics2D.OverlapCircle(transform.position, spriteRenderer.bounds.extents.x,playerLayerMask);
        //如果有角色在tile上面，则该tile暂时无视射线
        if(collider != null){
            gameObject.layer = 2; 
        }
        else{
            gameObject.layer = LayerMask.GetMask("Default");
        }
    } 
    private void CheckObstacles(){
        
        Collider2D collider =  Physics2D.OverlapCircle(transform.position, spriteRenderer.bounds.extents.x,obLayerMask);
        //如果检测到了碰撞->无法行走
        if(collider != null){
            hasObstacles = true;
        }
        else {
            hasObstacles = false;
        }
    }
    
    public void HighLightTile(){
        if(canWalk){
            spriteRenderer.color = highLightColor;
        }
        else 
            spriteRenderer.color = Color.white;
    }

    public void ResetTile(){
        spriteRenderer.color = Color.white;
    }
    public void OnMouseDown(){
        if(GameManager.instance.selectedUnit != null){
            if(!GameManager.instance.selectedUnit.hasMoved && canWalk){
                GameManager.instance.selectedUnit.Move(this.transform);
            }
        }
        
    }

    
}
