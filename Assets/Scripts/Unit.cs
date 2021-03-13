using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(HoverEffect))]
public class Unit : MonoBehaviour
{
    [SerializeField] [Range(0,7)] 
    private int moveRange = 3;

    [SerializeField] private float moveSpeed = 3;

    public bool selected = false;//表示当前单位是否被选中

    public bool hasMoved = false;//表示当前回合当前单位是否移动过
    public bool isMoving;

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

    //king的血量
    [SerializeField] private bool isKing;
    [SerializeField] private Text kingHealth;

    public GameObject WinFlag;

    //点击音效
    private AudioSource audioSource;
    private void Start(){
        audioSource = GetComponent<AudioSource>();
        camAnim = Camera.main.GetComponent<Animator>();
        UpdateKingHealth();
    }

    private void OnMouseDown(){
        //只有当前是你的回合，才能点击unit产生反应
        if(GameManager.instance.myPlayer == GameManager.instance.playerTurn){
            //初始化攻击指示器
            ResetWeaponIcon();
            
            if(playerNumber == GameManager.instance.playerTurn){
                GameManager.instance.selectedUnit = this;
                selected = true;
                audioSource.Play();
                ResetTiles();

                GetEnemies();
                //如果已经移动过了，就不能显示能移动的tile
                ShowWalkableTiles();
            }


            //攻击敌人
            Collider2D col  = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition),0.15f);
            Unit unit  = col.GetComponent<Unit>();
            if(GameManager.instance.selectedUnit != null){
                if(GameManager.instance.selectedUnit.enemiesInRange.Contains(unit) && GameManager.instance.selectedUnit.hasAttacked == false){
                    GameManager.instance.selectedUnit.Attack(unit);
                    string temp = "attack," + FindObjectOfType<Client>().matchNumber +","+ GameManager.instance.selectedUnit.transform.position.x +","+ GameManager.instance.selectedUnit.transform.position.y+","+unit.transform.position.x+","+unit.transform.position.y+",";
                    FindObjectOfType<Client>().sendMsgFromGame(temp);
                }
            }
        }
    }

    public void Attack(Unit enemy){
        
        camAnim.SetTrigger("shake");
        
        hasAttacked = true;
        //实际造成的总伤害 = 伤害-护盾抵消
        int enemyDamage = attackDamage - enemy.armor;

        int myDamage = enemy.defenseDamage - armor;
        
        
        //对敌人造成伤害
        if(enemyDamage >= 1){
            DamageIcon instance = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            instance.Setup(enemyDamage);
            enemy.health -= enemyDamage;
            enemy.UpdateKingHealth();
        }
        
        //反伤（如果是弓箭手，敌人距离超过1，则不会受到反伤）
        float distX = Mathf.Abs(transform.position.x - enemy.transform.position.x);
        float distY = Mathf.Abs(transform.position.y - enemy.transform.position.y);
        if(this.tag == "Archer" && ((distX+distY)>1) ){
            
        }
        else if(myDamage >= 1){
            DamageIcon instance = Instantiate(damageIcon, transform.position, Quaternion.identity);
            instance.Setup(myDamage);
            health -= myDamage;
            UpdateKingHealth();
        }

        if(enemy.health <=0){
            Instantiate(deathEffect,enemy.transform.position,Quaternion.identity);
            Destroy(enemy.gameObject);
            ShowWalkableTiles();
            GameManager.instance.RemoveStatsPanel(enemy);

            if(enemy.isKing){
                enemy.WinFlag.SetActive(true);
            }
        }

        if(health<=0){
            Instantiate(deathEffect,transform.position,Quaternion.identity);
            ResetTiles();
            Destroy(gameObject);
            GameManager.instance.RemoveStatsPanel(this);
            ShowWalkableTiles();

            if(isKing){
                WinFlag.SetActive(true);
            }
        }
        GameManager.instance.UpdateStatsPanel();

        //攻击完成之后立马刷新可以移动的位置
        ShowWalkableTiles();

        ResetWeaponIcon();
    }
    //king的血量更新之后，每次都会在ui画布上更新
    private void UpdateKingHealth(){
        if(isKing){
            kingHealth.text = health.ToString();
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
    public void Move(Vector2 position){
        StartCoroutine(MoveCo(position));
        hasMoved = true;
    }
    IEnumerator MoveCo(Vector2 position){

        //切换running状态
        //GetComponent<Animator>().SetBool("isRunning",true);
        isMoving = true;

        //开始移动时，将stats暂时先关闭，移动完成之后再打开
        GameManager.instance.DisableStatsPanel();

        while(transform.position.x != position.x){
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(position.x,transform.position.y),moveSpeed * Time.deltaTime);
            //yield return null;
            yield return new WaitForSeconds(0);
        }

        while(transform.position.y != position.y){
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x,position.y),moveSpeed * Time.deltaTime);
            yield return null;
        }
        //run动画结束
        //GetComponent<Animator>().SetBool("isRunning",false);

        
        //重新打开状态栏
        if(GameManager.instance.viewedUnit != null){
            if(GameManager.instance.viewedUnit.Equals(this)){
                GameManager.instance.ToggleStatsPanel(this);
            }
        }
        isMoving = false;
        
        
        ResetTiles();
        ShowWalkableTiles();
        ResetWeaponIcon();
        GetEnemies();

        
    }
    IEnumerator MoveCo(Transform _transform){
        //记录初始位置，然后直接给服务器发送移动消息
        string msg = "position,"+FindObjectOfType<Client>().matchNumber+","+transform.position.x+","+transform.position.y+","+_transform.position.x+","+_transform.position.y+",";
        
        FindObjectOfType<Client>().sendMsgFromGame(msg);


        //切换running状态
        //GetComponent<Animator>().SetBool("isRunning",true);
        isMoving = true;

        //开始移动时，将stats暂时先关闭，移动完成之后再打开
        GameManager.instance.DisableStatsPanel();

        while(transform.position.x != _transform.position.x){
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(_transform.position.x,transform.position.y),moveSpeed * Time.deltaTime);
            //yield return null;
            yield return new WaitForSeconds(0);
        }

        while(transform.position.y != _transform.position.y){
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x,_transform.position.y),moveSpeed * Time.deltaTime);
            yield return null;
        }
        //run动画结束
        //GetComponent<Animator>().SetBool("isRunning",false);

        
        //重新打开状态栏
        if(GameManager.instance.viewedUnit != null){
            if(GameManager.instance.viewedUnit.Equals(this)){
                GameManager.instance.ToggleStatsPanel(this);
            }
        }
        isMoving = false;
        
        
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

    private void OnMouseOver(){
        if(Input.GetMouseButtonDown(1)){
            GameManager.instance.ToggleStatsPanel(this);
        }
    }
}
