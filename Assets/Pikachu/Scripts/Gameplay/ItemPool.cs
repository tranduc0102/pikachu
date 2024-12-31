using System.Collections.Generic;
using UnityEngine;

namespace _pikachu
{
    public class ItemPool : MonoBehaviour
    {
        private Queue<Item> itemPool = new Queue<Item>();
        private Queue<LineRenderPath> objPool = new Queue<LineRenderPath>();
        public LineRenderPath GetItem(LineRenderPath itemPrefab, Vector3 position, Transform boardParent = null)
        {
            LineRenderPath item;
            if (objPool.Count > 0)
            {
                item = objPool.Dequeue();
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Instantiate(itemPrefab, position, Quaternion.identity, boardParent);
            }
            item.transform.position = position;
            return item;
        }
        public void ReturnItem(LineRenderPath item)
        {
            item.gameObject.SetActive(false);
            objPool.Enqueue(item);
        }
        public Item GetItem(Item itemPrefab, Vector3 position, Transform boardParent = null)
        {
            Item item;
            if (itemPool.Count > 0)
            {
                item = itemPool.Dequeue();
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Instantiate(itemPrefab, position, Quaternion.identity, boardParent);
            }
            item.transform.position = position;
            return item;
        }
        public void ReturnItem(Item item)
        {
            item.gameObject.SetActive(false);
            itemPool.Enqueue(item);
        }
    }

}
