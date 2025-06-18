using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("인벤토리")]
    public Inventory inventory;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.collider != null)
            {
                HitCheckObject(hit);
            }
        }
    }

    void HitCheckObject(RaycastHit2D hit)
    {
        IObjectStone clickInterface = hit.transform.gameObject.GetComponent<IObjectStone>();

        if (clickInterface != null)
        {
            Stone stone = clickInterface.ClickStone();
            Debug.Log($"{stone.stoneName}");
            inventory.AddStone(stone);
        }
    }
}