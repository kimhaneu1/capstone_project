using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Stone> stones;

    [SerializeField]
    private Transform slotParent;
    [SerializeField]
    private Slot[] slots;

#if UNITY_EDITOR
    private void OnValidate()
    {
        slots = slotParent.GetComponentsInChildren<Slot>();
    }
#endif

    void Awake()
    {
        FreshSlot();
    }

    public void FreshSlot()
    {
        foreach (string stoneName in Managers.Data.dataSettings.CollectedStoneNames)
        {
            Stone stone = Managers.Resource.Load<Stone>($"{stoneName}");
            stones.Add(stone);
        }

        int i = 0;
        for (; i < stones.Count && i < slots.Length; i++)
        {
            slots[i].stone = stones[i];
        }
        for (; i < slots.Length; i++)
        {
            slots[i].stone = null;
        }
    }

    public void AddStone(Stone stone)
    {
        if (stones.Count < slots.Length)
        {
            stones.Add(stone);
            FreshSlot();
        }
        else
        {
            Debug.Log("슬롯이 가득 차 있습니다.");
        }
    }
}