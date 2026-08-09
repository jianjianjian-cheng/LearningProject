using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanel : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private HP HP;

    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button inventoryHideButton;


    void Start()
    {
        //Lambda
        // button.onClick.AddListener(() => { Debug.Log("Click Button"); });

        button.onClick.AddListener(() => SetHP(20));

        inventoryButton.onClick.AddListener(() => OpenInventory());

        inventoryHideButton.onClick.AddListener(() => UIManager.Instacnce.Hide<InventoryPanel>());
    }

    private void SetHP(float value)
    {
        HP.SetHP(value);
    }

    private void OpenInventory()
    {
        UIManager.Instacnce.Show<InventoryPanel>();
    }

}
