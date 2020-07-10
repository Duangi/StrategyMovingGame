# Strategy Moving Game

## 简介

根据youtuber  Blackthornprod 在Udemy课程网站上的课程学习制作

该篇笔记为过程回顾，（关键步骤代码的记录，以及学习心得。

### 制作Tile

> 使用sprite并添加box collider的方法实现地图
>
> 在创建地图的时候，为了方便在创建的时候移动 Edit-Grid and Snap Settings-Increment Snap-Axis中设置x，y的值为1，按住Ctrl键，拖动sprite，可以实现每次移动一格单位长度

### 美化方法

> 1. 每一个tile不使用同一个图片，而是每一个Tile随机生成不同的图片

```
[SerializeField] private Sprite[] sprites;

void Start(){
    spriteRenderer = GetComponent<SpriteRenderer>();
    int randomNum = Random .Range(0,sprites.Length);
    spriteRenderer.sprite = sprites[randomNum];
}
```

> 2. 鼠标悬停在tile上 tile暂时放大

```
private void OnMouseEnter(){
    transform.localScale += Vector3.one *0.02f;
    spriteRenderer.sortingOrder = 25;
}

private void OnMouseExit(){
    transform.localScale -= Vector3.one *0.02f;
    spriteRenderer.sortingOrder = 0;
}
```



### 制作障碍物

> 每一个tile上挂载Tile脚本，通过脚本判断当前Tile上是否存在障碍物，是否可以移动
>
> 具体方法是通过射线检测一个圆内是否存在障碍物的collider

```
Collider2D collider =  Physics2D.OverlapCircle(transform.position, spriteRenderer.bounds.extents.x,obLayerMask);
```

### 制作人物

1. 设置中心点方式为pivot。新建空game object，将身体各个部位作为其子物体。
2. 制作人物动画，并保存到prefab

### Collider重叠问题

> 将人物放置在tile上面时，会出现人物的collider和tile的collider相互重叠，导致想通过`OnMouthEnter() {}`点击人物的collider时，鼠标发出的射线被tile挡住

我的解决方法：

使用检测障碍物的方法，判断tile上是否有人物，如果有人物，修改该tile的layer为2，（layer 2是Ignore Raycast层），在该层中的物体会忽略射线的检测，从而实现点击到人物。

### 人物静止动画制作技巧

1. 最基本的：将除了脚的其他组件作为body的子物体，body先向上运动，再回到原位。
2. 稍微进阶：实现head的动作延迟。
3. scale变化：设置人物的比例，从一开始的矮胖 变化到瘦高，可以让动作不那么僵硬



动作延迟的制作过程： 

1. 10帧的时候，body整体向上，20帧回到原位。
2. 5帧的时候，head单独向下运动，15帧的时候单独向上运动

### 人物移动

判断是否可移动:点击角色，遍历所有的tile，判断和当前角色的距离，将可以移动到的tile设置为灰色

实现移动：在该例子中是先水平移动，然后垂直移动。而不是直线移动到目标位置。使用协程函数实现。

```
public void Move(Transform _transform){
    StartCoroutine(MoveCo(_transform));
    hasMoved = true;
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
}
```



### 人物攻击

判断可攻击对象： 点击角色，遍历tile，判断攻击距离内是否有可攻击角色，如果有，将人物身上被disable的sword显示出来，表示可攻击。

实现攻击动作：当玩家点击可攻击角色时，根据射线判断点击位置是否存在可攻击对象，如果存在，则实现一个镜头抖动，加显示伤害图片，随即伤害图片消失，并伴随着一个粒子效果。

> 点击位置判断是否存在可攻击对象：

```
Collider2D col  = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition),0.15f);
Unit unit  = col.GetComponent<Unit>();
if(GameManager.instance.selectedUnit != null){
    if(GameManager.instance.selectedUnit.enemiesInRange.Contains(unit) && GameManager.instance.selectedUnit.hasAttacked == false){
    GameManager.instance.selectedUnit.Attack(unit);
    }
}
```



> 实现镜头抖动效果
>
> 给MainCamera一个Animation，通过设置camera的rotation实现镜头抖动

```
camAnim.SetTrigger("shake");
```



> 显示伤害图片

```
DamageIcon instance = Instantiate(damageIcon, transform.position, Quaternion.identity);
instance.Setup(myDamage);//myDamage是一个整数，通过这个数字，设置伤害的图片
```

> 伤害图片延迟消失

给伤害图片DamageIcon添加一个脚本DamageIcon.cs

```
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
```

### 制作独特的cursor

在场景中制作鼠标指针：

1. 在sprite editor中修改指针图片的中心点。
2. 将图片拖入场景中，挂载CursorFollow脚本

```
void Start()
{
	Cursor.visible = false;
}

void Update()
{
    Vector2 CursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    transform.position = CursorPos;
}
```



在UI中制作鼠标指针：

1. 新建一个Canvas：叫CursorCanvas，修改CanvasScale 的UI Scale Mode为Scale With Screen
2. 新建CanvasGroup组件，取消勾选Interactable和Blocks Raycasts两个选项。（让鼠标指针无法交互，并且不会阻挡射线检测
3. 里面只放一个Image，将指针的sprite拖进去，（因为修改了Interactable和Blocks Raycasts，所以该Canvas只放一个Image
4. 通过这个Image的Rect Transform组件中的 Pivot变量，修改指针的中心点。
5. 挂载CursorFollow

```
	void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        transform.position = Input.mousePosition;
    }
```



### GameManager使用模板

```
	public static GameManager instance;//gm实例
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
        //如果加上了，ReStart重新加载当前场景时，其他的对象会被全部摧毁重建，而绑定在当前GameManager上的引用会因为对象被摧毁		而丢失。

        
        //保证场景切换不会删掉manager
        //DontDestroyOnLoad(gameObject);
    }
```

### 后续功能添加

1. 继续优化界面，给各个东西都加上动画，让游戏更具生命力
2. 添加ai
3. 添加联网功能