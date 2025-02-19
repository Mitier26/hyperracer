using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
     [SerializeField] private GameObject carPrefab;
     [SerializeField] private GameObject roadPrefab;
     
     // UI 관련 코드
     [SerializeField] private MoveButton leftMoveButton;
     [SerializeField] private MoveButton rightMoveButton;
     [SerializeField] private TMP_Text gasText;
     [SerializeField] private GameObject startPanelPrefab;
     [SerializeField] private GameObject endPanelPrefab;
     [SerializeField] private Transform canvasTransform;

     // 자동차
     private CarController carController;
     
     // 도로 오브젝트 풀
     private Queue<GameObject> _roadPool = new Queue<GameObject>();
     private int _roadPoolSize = 3;
     
     // 도로 이동
     private List<GameObject> _activeRoads = new List<GameObject>();
     
     // 만들어지는 길의 index
     private int _roadIndex = 0;
     
     // 상태
     public enum State{Start, Play, End}
     public State GameState { get; private set; } = State.Start;
     
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
          Time.timeScale = 5f;
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
          
          // 게임 상태 Start로 변경
          GameState = State.Start;
          
          // 게임 시작
          ShowStartPanel();
     }

     private void Update()
     {
          switch (GameState)
          {
               case State.Start:
                    break;
               case State.Play:
                    foreach (var activeRoad in _activeRoads)
                    {
                         activeRoad.transform.Translate(Vector3.back * Time.deltaTime);
                    }
          
                    if(carController != null) gasText.text = carController.Gas.ToString();
                    break;
               case State.End:
                    break;
          }
     }
     
     private void StartGame()
     {
          _roadIndex = 0;
          
          // 도로 생성
          SpawnRoad(Vector3.zero);
          
          // 자동차 생성
          carController = Instantiate(carPrefab, new Vector3(0, 0, -3f), Quaternion.identity).GetComponent<CarController>();
          
          // Left, Right move buttond에 자동차 컨트롤 기능 추가

          leftMoveButton.OnMoveButtonDown += () => carController.Move(-1f);
          
          rightMoveButton.OnMoveButtonDown += () => carController.Move(1f);

          // 게임 상태 변경
          GameState = State.Play;
     }

     public void EndGame()
     {
          // 게임 상태 변경
          GameState = State.End;
          
          // 자동차 제거
          Destroy(carController.gameObject);
          
          // 도로 제거
          foreach (var activeRoad in _activeRoads)
          {
               activeRoad.SetActive(false);
          }
          
          // TODO: 게임 오버 패널 표시
          ShowEndPanel();
     }

     #region UI
     /// <summary>
     /// 시작 화면을 표시
     /// </summary>
     private void ShowStartPanel()
     {
         StartPanelController startPanelController = Instantiate(startPanelPrefab, canvasTransform).GetComponent<StartPanelController>();
         startPanelController.OnStartButtonClick += () =>
         {
              StartGame();
              Destroy(startPanelController.gameObject);
         };
     }

     /// <summary>
     /// 게임 종료 화면 표시
     /// </summary>
     private void ShowEndPanel()
     {
          StartPanelController endPanelController = Instantiate(endPanelPrefab, canvasTransform).GetComponent<StartPanelController>();
          endPanelController.OnStartButtonClick += () =>
          {
               Destroy(endPanelController.gameObject);
               ShowStartPanel();
          };
     }

     #endregion
     
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
          GameObject road;
          
          if (_roadPool.Count > 0)
          {
               road = _roadPool.Dequeue();
               road.transform.position = position;
               road.SetActive(true);
          }
          else
          {
               road = Instantiate(roadPrefab, position, Quaternion.identity);
          }

          // 가스 아이템 생성
          if (_roadIndex > 0 && _roadIndex % 2 == 0)
          {
               road.GetComponent<RoadController>().SpawnGas();
          }
          
          _activeRoads.Add(road);
          _roadIndex++;
     }

     public void DestroyRoad(GameObject road)
     {
          road.SetActive(false);
          _activeRoads.Remove(road);
          _roadPool.Enqueue(road);
     }

     #endregion

     

}
