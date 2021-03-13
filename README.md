# StrategyMovingGame
A game demo made with unity

## 游戏规则

1v1策略战棋游戏

对战双方在一个15*8的对战区域内，生成棋子，移动棋子，操纵棋子攻击敌方棋子。

双方都拥有一个"国王"棋子，"国王"棋子也可以移动/攻击，游戏结束的条件是击杀对方的"国王"

操作方式：鼠标点击

## 界面展示

![9fd6e23b-78a7-42b2-8974-a2a4104ee753](D:\Material\Project\unity\StategyGameMoving\README.assets\9fd6e23b-78a7-42b2-8974-a2a4104ee753.gif)



## 角色移动

![3df11ce6-afc5-4ea1-aa93-2fac7fa0ae46](D:\Material\Project\unity\StategyGameMoving\README.assets\3df11ce6-afc5-4ea1-aa93-2fac7fa0ae46.gif)

点击需要移动的棋子，可以移动到的格子会暂时变成灰色，再点击需要移动到的格子，棋子会自动移动到对应的位置。

## 生成棋子

![9feb43a3-3dd5-4b85-9c46-943fde7d8bb5](D:\Material\Project\unity\StategyGameMoving\README.assets\9feb43a3-3dd5-4b85-9c46-943fde7d8bb5.gif)

## 切换回合

![b4fa0bf6-0e70-4fc6-b93c-5b3b66c4c45e](D:\Material\Project\unity\StategyGameMoving\README.assets\b4fa0bf6-0e70-4fc6-b93c-5b3b66c4c45e.gif)

## 攻击动画

![8fea75a5-3771-4c21-86b7-5c2f356d91c9](D:\Material\Project\unity\StategyGameMoving\README.assets\8fea75a5-3771-4c21-86b7-5c2f356d91c9.gif)

## 查看属性

右键点击棋子可以查看属性，鼠标点击其他地方状态栏会消失

![image-20210313231508867](D:\Material\Project\unity\StategyGameMoving\README.assets\image-20210313231508867.png)

左上角为当前血量

左下角表示棋子的攻击力，一次攻击能对另一个棋子造成的伤害值

右上角表示护甲，可以抵消敌方棋子的一部分攻击，比如攻击力为2的棋子攻击该护甲为1的棋子，只会受到一点伤害

右下角表示反弹伤害，每次遭到攻击会反击，对地方造成伤害，同上，该伤害也会被敌方护甲抵消



![b2bce058-6988-4f49-9103-80ef591947da](D:\Material\Project\unity\StategyGameMoving\README.assets\b2bce058-6988-4f49-9103-80ef591947da.gif)



## 游戏结束

某一方的king死亡则游戏结束

![2c8280df-2949-47ba-b3aa-fc08990ee50c](D:\Material\Project\unity\StategyGameMoving\README.assets\2c8280df-2949-47ba-b3aa-fc08990ee50c.gif)

## 联网功能

![image-20210313185504225](D:\Material\Project\unity\StategyGameMoving\README.assets\image-20210313185504225.png)

一个很简陋的开始界面

1. 核心原理是，客户端通过按钮绑定的脚本，通过Socket连接到服务器端，服务器端采用java监听客户端的连接。

2. 当一个玩家申请连接时，服务器端等待另一个玩家连接，另一个玩家也连接时，服务器端将其匹配。
3. 当每一个玩家完成一次操作之后，将完成的操作发给服务器端，服务器端转发给另一个客户端
4. 另一个客户端收到服务器端发来的请求之后，更新界面上的操作