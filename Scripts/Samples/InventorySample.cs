using System.Collections.Generic;
using UnityEngine;

namespace DialogGraphWithXnodeAndLocalization.Samples
{
    [CreateAssetMenu(fileName = "Inventory Sample", menuName = "Tools/Dialog/Sample/Inventory Sample")]
    public class InventorySample : ScriptableObject
    {
        public Dictionary<string, int> inventory = new Dictionary<string, int>
        {
            { "Sword", 1 },
            { "Heal Potion", 2 }
        };

        public bool HasItem(string itemName) => inventory.ContainsKey(itemName);
        public bool HasAmmountToItem(string itemName, int ammount)
        {
            if (!inventory.ContainsKey(itemName))
                return false;

            return inventory[itemName] >= ammount;
        }

        public void AddItem(string itemName, int ammount) => inventory[itemName] = ammount;

        public void RemoveAmmountToItem(string itemName, int ammount)
        {
            if(!inventory.ContainsKey(itemName))
                return;

            inventory[itemName] -= ammount;
            if (inventory[itemName] < 0)
                inventory.Remove(itemName);
        }

        public void RemoveItem(string itemName)
        {
            if(!inventory.ContainsKey(itemName))
                return;

            inventory.Remove(itemName);
        }
    }
}