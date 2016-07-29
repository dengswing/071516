using System.Collections.Generic;

public class CatGameConst
{
    //prefab path
    public const string ResourcesDialogueBubble = "Feed/DialogueBubble";
    public const string ResourcesItem = "Feed/shopItem/icon_0{0}";
    public const string ResourcesMenu = "Feed/button/button_0{0}";

    //spine动画系列
    public static string[] Expression = new string[] { "expression_1_a", "expression_1_b", "expression_2_a", "expression_2_b", "expression_3_a", "expression_3_b", "expression_4_a", "expression_4_b" };
    public static List<string> CatYummyList = new List<string> { "idle_1toidle_2", "idle_2" };
    public static List<string> CatYuckList = new List<string> { "idle_1toidle_3", "idle_3" };

    public const string CatYummyChange = "idle_2toidle_1";
    public const string CatYuckChange = "idle_3toidle_1";
    public const string CatIdle = "idle_1";
    public const string CatEat = "eat";
    public const string YummyStar = "animation";
    public const string MusicNote = "star1";

    public static string[] MenuBtnName = new string[] { "Seafood", "Meat", "Soup" };

    // 文字内容系列
    public const string Dialogue_Feed_A = "I am so hungry.\nCan you give me some food?";
    public const string Dialogue_Feed_B = "Oh...Yuck...\nI hate the taste.";
    public const string Dialogue_Feed_C = "It's so delicious!\nPlease accept my gift.";

    public const string Dialogue_Fondle_A = "I am so bored.\nCan you pet me?";
    public const string Dialogue_Fondle_B = "Oh...Yuck...\nI hate that.";
    public const string Dialogue_Fondle_C = "Thank you!\nI feel great!";

    //抚摸系列动作
    public const string FondleEarL = "touch1_ear_L";
    public const string FondleEyeL = "eye_l";
    public const string FondleFootL = "foot_l";


}