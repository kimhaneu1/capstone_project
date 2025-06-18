using UnityEngine;

public class ObjectStone : MonoBehaviour, IObjectStone
{
    [Header("오목알")]
    public Stone stone;
    [Header("오목알 이미지")]
    public SpriteRenderer stoneImage;

    void Start()
    {
        stoneImage.sprite = stone.blackStoneImage;
    }
    public Stone ClickStone()
    {
        return this.stone;
    }
}