using UnityEngine;
using System.Collections.Generic;

public class MoveIndicatorManager : MonoBehaviour
{
    [Header("이동 가능한 표시 프리팹")]
    [SerializeField] private GameObject moveIndicatorPrefab;
    [SerializeField] private int poolingSize = 30;

    private Queue<GameObject> moveIndicatorObjectPool = new();
    private List<GameObject> activeObjects = new();

    public void Awake()
    {
        Initialize(poolingSize);
    }

    private void Initialize(int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject instance = Instantiate(moveIndicatorPrefab, transform);
            instance.SetActive(false);
            moveIndicatorObjectPool.Enqueue(instance);
        }
    }

    public void ShowMoveIndicator(Board board, IEnumerable<Vector2Int> position)
    {
        ClearMoveIndicator();

        foreach (Vector2Int gridPos in position)
        {
            if (moveIndicatorObjectPool.Count == 0)
            {
                Initialize(5);
            }
            
            GameObject instance = moveIndicatorObjectPool.Dequeue();
            Vector3 worldPos = board.GridToWorldPosition(gridPos);
            instance.transform.position = new Vector3(worldPos.x, 0.01f, worldPos.z);
            instance.SetActive(true);
            activeObjects.Add(instance);
        }
    }

    public void ClearMoveIndicator()
    {
        foreach (GameObject instance in activeObjects)
        {
            instance.SetActive(false);
            moveIndicatorObjectPool.Enqueue(instance);
        }
        
        activeObjects.Clear();
    }
}
