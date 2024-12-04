namespace LeagueSandbox.GameServer.Inventory
{
    public class ItemManager
    {
        private readonly Dictionary<int, ItemData> _itemTypes;

        public ItemManager()
        {
            _itemTypes = new Dictionary<int, ItemData>();
        }

        public ItemData GetItemType(int itemId)
        {
            return new();//_itemTypes[itemId];
        }

        public ItemData SafeGetItemType(int itemId)
        {
            return _itemTypes.GetValueOrDefault(itemId, null);
        }

        public void ResetItems()
        {
            _itemTypes.Clear();
        }

        public void AddItemType(ItemData itemType)
        {
            _itemTypes.Add(itemType.ItemId, itemType);
            itemType.CreateRecipe(this);
        }
    }
}