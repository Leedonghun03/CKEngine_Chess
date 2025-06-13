using UnityEngine;
using System.Collections.Generic;

public class MoveIndicatorManager : MonoBehaviour
{
    [Header("이동 가능한 표시 프리팹")]
    [SerializeField] private GameObject moveIndicatorPrefab;
    
    [Header("오브젝트 풀링 사이즈")]
    [SerializeField] private int poolingSize = 30;

    private readonly Queue<GameObject> moveIndicatorObjectPool = new();
    private readonly List<GameObject> activeObjects = new();

    public void Awake()
    {
        Initialize(poolingSize);
    }

    // 풀링 사이즈 만큼 오브젝트 생성
    private void Initialize(int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject instance = Instantiate(moveIndicatorPrefab, transform);
            instance.SetActive(false);
            moveIndicatorObjectPool.Enqueue(instance);
        }
    }

    // 체스 말 이동 가능 위치 보여주는 함수
    public void ShowMoveIndicator(Board chessBoard, IEnumerable<Vector2Int> position)
    {
        ClearMoveIndicator();

        foreach (Vector2Int gridPos in position)
        {
            if (moveIndicatorObjectPool.Count == 0)
            {
                Initialize(5);
            }
            
            GameObject instance = moveIndicatorObjectPool.Dequeue();
            Vector3 worldPos = chessBoard.GridToWorldPosition(gridPos);
            instance.transform.position = new Vector3(worldPos.x, 0.01f, worldPos.z);
            instance.SetActive(true);
            activeObjects.Add(instance);
        }
    }

    // 체스 말 이동 가능 위치 초기화 함수
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
