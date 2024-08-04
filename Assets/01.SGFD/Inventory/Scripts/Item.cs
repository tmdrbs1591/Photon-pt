using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field: SerializeField]
    public ItemSO InventoryItem { get; private set; }

    [field: SerializeField]
    public int Quantity { get; set; } = 1;


    [SerializeField]
    private float duration = 0.3f;

    [SerializeField]
    private int songIndex;

    private void Start()
    {
       // GetComponent<SpriteRenderer>().sprite = InventoryItem.ItemImage;
    }

    internal void DestroyItem()
    {
        GetComponent<Collider>().enabled = false;
        StartCoroutine(AnimateItemPickup());
    }
    private IEnumerator AnimateItemPickup()
    {
        SingleAudioManager.instance.PlaySound(transform.position, songIndex, Random.Range(1f, 1f), 0.4f);
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;

        }
        Destroy(gameObject);

    }

}
