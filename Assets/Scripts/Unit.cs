﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(HoverEffect))]
public class Unit : MonoBehaviour
{
    [SerializeField] [Range(1,7)] 
    private int moveRange = 3;

    [SerializeField] private float moveSpeed = 3;

    public bool selected = false;//表示当前单位是否被选中

    public bool hasMoved = false;//表示当前回合当前单位是否移动过

    public int playerNumber;//表示当前unit是蓝色方/红色方

    
    public Color squareColor;

    [SerializeField]private int attackRange;
    List<Unit> enemiesInRange = new List<Unit>();
    public bool hasAttacked;

    public GameObject weaponIcon;

    public int health;
    public int attackDamage;
    public int defenseDamage;
    public int armor;

    public DamageIcon damageIcon;
    public GameObject deathEffect;
    private Animator camAnim;
    private void Start(){
        camAnim = Camera.main.GetComponent<Animator>();
    }

    private void OnMouseDown(){
        //初始化攻击指示器
        ResetWeaponIcon();

        if(playerNumber == GameManager.instance.playerTurn){
            GameManager.instance.selectedUnit = this;
            selected = true;
            ResetTiles();

            GetEnemies();
            //如果已经移动过了，就不能显示能移动的tile
            ShowWalkableTiles();
        }

        //被攻击

        //
        Collider2D col  = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition),0.15f);
        Unit unit  = col.GetComponent<Unit>();
        if(GameManager.instance.selectedUnit != null){
            if(GameManager.instance.selectedUnit.enemiesInRange.Contains(unit) && GameManager.instance.selectedUnit.hasAttacked == false){
                GameManager.instance.selectedUnit.Attack(unit);
            }
        }
    }

    private void Attack(Unit enemy){
        
        camAnim.SetTrigger("shake");
        
        hasAttacked = true;
        //实际造成的总伤害 = 伤害-护盾抵消
        int enemyDamage = attackDamage - enemy.armor;

        int myDamage = enemy.defenseDamage - armor;

        if(enemyDamage >= 1){
            DamageIcon instance = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            instance.Setup(enemyDamage);
            enemy.health -= enemyDamage;
        }

        if(myDamage >= 1){
            DamageIcon instance = Instantiate(damageIcon, transform.position, Quaternion.identity);
            instance.Setup(myDamage);
            health -= myDamage;
        }

        if(enemy.health <=0){
            Instantiate(deathEffect,enemy.transform.position,Quaternion.identity);
            Destroy(enemy.gameObject);
            ShowWalkableTiles();
        }

        if(health<=0){
            Instantiate(deathEffect,transform.position,Quaternion.identity);
            ResetTiles();
            Destroy(gameObject);
        }
    }
    private void ShowWalkableTiles(){
        if(!hasMoved){
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
        
    }
    //这个方法会在【Tile】脚本中OnMouseDown函数中被【调用】
    public void Move(Transform _transform){
        StartCoroutine(MoveCo(_transform));
        hasMoved = true;
    }

    IEnumerator MoveCo(Transform _transform){
        GetComponent<Animator>().SetBool("isRunning",true);

        while(transform.position.x != _transform.position.x){
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(_transform.position.x,transform.position.y),moveSpeed * Time.deltaTime);
            //yield return null;
            yield return new WaitForSeconds(0);
        }

        while(transform.position.y != _transform.position.y){
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x,_transform.position.y),moveSpeed * Time.deltaTime);
            yield return null;
        }
        GetComponent<Animator>().SetBool("isRunning",false);
        ResetTiles();
        ShowWalkableTiles();
        ResetWeaponIcon();
        GetEnemies();
    }

    public void ResetTiles(){
        for(int i = 0;i<GameManager.instance.tiles.Length;i++){
            GameManager.instance.tiles[i].ResetTile();
        }
    }

    public void GetEnemies(){

        enemiesInRange.Clear();

        foreach(Unit unit in FindObjectsOfType<Unit>()){
            float distX = Mathf.Abs(transform.position.x - unit.transform.position.x);
            float distY = Mathf.Abs(transform.position.y - unit.transform.position.y);
            if(distX + distY <= attackRange){
                //当敌人存在自己的攻击距离内
                if(unit.playerNumber != GameManager.instance.playerTurn && !hasAttacked){
                    enemiesInRange.Add(unit);
                    unit.weaponIcon.SetActive(true);
                }   
            }
        }
    }

    public void ResetWeaponIcon(){
        foreach(Unit unit in FindObjectsOfType<Unit>()){
            unit.weaponIcon.SetActive(false);
        }
    }
}
