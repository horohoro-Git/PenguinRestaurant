using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoneyPile))]
public class MoneyPileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MoneyPile moneyPile = (MoneyPile)target;

        // 기본 인스펙터 속성 그리기
        DrawDefaultInspector();

        // 버튼을 만들고 클릭 시 EarnMoney 호출
        if (GUILayout.Button("돈 벌기"))
        {
           // moneyPile.EarnMoney();
        }
    }
}
