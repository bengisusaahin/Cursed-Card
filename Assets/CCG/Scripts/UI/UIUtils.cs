using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIUtils
{
    // TAKEN FROM UMMORPG / vis2k assets (paid) - will update or remove later
    // instantiate/remove enough prefabs to match amount
    public static void BalancePrefabs(GameObject prefab, int amount, Transform parent)
    {
        // instantiate until amount
        for (int i = parent.childCount; i < amount; ++i)
        {
            GameObject go = GameObject.Instantiate(prefab);
            go.transform.SetParent(parent, false);
        }

        // delete everything that's too much
        // (backwards loop because Destroy changes childCount)
        for (int i = parent.childCount - 1; i >= amount; --i)
            GameObject.Destroy(parent.GetChild(i).gameObject);
    }
}
