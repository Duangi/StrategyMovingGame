using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Barrack : MonoBehaviour
{
    [SerializeField]private Button player1ToggleButton;
    [SerializeField]private Button player2ToggleButton;
    [SerializeField]private GameObject player1Menu;
    [SerializeField]private GameObject player2Menu;

    private bool isPlanting;//正在种小兵
    private void Update(){
        if (GameManager.instance.playerTurn == 1){
            player1ToggleButton.interactable = true;
            player2ToggleButton.interactable = false;

        }
        else if(GameManager.instance.playerTurn == 2){
            player1ToggleButton.interactable = false;
            player2ToggleButton.interactable = true;
        }

        
        if(isPlanting){
            //正在决定的时候，可以按下右键取消
            if(Input.GetMouseButtonDown(1)){
                if(GameManager.instance.playerTurn == 1){
                    player1Menu.SetActive(true); 
                }
                else if(GameManager.instance.playerTurn == 2){
                    player2Menu.SetActive(true);
                }
                GameManager.instance.purchasedItem = null;

                GameManager.instance.ResetTiles();
            }
        }
    }

    public void ToggleMenu(GameObject menu){
        menu.SetActive(!menu.activeSelf);
    }

    public void CloseMenu(){
        player1Menu.SetActive(false);
        player2Menu.SetActive(false);
    }

    public void BuyItem(BarrackItem item){
        isPlanting = true;
        //扣钱
        if(GameManager.instance.playerTurn == 1 && item.cost <= GameManager.instance.player1Gold){
            player1Menu.SetActive(false); 
        }
        else if(GameManager.instance.playerTurn == 2 && item.cost <=GameManager.instance.player2Gold){
            player2Menu.SetActive(false); 
        }else{
            print("NOT ENOUGH GOLD!");
            return;
        }
        
        
        //将选中的单位存储在gm里
        GameManager.instance.purchasedItem = item;

        //如果当前有单位被选中的话，需要将当前选中单位取消，并重置tile
        if(GameManager.instance.selectedUnit != null){
            GameManager.instance.selectedUnit.selected = false;
            GameManager.instance.selectedUnit = null;
        }
        //将可以放置小兵的地方设置为高亮
        GetCreatableTiles();
    }
    void GetCreatableTiles(){
        foreach (Tile tile in FindObjectsOfType<Tile>()){
            if (tile.isClear()){
                tile.SetCreatable();
            }
        }
    }
}
