using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
     [SerializeField] private GameObject carPrefab;
     [SerializeField] private GameObject roadPrefab;
     
     // UI 관련 코드
     [SerializeField] private MoveButton leftMoveButton;
     [SerializeField] private MoveButton rightMoveButton;

     private Queue<GameObject> _roadPool = new Queue<GameObject>();
     private int _roadPoolSize = 3;
     
     // 도로 이동
     private List<GameObject> _activeRoads = new List<GameObject>();
     
     // 싱글톤
     private static GameManager _instance;

     public static GameManager Instance
     {
          get
          {
               if (_instance == null)
               {
                    _instance = FindObjectOfType<GameManager>();
               }
               return _instance;
          }
     }

     private void Awake()
     {
          if (_instance == null)
          {
               _instance = this;
          }
          else
          {
               Destroy(gameObject);
          }
     }

     private void Start()
     {
          // 도로 오브젝트 풀 초기화
          InitializeRoadPool();
          
          // 게임 시작
          StartGame();
     }

     private void Update()
     {
          // 활성화 된 도로를 아래로 서서히 이동
          foreach (var activeRoad in _activeRoads)
          {
               activeRoad.transform.Translate(Vector3.back * Time.deltaTime);
          }
     }

     private void StartGame()
     {
          // 도로 생성
          SpawnRoad(Vector3.zero);
          
          // 자동차 생성
          var carController = Instantiate(carPrefab, new Vector3(0, 0, -3f), Quaternion.identity).GetComponent<CarController>();
          
          // Left, Right move buttond에 자동차 컨트롤 기능 추가

          leftMoveButton.OnMoveButtonDown += () => carController.Move(-1f);
          
          rightMoveButton.OnMoveButtonDown += () => carController.Move(1f);
     }
     
     #region 도로 생성 및 관리
     
     /// <summary>
     /// 도로 오브젝트 풀 초기화
     /// </summary>
     private void InitializeRoadPool()
     {
          for (int i = 0; i < _roadPoolSize; i++)
          {
               GameObject road = Instantiate(roadPrefab);
               road.SetActive(false);
               _roadPool.Enqueue(road);
          }
     }
     
     /// <summary>
     /// 도로 오브젝트 풀에서 불러와 배치하는 함수
     /// </summary>
     public void SpawnRoad(Vector3 position)
     {
          if (_roadPool.Count > 0)
          {
               GameObject road = _roadPool.Dequeue();
               road.transform.position = position;
               road.SetActive(true);
               
               // 활성화 된 딜을 움직이기 위해 list에 저장
               _activeRoads.Add(road);
          }
          else
          {
               GameObject road = Instantiate(roadPrefab);
               road.transform.position = position;
               _activeRoads.Add(road);
          }
     }

     public void DestroyRoad(GameObject road)
     {
          road.SetActive(false);
          _activeRoads.Remove(road);
          _roadPool.Enqueue(road);
     }

     #endregion


}
