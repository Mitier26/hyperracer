using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadController : MonoBehaviour
{
    [SerializeField] private GameObject[] gasObjects;

    private void OnEnable()
    {
        // 모든 가스 오브젝트 비활성
        foreach (var gasObject in gasObjects)
        {
            gasObject.SetActive(false);
        }
    }

    /// <summary>
    /// 플레이어 차량이 도로에 진입하면 다른 도로를 생성
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 도로를 생성하는 것는 GameManager다!!
            // GameManager에 도로를 만들라고 해야 한다.
            // 그리고 이것은 계속 On하고 있는 것이 아님.
            GameManager.Instance.SpawnRoad(transform.position + Vector3.forward * 10f);
        }
    }

    /// <summary>
    /// 플레이어 차량이 도로를 벗어나면 해당 도로를 제거
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.DestroyRoad(gameObject);
        }
    }

    /// <summary>
    /// 랜덤으로 가스 아이템을 표시
    /// </summary>
    public void SpawnGas()
    {
        int index = Random.Range(0, gasObjects.Length);
        gasObjects[index].SetActive(true);
    }
}
