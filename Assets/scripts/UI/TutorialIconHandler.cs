using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialIconHandler : MonoBehaviour, IPointerClickHandler
{
    TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            var linkInfo = text.textInfo.linkInfo[linkIndex];
           
            switch(linkInfo.GetLinkID())
            {
                case "Counter":
                    GameInstance.GameIns.applianceUIManager.shopUI.scrolling.ScrollUp();
                    GameInstance.GameIns.store.ChangeList(WorkSpaceType.Counter);
                    break;
                case "Table":
                    GameInstance.GameIns.applianceUIManager.shopUI.scrolling.ScrollUp();
                    GameInstance.GameIns.store.ChangeList(WorkSpaceType.Table);
                    break ;
                case "Machine":
                    GameInstance.GameIns.applianceUIManager.shopUI.scrolling.ScrollUp();
                    GameInstance.GameIns.store.ChangeList(WorkSpaceType.FoodMachine);
                    break;
                case "Advertisement":
                    GameInstance.GameIns.uiManager.ChangeScene(SceneState.Draw);
                    break;
                case "Restaurant":
                    GameInstance.GameIns.uiManager.ChangeScene(SceneState.Restaurant);
                    break;
                case "PenguinEmployee":
                    GameInstance.GameIns.restaurantManager.HireEmployee();
                    break;
                case "Fishing":
                    GameInstance.GameIns.uiManager.ChangeScene(SceneState.Fishing);
                    break;
                case "Trashcan":
                    GameInstance.GameIns.applianceUIManager.shopUI.scrolling.ScrollUp();
                    GameInstance.GameIns.store.ChangeList(WorkSpaceType.Trashcan);
                    break;
                case "WorldMap":
                    GameInstance.GameIns.uiManager.ChangeMap();
                    break;
            }
        }
    }
}
