namespace Catopia
{
    /// <summary>
    /// 喂食菜单点击 
    /// </summary>
    public delegate void FeedMenuClickDelegate(int index);

    /// <summary>
    /// 喂食点击 
    /// </summary>
    public delegate void FeedClickDelegate(EAT_STATE eatState);

	/// <summary>
	/// 喂食点击 
	/// </summary>
	public delegate void FeedStartClickDelegate(int index);
}