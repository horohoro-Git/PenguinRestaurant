using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationKeys 
{
    //인스턴싱 애니메이션 파라미터
    public const string Idle = "Idle_A"; 
    public const string Walk = "Walk";
    public const string Run = "Run";
    public const string Fly = "Fly";
    public const string Hit = "Hit";
    public const string Bounce = "Bounce";
    public const string Eat = "Eat";

    // Shape Keys
    public const int Happy = 1;
    public const int Excited = 2;
    public const int Sad = 3;
    public const int Trauma = 4;
    public const int Dead = 5;


    // 애니메이터 파라미터
    public const string state = "state";
    public const string eat = "eat";
    public const string emotion = "emotion";

    public const string popup_close = "popup_close";
}
