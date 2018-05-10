# 3D_Game-6  
## 智能巡逻兵  
- 游戏设计要求：  
  - 创建一个地图和若干巡逻兵(使用动画)；  
  - 每个巡逻兵走一个3~5个边的凸多边型，位置数据是相对地址。即每次确定下一个目标位置，用自己当前位置为原点计算；  
  - 巡逻兵碰撞到障碍物，则会自动选下一个点为目标；  
  - 巡逻兵在设定范围内感知到玩家，会自动追击玩家；  
  - 失去玩家目标后，继续巡逻；  
  - 计分：玩家每次甩掉一个巡逻兵计一分，与巡逻兵碰撞游戏结束  
- 游戏规则：  
  - 每一次躲开巡逻兵进入下一个巡逻兵的范围，会加分。  
  - 初始情况只有四个巡逻兵，分别分布在四个矩形范围内，但是可以选择游戏界面左边的“Level Up”按钮，没按一次，一个矩形范围的巡逻兵相应增加一个，提高了游戏难度。  
  - 当玩家未进入巡逻兵的范围内，巡逻兵走动，当玩家进入范围内的消息被发布时，改变巡逻兵的速度为原来的两倍，行进方向为玩家所在的位置，当玩家离开后，又恢复行走的速度。
  - 当玩家被巡逻兵碰到时，会显示lose，但当点击reset即可重新开始游戏，但所在的level不会恢复到初始的状态。  
- 游戏实现效果：    
![avatar](https://github.com/MockingT/3D_Game-6/blob/master/pictures/result.png)  
- 文件结构  
![avatar](https://github.com/MockingT/3D_Game-6/blob/master/pictures/struct.png)  
  - SceneController.cs文件用于加载初始状态和提供Level Up和Reset功能，控制呈现出来的游戏界面。  
  - BaseAction.cs文件
