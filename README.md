# StrategyMovingGame
A game demo made with unity

## 游戏规则

1v1策略战棋游戏

对战双方在一个15*8的对战区域内，生成棋子，移动棋子，操纵棋子攻击敌方棋子。

双方都拥有一个"国王"棋子，"国王"棋子也可以移动/攻击，游戏结束的条件是击杀对方的"国王"

操作方式：鼠标点击

## 界面展示

![界面展示](https://github.com/Duangi/StrategyMovingGame/blob/master/README.assets/%E7%95%8C%E9%9D%A2%E5%B1%95%E7%A4%BA.gif?raw=true)



## 角色移动

![角色移动](https://github.com/Duangi/StrategyMovingGame/blob/master/README.assets/%E8%A7%92%E8%89%B2%E7%A7%BB%E5%8A%A8.gif?raw=true)

点击需要移动的棋子，可以移动到的格子会暂时变成灰色，再点击需要移动到的格子，棋子会自动移动到对应的位置。

## 生成棋子

![生成棋子](https://github.com/Duangi/StrategyMovingGame/blob/master/README.assets/%E7%94%9F%E6%88%90%E6%A3%8B%E5%AD%90.gif?raw=true)

## 切换回合

![切换回合](https://github.com/Duangi/StrategyMovingGame/blob/master/README.assets/%E5%88%87%E6%8D%A2%E5%9B%9E%E5%90%88.gif?raw=true)

## 攻击动画

![攻击动画](https://github.com/Duangi/StrategyMovingGame/blob/master/README.assets/%E6%94%BB%E5%87%BB%E5%8A%A8%E7%94%BB.gif?raw=true)

## 查看属性

右键点击棋子可以查看属性，鼠标点击其他地方状态栏会消失

![查看属性](https://github.com/Duangi/StrategyMovingGame/blob/master/README.assets/%E6%9F%A5%E7%9C%8B%E5%B1%9E%E6%80%A7.png?raw=true)

左上角为当前血量

左下角表示棋子的攻击力，一次攻击能对另一个棋子造成的伤害值

右上角表示护甲，可以抵消敌方棋子的一部分攻击，比如攻击力为2的棋子攻击该护甲为1的棋子，只会受到一点伤害

右下角表示反弹伤害，每次遭到攻击会反击，对地方造成伤害，同上，该伤害也会被敌方护甲抵消



![查看属性](https://github.com/Duangi/StrategyMovingGame/blob/master/README.assets/%E6%9F%A5%E7%9C%8B%E5%B1%9E%E6%80%A7.gif?raw=true)



## 游戏结束

某一方的king死亡则游戏结束

![游戏结束](https://github.com/Duangi/StrategyMovingGame/blob/master/README.assets/%E6%B8%B8%E6%88%8F%E7%BB%93%E6%9D%9F.gif?raw=true)

## 联网功能

![联网功能](https://github.com/Duangi/StrategyMovingGame/blob/master/README.assets/%E8%81%94%E7%BD%91%E5%8A%9F%E8%83%BD.png?raw=true)

一个很简陋的开始界面

1. 核心原理是，客户端通过按钮绑定的脚本，通过Socket连接到服务器端，服务器端采用java监听客户端的连接。

2. 当一个玩家申请连接时，服务器端等待另一个玩家连接，另一个玩家也连接时，服务器端将其匹配。
3. 当每一个玩家完成一次操作之后，将完成的操作发给服务器端，服务器端转发给另一个客户端
4. 另一个客户端收到服务器端发来的请求之后，更新界面上的操作