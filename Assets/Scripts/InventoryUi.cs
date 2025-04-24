using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour
{
    public GameObject[] slots;
    public int selectedIndex = 0;
    private int itemSlotSize = 100;
    
    public ItemDatabase itemDatabase;

    void Start()
    {
        itemDatabase = GameObject.Find("ItemDatabase").GetComponent<ItemDatabase>();
        UpdateSelection(0);
    }

    void Update()
    {
        //CheckNumberInput();
        UpdateToolbar();
    }

    void CheckNumberInput()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedIndex = i;
                UpdateSelection(i);
                return;
            }
        }
    }

    public void UpdateSelection(int index)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Transform highlight = slots[i].transform.Find("Highlight");
            if (highlight != null)
            {
                highlight.gameObject.SetActive(i == index);
            }
        }
    }
    
    public void UpdateToolbar()
    {
        
        for (int i = 0; i < slots.Length; i++)
        {
            
            if (GameManager.Instance.Inventory.Resources.Count <= i || GameManager.Instance.Inventory.Resources.Count == 0)
            {
                slots[i].transform.Find("ItemSlot").GetComponent<Image>().gameObject.SetActive(false); 
                // slots[i].GetComponent<Image>().color = new Color(32, 32, 32);
                slots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = "";
                
                continue;
            }
            
            var item = GameManager.Instance.Inventory.Resources.ElementAt(i);
            slots[i].transform.Find("ItemSlot").GetComponent<Image>().gameObject.SetActive(true); 
            slots[i].transform.Find("ItemSlot").GetComponent<Image>().sprite = itemDatabase.GetItemById(item.Key.id).icon;
            // slots[i].GetComponent<Image>().color = Color.white;
            slots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = item.Value.ToString();
        }
    }
}