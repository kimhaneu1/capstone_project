using UnityEngine;
using UnityEngine.UI;

public interface IObjectStone
{
    Stone ClickStone();
}

public class Slot : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Button button;

    private Stone _stone;
    public Stone stone
    {
        get { return _stone; }
        set
        {
            _stone = value;
            if (_stone != null)
            {
                image.sprite = stone.whiteStoneImage;
                image.color = new Color(1, 1, 1, 1);
                button.interactable = true;
            }
            else
            {
                image.color = new Color(1, 1, 1, 0);
                button.interactable = false;
            }
        }
    }

    private void Awake()
    {
        button.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        if (_stone != null)
        {
            GameServerManager.GameServer.selectedStoneName = stone.name;
            GameObject.Find("UI_GameReadyScene").GetComponent<UI_GameReadyScene>().SetProfileImage(stone);
        }
    }
}
