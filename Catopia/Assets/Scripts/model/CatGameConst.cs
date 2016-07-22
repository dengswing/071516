using System.Collections.Generic;

public class CatGameConst
{
    //prefab path
    public const string ResourcesDialogueBubble = "Feed/DialogueBubble";
    public const string ResourcesItem = "Feed/shopItem/icon_0{0}";
    public const string ResourcesMenu = "Feed/button/button_0{0}";

    //spine动画系列
    public static string[] Expression = new string[] { "expression_1_a", "expression_1_b", "expression_2_a", "expression_2_b", "expression_3_a", "expression_3_b", "expression_4_a", "expression_4_b" };
    public static List<string> CatYummyList = new List<string> { "ilde_1toidle_2", "idle_2" };
    public static List<string> CatYuckList = new List<string> { "idle_1toidle_3", "idle_3" };

    public const string CatYummyChange = "ilde_2toidle_1";
    public const string CatYuckChange = "idle_3toidle_1";
    public const string CatIdle = "idle_1";
    public const string CatEat = "eat";
    public const string YummyStar = "animation";

    // 文字内容系列
    public const string Dialogue_A = "I am so hungry.\nCan you give me some food.";
    public const string Dialogue_B = "Oh...Yuck...\nI hate the taste.";
    public const string Dialogue_C = "It's so delicious!\nPlease accept my gifts.";

    public static string[] MenuBtnName = new string[] { "Seafood", "Meat", "Soup" };

}