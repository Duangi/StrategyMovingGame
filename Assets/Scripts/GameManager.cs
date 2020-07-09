using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;//gm实例
    
    public Tile[] tiles;//存储所有的tile
    
    public Unit selectedUnit;//被选中的角色

    public int playerTurn = 1;

    [SerializeField] private GameObject selectedUnitSquare;//selectedUnit高亮提示框
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

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            EndTurn();
        }

        //如果有被选择的角色，高亮框显示，修改为适应的颜色，并跟随角色移动
        if(selectedUnit != null){
            selectedUnitSquare.SetActive(true);
            selectedUnitSquare.GetComponent<SpriteRenderer>().color = selectedUnit.squareColor;
            selectedUnitSquare.transform.position = selectedUnit.transform.position;
        }
        else {
            selectedUnitSquare.SetActive(false);
        }
    }

    void EndTurn(){
        if(playerTurn == 1){
            playerTurn = 2;
        }
        else if (playerTurn == 2){
            playerTurn = 1;
        }

        //如果结束回合的时候，还有单位被选中，将选中单位清空
        if(selectedUnit != null){
            selectedUnit.selected = false;
        }

        selectedUnit.ResetTiles();
        
        //回合重新开始，所有数值重置
        foreach (Unit unit in FindObjectsOfType<Unit>()){
            unit.hasMoved = false;
            unit.weaponIcon.SetActive(false);
            unit.hasAttacked = false;
        }
    }
}
