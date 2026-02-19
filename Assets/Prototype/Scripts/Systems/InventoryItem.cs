[System.Serializable]
public class InventoryItem
{
    // Holds ANY item data (Crop, Product, Tool)
    //public BaseItemData data;
    public int amount;

    public InventoryItem(/*BaseItemData data, */int amount)
    {
        //this.data = data;
        this.amount = amount;
    }
}