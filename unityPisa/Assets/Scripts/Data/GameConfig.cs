using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class GameConfig 
{
    private static GameConfig _self;
    public static GameConfig self
    {
        get
        {
            if (GameConfig._self == null)
            {
                GameConfig._self = new GameConfig();
            }

            return _self;
        }
    }

    //玩家获取食物最大数量
    public int PlayerGetNum = 10;

    //开桌子需要的金币
    public int openDesk = 100;

    //开桌子4人需要的金币
    public int openDesk4 = 3000;

    //开外卖需要的金币
    public int openWaimai = 1000;

    //人物移动速度
    public float playerSpeed = 2;

    //获取食物的速度
    public float getFoodTime = 0.1f;

    //放下食物的速度
    public float giveFoodTime = 0.06f;

    //检测时间的速度
    public float checkPosTime = 0.1f;

    //客人吃的速度
    public float aiEatTime = 0.8f;

    //机器生成的最大值
    public int newFoodMax = 10;

    //外卖生成速度
    public float waimaiNewTime = 2f;

    //外卖车的速度
    public float waimaiMotoSpeed = 4f;

    //客人没有食物离开的时间
    public float aiSayGoTime = 15;

    //披萨的堆叠高度
    public float pisiY=0.08f;

    //外卖盒子的上限
    public int waimaiHeMax = 20;

    //开启披萨机价格
    public int openFood = 500;

    //生成金币的基础值
    public int newGold = 10;

    //升级食物机金币
    public int upFoodLeve = 500;

    //升级食物机的最大等级
    public int maxUpFood = 0;

    //食物机的升级 每个等级增加3个披萨
    public int upFoodMax = 3;

    //食物机的升级 每个等级减少0.2秒的时间
    public float upFoodGetSpeed = 0.2f;

    //新开店需要的金额
    public List<int> levelGold = new List<int> {0,20000,30000 };
    //新开店的名字
    public List<string> levelSay = new List<string> { "hamburger", "chicken", "pizza" };

    public int upLevelMax = 4;

    public int totleLevel = 2;

    public int setWaimai = 3;
    public int setFood = 1;

    //生成格子广告的时间
    public float adNew = 30;

    //插屏广告的时间
    public float inseAd = 140;
}