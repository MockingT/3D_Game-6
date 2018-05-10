# 3D_Game-6  
## 智能巡逻兵  
视频链接：  
- 游戏设计要求：  
  - 创建一个地图和若干巡逻兵(使用动画)；  
  - 每个巡逻兵走一个3~5个边的凸多边型，位置数据是相对地址。即每次确定下一个目标位置，用自己当前位置为原点计算；  
  - 巡逻兵碰撞到障碍物，则会自动选下一个点为目标；  
  - 巡逻兵在设定范围内感知到玩家，会自动追击玩家；  
  - 失去玩家目标后，继续巡逻；  
  - 计分：玩家每次甩掉一个巡逻兵计一分，与巡逻兵碰撞游戏结束  
- 游戏规则：  
  - 每一次躲开巡逻兵进入下一个巡逻兵的范围，会加分。  
  - 初始情况只有四个巡逻兵，分别分布在四个矩形范围内，但是可以选择游戏界面左边的“Level Up”按钮，每按一次，一个矩形范围的巡逻兵相应增加一个，提高了游戏难度。  
  - 当玩家未进入巡逻兵的范围内，巡逻兵走动，当玩家进入范围内的消息被发布时，改变巡逻兵的速度为原来的两倍，行进方向为玩家所在的位置，当玩家离开后，又恢复行走的速度。
  - 当玩家被巡逻兵碰到时，会显示lose，但当点击reset即可重新开始游戏，但所在的level不会恢复到初始的状态。  
- 游戏实现效果：    
![avatar](https://github.com/MockingT/3D_Game-6/blob/master/pictures/result.png)  
- 文件结构  
![avatar](https://github.com/MockingT/3D_Game-6/blob/master/pictures/struct.png)  
  - SceneController.cs文件用于加载初始状态和提供Level Up和Reset功能，控制呈现出来的游戏界面。  
  - BaseAction.cs文件提供一些基础的动作，例如Patrol的静止，走动巡逻以及跑步追击等。  
  - Object.cs文件则是Patrol的工厂类（和之前几次作业想法类似），用于管理多个巡逻兵的生成，储存和清除以及Level Up功能中的增加。  
  - PatrolUI.cs则是具体实现每个巡逻兵的动作（上下左右循环行走，追击目标）等，继承了BaseAction.cs文件中的动作基类。  
  - PublisherAndObservser.cs文件则是题目中要求的订阅发布模式的具体实现，提供了一个Publisher类以及接受发布的Observer接口。  
  - ActorController.cs文件用于管理目标和目标Action的状态，实现具体的发布消息。当目标被碰到时，它会发布消息目标的死亡，这个时候游戏结束。当目标进入某个Patrol的范围内时，它发布消息通知patrol目标的进入，patrol在用Observer提供的接口接收到发布时，会改变自身状态。  
  - ScoreRecorder.cs和UIController.cs文件功能较为简单，就是显示分数和提供失败的消息。  
- 部分关键代码（具体代码见code文件夹）：  
  - 订阅与发布模式：创建一个Publisher类，由ActorController使用发布消息，而另外创建一个Observeser接口，里面接收发布的函数在PatrolUI和SceneController中具体实现，对应不同的接收到信息之后的反应。  
  
  
        public interface Publish
        {
            void notify(ActorState state, int pos, GameObject actor);
            void add(Observer observer);
            void delete(Observer observer);
        }

        // implemented in SceneController.cs
        public interface Observer
        {
            void notified(ActorState state, int pos, GameObject actor);
            // receiver
        }

        public class Publisher : Publish {
            private delegate void ActionUpdate(ActorState state, int pos, GameObject actor);
            private ActionUpdate updatelist;

            // instance
            private static Publish _instance;
            public static Publish getInstance()
            {
                if (_instance == null)
                    _instance = new Publisher();
                return _instance;
            }

            public void notify(ActorState state, int pos, GameObject actor)
            {
                // publish the notification
                if (updatelist != null)
                    updatelist(state, pos, actor);
            }

            public void add(Observer observer)
            {
                updatelist += observer.notified;
            }

            public void delete(Observer observer)
            {
                updatelist -= observer.notified;
            }
        }  
          
          
   - Level Up功能：因为想增加以下游戏难度（虽然已经很难了），于是增加了一Level Up的按钮，点一下patrol就在相应的位置多一个，会同时巡逻，一起追杀目标，其实主要用到的还是初始化场景时候的创建patrol的方法（LoadResources），不过对第一轮和后面的进行了区分，避免创建不必要的东西（目标或者patrol）。  
  
          private void LoadResources()
          {
              // the original position
              float[] posx = { -5, 7, -5, 5 };
              float[] posz = { -5, -7, 5, 5 };
              if(first_round == true)
              {
                  // the actor
                  actor = Instantiate(Resources.Load("prefabs/Ami"), new Vector3(2, 0, -2), Quaternion.Euler(new Vector3(0, 180, 0))) as GameObject;
                  ObjectFactory fac = Singleton<ObjectFactory>.Instance;
                  // the patrols
                  for (int i = 0; i < posx.Length; i++)
                  {
                      GameObject patrol = fac.setObjectOnPos(new Vector3(posx[i], 0, posz[i]), Quaternion.Euler(new Vector3(0, 180, 0)));
                      patrol.name = "Patrol" + (i + 1);
                  }
              }
              // higher level
              else
              {
                  level++;
                  for (int i = level-1; i < level; i++)
                  {
                      GameObject patrol = fac.setObjectOnPos(new Vector3(posx[i], 0, posz[i]), Quaternion.Euler(new Vector3(0, 180, 0)));
                      patrol.name = "Patrol" + (i + 1);
                  }
              }
              first_round = false;
          }  
          
          
    - Patrol不断地环绕循环：在PatrolUI继承的基础动作的类中的Update函数中回调调用下面这个函数  
  
          public void SSEventAction(SSAction source, SSActionEventType events = SSActionEventType.COMPLETED, int intParam = 0, string strParam = null, Object objParam = null)
          {
              currentState = currentState > ActionState.WALKBACK ? ActionState.IDLE : (ActionState)((int)currentState + 1);
              switch (currentState)
              {
                  // go forward
                  case ActionState.WALKFORWARD:
                      walkForward();
                      break;
                  // go back
                  case ActionState.WALKBACK:
                      walkBack();
                      break;
                  // go left
                  case ActionState.WALKLEFT:
                      walkLeft();
                      break;
                  // go right
                  case ActionState.WALKRIGHT:
                      walkRight();
                      break;
                  // stay still
                  default:
                      idle();
                      break;
              }
           }
