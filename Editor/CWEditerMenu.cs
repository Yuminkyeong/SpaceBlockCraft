using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CWEditerMenu 
{
    #region 윈도우 
    [MenuItem("GameObject/CWGames/Window/DialogBox", false, -1)]
    private static void GeneratePopUp(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/DialogBox.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "DialogBox";
        Selection.activeGameObject = gg;
       

    }

    [MenuItem("GameObject/CWGames/Window/ListDialogBox", false, -1)]
    private static void GenerateListDialogBox(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/ListDialogBox.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "ListDialogBox";
        Selection.activeGameObject = gg;


    }

    [MenuItem("GameObject/CWGames/Window/ShopDlg", false, -1)]
    private static void GenerateShopDlg(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/ShopDlg.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "ShopDlg";
        Selection.activeGameObject = gg;


    }
    [MenuItem("GameObject/CWGames/Window/Window", false, -1)]
    private static void GenerateWindow(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/Window.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Window";
        Selection.activeGameObject = gg;
    }

    [MenuItem("GameObject/CWGames/Window/Window2", false, -1)]
    private static void GenerateWindow2(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/Window2.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Window2";
        Selection.activeGameObject = gg;
    }

    #endregion
    #region UI
    
    [MenuItem("GameObject/CWGames/UI/Panel", false, -1)]
    private static void GeneratePanel(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/Panel.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Panel";
        Selection.activeGameObject = gg;
    }


    [MenuItem("GameObject/CWGames/UI/OutLine", false, -1)]
    private static void GenerateOutLine(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/OutLine.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "OutLine";

        RectTransform Rt = gg.GetComponent<RectTransform>();

        Rt.offsetMax = Vector2.zero;
        Rt.offsetMin = Vector2.zero;

        Selection.activeGameObject = gg;
    }


    [MenuItem("GameObject/CWGames/UI/Line", false, -1)]
    private static void GenerateLine(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/Line.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Line";
        Selection.activeGameObject = gg;
    }

    [MenuItem("GameObject/CWGames/UI/EmptyScrollBox", false, -1)]
    private static void GenerateEmptyScrollBox(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/EmptyScrollBox.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "EmptyScrollBox";
        Selection.activeGameObject = gg;
    }

    //

    [MenuItem("GameObject/CWGames/UI/Background", false, -1)]
    private static void GenerateBackground(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/Background.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Background";
        Selection.activeGameObject = gg;
    }


    [MenuItem("GameObject/CWGames/Production/Production", false, -1)]
    private static void GenerateProduction(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/ProductionSample.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Prodution";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/UI/BoxImage", false, -1)]
    private static void GenerateBoxImage(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/BoxImage.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "BoxImage";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/UI/ItemBox", false, -1)]
    private static void GenerateItemBox(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/ItemBox.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "ItemBox";
        Selection.activeGameObject = gg;
    }

    [MenuItem("GameObject/CWGames/UI/ItemInfoSlot", false, -1)]
    private static void GenerateItemInfoSlot(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/ItemInfoSlot.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "ItemInfoSlot";
        Selection.activeGameObject = gg;
    }

    [MenuItem("GameObject/CWGames/Production/Page", false, -1)]
    private static void GenerateProductionPage(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/Page.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Page";
        Selection.activeGameObject = gg;
    }
    #endregion
    #region 버튼들 

    [MenuItem("GameObject/CWGames/Button/CloseButton", false, -1)]
    private static void GenerateCloseButton(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/CloseButton.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "CloseButton";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/Button/BoxButton", false, -1)]
    private static void GenerateBoxButton(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/BoxButton.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "BoxButton";
        Selection.activeGameObject = gg;
    }

    [MenuItem("GameObject/CWGames/Button/NormalButton", false, -1)]
    private static void GenerateButton(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/Button.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Button";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/Button/ImageButton", false, -1)]
    private static void GenerateImageButton(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/ImageButton.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "IconButton";
        Selection.activeGameObject = gg;
    }

    //
    [MenuItem("GameObject/CWGames/Button/EmptyButton", false, -1)]
    private static void GenerateEmptyButton(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/EmptyButton.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "EmptyButton";
        Selection.activeGameObject = gg;
    }

    [MenuItem("GameObject/CWGames/Button/YellowButton", false, -1)]
    private static void GenerateYellowButton(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/YellowButton.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Button";
        Selection.activeGameObject = gg;
    }

    #endregion
    #region ValueUI
    [MenuItem("GameObject/CWGames/ValueUI/ShowHide", false, -1)]
    private static void GenerateShowHide(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/ShowHide.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "ShowHide";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/ValueUI/ShowTip", false, -1)]
    private static void GenerateShowTip(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/ShowTip.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "ShowTip";
        Selection.activeGameObject = gg;
    }

    [MenuItem("GameObject/CWGames/ValueUI/HelpClick", false, -1)]
    private static void GenerateHelpClick(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/HelpClick.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "HelpClick";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/ValueUI/CheckAlram", false, -1)]
    private static void GenerateCheckAlram(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/CheckAlram.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "CheckAlram";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/ValueUI/TutoHelp", false, -1)]
    private static void GenerateTutoHelp(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/TutoHelp.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "TutoHelp";
        Selection.activeGameObject = gg;
    }

    #endregion
    #region HELP
    [MenuItem("GameObject/CWGames/HELP/HelpCuror", false, -1)]
    private static void GenerateHelpCuror(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/HelpCuror.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "HelpCuror";
        Selection.activeGameObject = gg;
    }


    #endregion
    #region TEXT

    [MenuItem("GameObject/CWGames/TEXT/TextBox", false, -1)]
    private static void GenerateTextBox(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/TextBox.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "TextBox";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/TEXT/Tip", false, -1)]
    private static void GenerateTip(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/Tip.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Tip";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/TEXT/Coinbg", false, -1)]
    private static void GenerateCoinbg(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/Coinbg.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Coinbg";
        Selection.activeGameObject = gg;
    }

    [MenuItem("GameObject/CWGames/TEXT/Text", false, -1)]
    private static void GenerateText(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/Text.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "Text";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/TEXT/NumberText", false, -1)]
    private static void GenerateNumberText(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/NumberText.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "NumberText";
        Selection.activeGameObject = gg;
    }
    [MenuItem("GameObject/CWGames/TEXT/CoinText", false, -1)]
    private static void GenerateCoinText(MenuCommand command)
    {
        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/CoinText.prefab", typeof(GameObject));
        GameObject gg = GameObject.Instantiate(obj);
        GameObject gParent = (GameObject)command.context;
        gg.transform.SetParent(gParent.transform);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.rotation = new Quaternion();
        gg.name = "CoinText";
        Selection.activeGameObject = gg;
    }

    #endregion
}
