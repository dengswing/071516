using System;

namespace Catopia
{
    /// <summary>
    /// 喂食菜单点击 
    /// </summary>
    public delegate void FeedMenuClickDelegate(int index);

    /// <summary>
    /// 喂食点击 
    /// </summary>
    public delegate void FeedClickDelegate(CAT_STATE eatState);

    /// <summary>
    /// 喂食点击 
    /// </summary>
    public delegate void FeedStartClickDelegate(int index);

    /// <summary>
    /// 抚摸铭感部位
    /// </summary>
    /// <param name="state"></param>
    /// <param name="callBack"></param>
    public delegate void FondleSensitivity(string state, Action callBack);

    /// <summary>
    /// 开始抚摸猫
    /// </summary>
    /// <param name="callBack"></param>
    public delegate void FondleCatStart(CatZoomState state, Action callBack);

}