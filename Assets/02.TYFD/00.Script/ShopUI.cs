using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public Button increaseAttackButton;
    private PlayerShop playerShop;

    private void Awake()
    {
        playerShop = GetComponentInParent<PlayerShop>();
        increaseAttackButton.onClick.AddListener(() => playerShop.OnIncreaseStatButton("attackPower", 1f));
    }

    public void ToggleShop()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}