using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;//gm实例
    
    public Tile[] tiles;//存储所有的tile
    
    [HideInInspector]public Unit selectedUnit;//被选中的角色

    [HideInInspector]public int playerTurn = 1;

    [SerializeField] private GameObject selectedUnitSquare;//selectedUnit高亮提示框
    
    //切换回合
    [SerializeField] private Image playerIndicator;//表示当前回合玩家的图片
    [SerializeField] private Sprite player1Indicator;//玩家1图片
    [SerializeField] private Sprite player2Indicator;//玩家2图片

    //玩家金币
    public int player1Gold = 10;
    public int player2Gold = 10;
    [SerializeField] private Text player1GoldText;
    [SerializeField] private Text player2GoldText;

    //购买
    public BarrackItem purchasedItem;

    //状态栏
    public GameObject statsPanel;
    public Vector2 statsPanelShift;
    public Unit viewedUnit;//当前查看的Unit
    [SerializeField] private Text healthText;
    [SerializeField] private Text armorText;
    [SerializeField] private Text attackDamageText;
    [SerializeField] private Text defenseDamageText;

    [SerializeField] private Button TurnButton;

    [HideInInspector]public int myPlayer;//联网专用：表示当前玩家是哪一方
    private List<string> messages;

    [SerializeField]private Text a;
    [SerializeField]private Text b;
    
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
        //在当前的游戏设计中，不需要加上DontDestroyOnLoad
        //如果加上了，ReStart重新加载当前场景时，其他的对象会被全部摧毁重建，而绑定在当前GameManager上的引用会因为对象被摧毁而丢失。

        
        //保证场景切换不会删掉manager
        //DontDestroyOnLoad(gameObject);
    }

    private void Start(){
        playerIndicator.sprite = player1Indicator; 
        GetGoldIncome(1);

        UpdateGoldText();
        GetComponent<Barrack>().CloseMenu();

        messages = FindObjectOfType<Client>().messages;
    }
    private void Update(){

        //如果有被选择的角色，高亮框显示，修改为适应的颜色，并跟随角色移动
        if(selectedUnit != null){
            //当有角色正在移动的时候，直接点击切换切换回合按钮会导致奇怪的bug，因此设定当角色正在移动时，不能切换回合
            if(selectedUnit.isMoving){
                TurnButton.interactable = false;
            }else{
                TurnButton.interactable = true;
            }
            selectedUnitSquare.SetActive(true);
            selectedUnitSquare.GetComponent<SpriteRenderer>().color = selectedUnit.squareColor;
            selectedUnitSquare.transform.position = selectedUnit.transform.position;
        }
        else {
            selectedUnitSquare.SetActive(false);
        }

        messagesHandler();
        //只有当前玩家可以按动回合结束按钮
        if(myPlayer == playerTurn){
            TurnButton.interactable = true;
        }        
        else{
            TurnButton.interactable = false;
        }
        a.text = myPlayer.ToString();
        b.text = playerTurn.ToString();
    }
    //当玩家右键点击单位时，该函数会被调用
    //功能：右键点击该单位时，
    public void ToggleStatsPanel(Unit unit){
        
        if(unit.Equals(viewedUnit) == false){
            statsPanel.SetActive(true);
            statsPanel.transform.position = (Vector2)unit.transform.position + statsPanelShift;
            viewedUnit = unit;
            UpdateStatsPanel();
        }else{
            DisableStatsPanel();
        }
    }
    //取消显示状态栏
    public void DisableStatsPanel(){
        statsPanel.SetActive(false);
        viewedUnit = null;
    }
    //暂时取消显示状态栏，在移动完成之后再显示
    public void DisableStatsPanelTemp(){
        statsPanel.SetActive(false);
    }
    //当一个单位死亡时，会调用该函数
    public void RemoveStatsPanel(Unit unit){
        //如果当前死亡的单位正好就是打开状态栏的单位，则关闭状态栏
        if(unit.Equals(viewedUnit)){
            DisableStatsPanel();
        }
    }
    public void UpdateStatsPanel(){
        if(viewedUnit != null){
            healthText.text = viewedUnit.health.ToString();
            armorText.text = viewedUnit.armor.ToString();
            attackDamageText.text = viewedUnit.attackDamage.ToString();
            defenseDamageText.text = viewedUnit.defenseDamage.ToString();
        }
    }
    public void UpdateGoldText(){
        player1GoldText.text = player1Gold.ToString();
        player2GoldText.text = player2Gold.ToString();
    }

    private void GetGoldIncome(int playerTurn){
        //遍历所有village
        foreach(Village village in FindObjectsOfType<Village>()){
            if(village.playerNumber == playerTurn){
                if(playerTurn == 1){
                    player1Gold += village.goldPerTurn;
                }
                else if(playerTurn == 2){
                    player2Gold += village.goldPerTurn;
                }
            }
        }

        UpdateGoldText();
    }
    public void ResetTiles(){
        for(int i = 0;i<GameManager.instance.tiles.Length;i++){
            GameManager.instance.tiles[i].ResetTile();
        }
    }
    public void EndTurn(){

        // if(myPlayer==1){
        //     playerTurn = 2;
        // }
        // else{
        //     playerTurn = 1;
        // }
        if(playerTurn == 1){
            Debug.Log("playerTurn=="+playerTurn);
            playerIndicator.sprite = player2Indicator;
            playerTurn = 2;
        }
        else if (playerTurn == 2){
            Debug.Log("playerTurn=="+playerTurn);
            playerIndicator.sprite = player1Indicator;
            playerTurn = 1;
        }
        else{
            Debug.Log("和1/2都不等??");
        }

        GetGoldIncome(playerTurn);

        //如果结束回合的时候，还有单位被选中，将选中单位清空
        if(selectedUnit != null){
            selectedUnit.selected = false;
            selectedUnit.ResetTiles();
        }

        //回合重新开始，所有数值重置
        foreach (Unit unit in FindObjectsOfType<Unit>()){
            unit.hasMoved = false;
            unit.weaponIcon.SetActive(false);
            unit.hasAttacked = false;
        }

        GetComponent<Barrack>().CloseMenu();
    }

    public void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //该函数将从服务器端传来的消息进行处理
    public void messagesHandler(){
        if(messages!=null){
            //FindObjectOfType<Client>().messagesChange.WaitOne();
            for(int i=0;i<messages.Count;i++){
                //先将string用逗号分割
                string[] s = messages[i].Split(',');
                switch(s[0]){
                    case "position":{
                        Vector2 pos1 = new Vector2(int.Parse(s[1]),int.Parse(s[2]));
                        Vector2 pos2 = new Vector2(int.Parse(s[3]),int.Parse(s[4]));

                        //检测处于pos1位置的单位，将其移动到位置2
                        Collider2D col  = Physics2D.OverlapCircle(pos1,0.15f);
                        Unit unit  = col.GetComponent<Unit>();
                        unit.Move(pos2);
                        messages.Remove(messages[i]);
                        break;
                    }
                    case "player":{
                        int t = int.Parse(s[1]);
                        switch(t){
                            case 1:{
                                myPlayer = 1;
                                Debug.Log("myplayer="+myPlayer);
                                Debug.Log("playerTurn="+playerTurn);
                                break;
                            }
                            case 2:{
                                Debug.Log("myplayer="+myPlayer);
                                Debug.Log("playerTurn="+playerTurn);
                                myPlayer = 2;
                                break;
                            }
                            default:{
                                Debug.Log("wrong "+s[1]);
                                break;
                            }
                        }
                        messages.Remove(messages[i]);
                        break;
                    }
                    default: messages.Remove(messages[i]);break;
                }
            }
            //FindObjectOfType<Client>().messagesChange.Release();
        }
    }

    // public void UpdateOperation(){
    //     if(myPlayer == 1){
    //         EnableOperation(1);
    //         //DisableOperation
    //     }
    // }

    //当切换回合时
    public void EnableOperation(int player){

    }
    //1.从游戏一开始，就禁用所有可以对对手造成影响的操作
    //2.当切换到对面回合时，暂时禁用自己的操作
    public void DisableOperation(int player){
        GetComponent<Barrack>().CloseMenu();
        
    }
}
