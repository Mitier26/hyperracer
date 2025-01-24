using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GameTestScript
{
    private CarController _carController;
    private GameObject _leftMoveButton;
    private GameObject _rightMoveButton;
    
    // A Test behaves as an ordinary method
    [Test]
    public void GameTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator GameTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        
        // 타임 스케일 변경
        Time.timeScale = 5f;
        
        // 씬 로드
        SceneManager.LoadScene("Scenes/Game", LoadSceneMode.Single);
        
        yield return waitForSceneLoad();
        
        // 필수 오브젝트 확인
        var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Assert.IsNotNull(gameManager, "GameManager not found");
        
        var startButton = GameObject.Find("Start Button");
        Assert.IsNotNull(startButton, "StartButton not found");
        
        
        // Start Button Click, 게임 실행
        startButton.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
        
        // 플레이어 자동차 확인
        _carController = GameObject.Find("Car(Clone)").GetComponent<CarController>();
        Assert.IsNotNull(_carController, "CarController not found");
        
        // 게임 제어 관련 버튼 
        _leftMoveButton = GameObject.Find("LeftMoveButton");
        Assert.IsNotNull(_leftMoveButton, "LeftButton not found");
        _rightMoveButton = GameObject.Find("RightMoveButton");
        Assert.IsNotNull(_rightMoveButton, "RightButton not found");
        
        // 가스의 스폰 위치 확인
        Vector3 leftPosition = new Vector3(-1.5f, 0.3f, -3f);
        Vector3 centerPosition = new Vector3(0f, 0.3f, -3f);
        Vector3 rightPosition = new Vector3(1.5f, 0.3f, -3f);

        float rayDistance = 10f;
        Vector3 rayDirection = Vector3.forward;
        
        float elapsedTime = 0f;
        float targetTime = 10f;
        
        // 반복
        while (gameManager.GameState == GameManager.State.Play)
        {
            RaycastHit hit;
            if (Physics.Raycast(leftPosition, rayDirection, out hit, rayDistance, LayerMask.GetMask("Gas")))
            {
                Debug.Log("left");
                MoveCar(hit.point);
            }
            else if (Physics.Raycast(centerPosition, rayDirection, out hit, rayDistance, LayerMask.GetMask("Gas")))
            {
                Debug.Log("center");
                MoveCar(hit.point);
            }
            else if (Physics.Raycast(rightPosition, rayDirection, out hit, rayDistance, LayerMask.GetMask("Gas")))
            {
                Debug.Log("right");
                MoveCar(hit.point);
            }
            else
            {
                Debug.Log("none");
                MoveButtonUp(_leftMoveButton);
                MoveButtonUp(_rightMoveButton);
            }
            
            Debug.DrawRay(leftPosition, rayDirection * rayDistance, Color.red);
            Debug.DrawRay(centerPosition, rayDirection * rayDistance, Color.green);
            Debug.DrawRay(rightPosition, rayDirection * rayDistance, Color.blue);
            
            // 시간 체크
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        if (elapsedTime < targetTime)
        {
            Assert.Fail("Game time is too short");
        }
        
        Time.timeScale = 1f;
    }

    private IEnumerator waitForSceneLoad()
    {
        while (SceneManager.GetActiveScene().buildIndex > 0)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Move Button Down
    /// </summary>
    /// <param name="moveButton"></param>
    private void MoveButtonDown(GameObject moveButton)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(moveButton, pointerEventData, ExecuteEvents.pointerDownHandler);
    }
    
    /// <summary>
    /// Move Button Up
    /// </summary>
    /// <param name="moveButton"></param>
    private void MoveButtonUp(GameObject moveButton)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(moveButton, pointerEventData, ExecuteEvents.pointerUpHandler);
    }
    
    /// <summary>
    /// 자동차 이동
    /// </summary>
    /// <param name="targetPosition"></param>
    private void MoveCar(Vector3 targetPosition)
    {
        if (Mathf.Abs(targetPosition.x - _carController.transform.position.x) < 0.1f)
        {
            MoveButtonUp(_rightMoveButton);
            MoveButtonUp(_leftMoveButton);
            return;
        }
        if (targetPosition.x < _carController.transform.position.x)
        {
            // 왼쪽 이동
            MoveButtonDown(_leftMoveButton);
            MoveButtonUp(_rightMoveButton);
        }
        else if (targetPosition.x > _carController.transform.position.x)
        {
            // 오른쪽 이동
            MoveButtonDown(_rightMoveButton);
            MoveButtonUp(_leftMoveButton);
        }
        else
        {
            MoveButtonUp(_leftMoveButton);
            MoveButtonUp(_rightMoveButton);
        }
    }
}
