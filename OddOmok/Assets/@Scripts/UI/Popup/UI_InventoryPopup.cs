using Cysharp.Threading.Tasks;
using NUnit.Framework.Interfaces;
using System.Linq;
using UnityEngine;
using static Define;

public class UI_InventoryPopup : UI_Popup
{
    enum GameObjects
    {
        OverlayImage,
        Background,
        CancelButtonImage,
    }

    enum Buttons
    {
        CancelButton,
    }

    enum Texts
    {
        InventoryText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetButton((int)Buttons.CancelButton).gameObject.BindEvent((evt) => CloseInventory());

        GetText((int)Texts.InventoryText).text = "오목알 선택";

        return true;
    }

    private void CloseInventory()
    {
        ClosePopupUI();
    }
}
