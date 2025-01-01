using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace _pikachu
{
    public class Board : MonoBehaviour
    {
        [Header("GameObject")]
        [SerializeField] private Transform _boardObject;
        [SerializeField] private Item _itemPrefab;
        [SerializeField] private LineRenderPath _path;
        [SerializeField] private List<Item> path;

        [Header("Setting Board")]
        [SerializeField] private int totalColumn = 10;
        [SerializeField] private int totalRow = 8;
        public float cellXY = 0.1f;
        public int TotalColumn
        {
            get { return totalColumn; }
            set { totalColumn = Mathf.Min(14, value); }
        }
        public int TotalRow
        {
            get { return totalRow; }
            set { totalRow = Mathf.Min(12, value); }
        }

        [SerializeField] private Sprite[] spritesItem;


        [Header("Hint")]
        private Item hintObject1;
        private Item hintObject2;

        [Header("Setting ListItem")]
        private List<int> ListValues;
        private int totalItem;
        public Item[,] items;
        public ItemPool _itemPool;
        public List<Item> _listBorder;
        public int TotalItem
        {
            get { return totalItem; }
            set { totalItem = value; }
        }

        [System.Obsolete]
        private void Reset()
        {
            LoadCompoments();
        }

        private void LoadCompoments()
        {
            _itemPool = GameObject.Find("ItemPool").GetComponent<ItemPool>();
            if (_itemPool == null)
            {
                GameObject itemPoolObject = new GameObject("ItemPool");
                _itemPool = itemPoolObject.AddComponent<ItemPool>();
            }
            _boardObject = GameObject.Find("Board").transform;
            _itemPrefab = Resources.Load<Item>("Item");
            spritesItem = Resources.LoadAll<Sprite>("Items");
            _listBorder = new List<Item>();
            _path = Resources.Load<LineRenderPath>("Path");
        }

        [System.Obsolete]
        private void GetValues(int start, int end)
        {
            if (end > spritesItem.Length)
            {
                end = spritesItem.Length;
            }
            ListValues = new List<int>();
            int value;
            for (int i = 0; i < totalItem / 2; i++)
            {
                value = Random.Range(start, end);
                ListValues.Add(value);
                ListValues.Add(value);
            }

            for (int index = 0; index < ListValues.Count; index++)
            {
                int temp = ListValues[index];
                int randomIndex = Random.Range(index, ListValues.Count);
                ListValues[index] = ListValues[randomIndex];
                ListValues[randomIndex] = temp;
            }
        }

        [System.Obsolete]
        public void SpawnShapeSquare(int start, int end)
        {
            int indexx = 1;
            totalItem = (totalColumn - 2) * (totalRow - 2);
            GetValues(start, end);
            items = new Item[totalRow, totalColumn];
            int index = 0;
            for (int i = 0; i < totalRow; i++)
            {
                for (int j = 0; j < totalColumn; j++)
                {
                    float xPos = i;
                    float yPos = j;
                    float posXWithGap = xPos * (cellXY + 1);
                    float posYWithGap = yPos * (cellXY + 1);

                    if (i == 0 || i == totalRow - 1 || j == 0 || j == totalColumn - 1)
                    {
                        Item borderItem = _itemPool.GetItem(_itemPrefab, new Vector3(posXWithGap, posYWithGap, 0f), _boardObject);
                        borderItem.name = "Border" + indexx;
                        indexx++;
                        borderItem.row = i;
                        borderItem.column = j;
                        borderItem.gameObject.SetActive(false);
                        items[i, j] = borderItem;
                        _listBorder.Add(borderItem);
                        continue;
                    }
                    Item newItem = _itemPool.GetItem(_itemPrefab, new Vector3(posXWithGap, posYWithGap, 0f), _boardObject);
                    newItem.name = $"[{i}, {j}]";
                    newItem.row = i;
                    newItem.column = j;
                    newItem.value = ListValues[index];
                    newItem.spriteObject.sprite = spritesItem[ListValues[index]];
                    newItem.transform.localScale = Vector3.zero;
                    items[i, j] = newItem;
                    index++;
                }
            }

        }
        public void activeAnimItems()
        {
            StartCoroutine(StartAnim());
        }
        IEnumerator StartAnim()
        {
            GameManager.Instance.Pause = true;
            for (int j = 0; j < items.GetLength(1); j++)
            {
                for(int i = 0; i < items.GetLength(0); i++)
                {
                    if (items[i, j].gameObject.activeSelf)
                    {
                        items[i, j].AnimActive();
                        yield return new WaitForSeconds(0.02f);
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
            GameManager.Instance.Pause = false;
            if(GameManager.Instance.Level == 1 && GameManager.Instance.IsFirstPlayGame_Pika)
            {
                yield return new WaitForSeconds(0.2f);
                Tutorial_Pikachu.Instance.ShowHint();
            }
        }
        public bool CheckConnectItem(Item startItem, Item endItem, bool hint = false)
        {

            if (startItem == null || endItem == null ||
                startItem.value != endItem.value ||
                ReferenceEquals(startItem, endItem))
            {
                return false;
            }
            Queue<(Item item, int changeDirection, List<Item> path)> queue = new Queue<(Item, int, List<Item>)>();
            Dictionary<Item, int> visited = new Dictionary<Item, int>();

            queue.Enqueue((startItem, 0, new List<Item> { startItem }));
            visited[startItem] = 0;

            int[] rowDirections = { -1, 1, 0, 0 }; // Lên, Xuống, Trái, Phải
            int[] colDirections = { 0, 0, -1, 1 };

            while (queue.Count > 0)
            {
                var (currentItem, changeDirection, path) = queue.Dequeue();

                if (currentItem == endItem)
                {
                    if (!hint)
                    {
                        DOVirtual.DelayedCall(0.15f, () =>
                        {
                            LineRenderPath objPath = _itemPool.GetItem(_path, transform.position);
                            objPath.DrawPathWithLineRenderer(path);
                        });
                    }
                    return true;
                }

                for (int i = 0; i < 4; i++)
                {
                    int newRow = currentItem.row + rowDirections[i];
                    int newCol = currentItem.column + colDirections[i];

                    if (newRow < 0 || newCol < 0 || newRow >= totalRow || newCol >= totalColumn)
                        continue;

                    Item neighbor = items[newRow, newCol];
                    if ((neighbor == endItem || !neighbor.gameObject.activeSelf) &&
                        (!visited.ContainsKey(neighbor) || visited[neighbor] >= changeDirection))
                    {
                        int newChangeDirection = changeDirection;
                        if (path.Count > 1)
                        {
                            Item prevItem = path[path.Count - 2];
                            int prevDirection = GetDirection(prevItem, currentItem);
                            int currentDirection = GetDirection(currentItem, neighbor);

                            if (prevDirection != currentDirection)
                                newChangeDirection++;
                        }

                        if (newChangeDirection > 2)
                            continue;

                        visited[neighbor] = newChangeDirection;
                        var newPath = new List<Item>(path) { neighbor };
                        queue.Enqueue((neighbor, newChangeDirection, newPath));
                    }
                }
            }
            return false;
        }

        private int GetDirection(Item from, Item to)
        {
            if (to.row < from.row) return 0;
            if (to.row > from.row) return 1;
            if (to.column < from.column) return 2;
            if (to.column > from.column) return 3;
            return -1;
        }


        public bool GetHint(bool check = false)
        {
            ClearHintItems();
            for (int row = 0; row < items.GetLength(0); row++)
            {
                for (int column = 0; column < items.GetLength(1); column++)
                {
                    if (!items[row, column].gameObject.activeSelf || items[row, column].value == -1 )
                    {
                        continue;
                    }

                    Item firstPosition = items[row, column];
                    List<Item> positions = GetSameValueItems(firstPosition);

                    foreach (Item position in positions)
                    {
                        if (CheckConnectItem(firstPosition, items[position.row, position.column], true) && !ReferenceEquals(firstPosition, items[position.row, position.column]))
                        {
                            hintObject1 = items[row, column];
                            hintObject2 = items[position.row, position.column];
                            if (!check)
                            {
                                ShowHintItemsColor();
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private List<Item> GetSameValueItems(Item currentItem)
        {
            List<Item> listSameItems = new List<Item>();
            for (int row = 0; row < items.GetLength(0); row++)
            {
                for (int column = 0; column < items.GetLength(1); column++)
                {
                    if (currentItem.row == row && currentItem.column == column || items[row, column].value == -1)
                    {
                        continue;
                    }

                    if (items[row, column].value == currentItem.value)
                    {
                        Item sameItem = items[row, column];
                        listSameItems.Add(sameItem);
                    }
                }
            }
            return listSameItems;
        }
        public void Change(bool showAnim = false)
        {
            List<int> listValues = ListValues;
            if (showAnim)
            {
                foreach (var item in items)
                {
                    item.AnimShuffe();
                }
            }
            for (int row = 0; row < items.GetLength(0); row++)
            {
                for (int column = 0; column < items.GetLength(1); column++)
                {
                    Item item1 = items[row, column];
                    if (!item1.gameObject.activeSelf)
                    {
                        continue;
                    }
                RanDomItem:
                    int randomRow = Random.Range(row, items.GetLength(0));
                    int randomColumn = Random.Range(column, items.GetLength(1));
                    Item item2 = items[randomRow, randomColumn];
                    if (!item2.gameObject.activeSelf)
                    {
                        goto RanDomItem;
                    }
                    SwapItems(item1, item2);
                }
            }
            ClearHintItems();
        }
        public void SwapItems(Item itemA, Item itemB)
        {
            int tempValue = itemA.value;
            itemA.value = itemB.value;
            itemB.value = tempValue;

            Sprite tempSprite = itemA.spriteObject.sprite;
            itemA.spriteObject.sprite = itemB.spriteObject.sprite;
            itemB.spriteObject.sprite = tempSprite;
        }
        private void ShowHintItemsColor()
        {
            if (hintObject1 != null && hintObject2 != null)
            {
                Color color = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f);
                hintObject1.itemBackground.color = color;
                hintObject2.itemBackground.color = color;
            }
        }

        private void ClearHintItems()
        {
            if (hintObject1 != null && hintObject2 != null)
            {
                Color cleanColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                hintObject1.itemBackground.color = cleanColor;
                hintObject2.itemBackground.color = cleanColor;
            }
            hintObject1 = null;
            hintObject2 = null;
        }

        [System.Obsolete]
        public void UpdateBoardLevelRight(Item firstItem, Item secondItem)
        {
            MoveAllItemsRight(firstItem.column);
            MoveAllItemsRight(secondItem.column);
        }

        private void MoveAllItemsRight(int column)
        {
            for (int rowI = totalRow - 2; rowI >= 1; rowI--)
            {
                for (int rowJ = totalRow - 2; rowJ >= 1; rowJ--)
                {
                    if (items[rowJ, column].value == -1 && items[rowJ - 1, column].value > -1 && !items[rowJ, column].name.Contains("Border"))
                    {
                        Vector3 tmpV3 = items[rowJ - 1, column].transform.position; ;
                        items[rowJ - 1, column].transform.position = items[rowJ, column].transform.position;
                        items[rowJ, column].transform.position = tmpV3;

                        Item tempItem = items[rowJ, column];
                        items[rowJ, column] = items[rowJ - 1, column];
                        items[rowJ - 1, column] = tempItem;

                        items[rowJ, column].row = rowJ;
                        items[rowJ - 1, column].row = rowJ - 1;
                    }
                }
            }
        }
        public void UpdateBoardLevelLeft(Item firstItem, Item secondItem)
        {
            MoveAllItemLeft(firstItem.column);
            MoveAllItemLeft(secondItem.column);
        }

        private void MoveAllItemLeft(int column)
        {
            for (int rowI = 0; rowI <= totalRow - 2; rowI++)
            {
                for (int rowJ = 0; rowJ <= totalRow - 2; rowJ++)
                {
                    if (items[rowJ, column].value == -1 && items[rowJ + 1, column].value > -1 && !items[rowJ, column].name.Contains("Border"))
                    {
                        Vector3 tmpV3 = items[rowJ + 1, column].transform.position;
                        items[rowJ + 1, column].transform.position = items[rowJ, column].transform.position;
                        items[rowJ, column].transform.position = tmpV3;

                        Item tempItem = items[rowJ, column];
                        items[rowJ, column] = items[rowJ + 1, column];
                        items[rowJ + 1, column] = tempItem;


                        items[rowJ, column].row = rowJ;
                        items[rowJ + 1, column].row = rowJ + 1;
                    }
                }
            }
        }

        public void UpdateBoardLevelUp(Item firstItem, Item secondItem)
        {
            MoveAllItemsUp(firstItem.row);
            MoveAllItemsUp(secondItem.row);
        }

        private void MoveAllItemsUp(int row)
        {
            for (int columnI = totalColumn - 2; columnI >= 1; columnI--)
            {
                for (int columnJ = totalColumn - 2; columnJ >= 1; columnJ--)
                {
                    if (items[row, columnJ].value == -1 && items[row, columnJ - 1].value > -1 && !items[row, columnJ].name.Contains("Border"))
                    {
                        Vector3 tmpV3 = items[row, columnJ - 1].transform.position;
                        items[row, columnJ - 1].transform.position = items[row, columnJ].transform.position;
                        items[row, columnJ].transform.position = tmpV3;

                        Item tempItem = items[row, columnJ];
                        items[row, columnJ] = items[row, columnJ - 1];
                        items[row, columnJ - 1] = tempItem;

                        items[row, columnJ].column = columnJ;
                        items[row, columnJ - 1].column = columnJ - 1;
                    }
                }
            }
        }

        public void UpdateBoardLevelDown(Item firstItem, Item secondItem)
        {
            MoveAllItemsDown(firstItem.row);
            MoveAllItemsDown(secondItem.row);
        }

        private void MoveAllItemsDown(int row)
        {
            for (int columnI = 1; columnI < totalColumn - 2; columnI++)
            {
                for (int columnJ = 1; columnJ < totalColumn - 2; columnJ++)
                {
                    if (items[row, columnJ].value == -1 && items[row, columnJ + 1].value > -1 && !items[row, columnJ].name.Contains("Border"))
                    {
                        Vector3 tmpV3 = items[row, columnJ + 1].transform.position;
                        items[row, columnJ + 1].transform.position = items[row, columnJ].transform.position;
                        items[row, columnJ].transform.position = tmpV3;

                        Item tempItem = items[row, columnJ];
                        items[row, columnJ] = items[row, columnJ + 1];
                        items[row, columnJ + 1] = tempItem;


                        items[row, columnJ].column = columnJ;
                        items[row, columnJ + 1].column = columnJ + 1;
                    }
                }
            }

        }
    }

}