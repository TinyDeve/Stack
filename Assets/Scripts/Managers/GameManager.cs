using UnityEngine;
using DG.Tweening;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject stackPrefab;
    [SerializeField] private GameObject cutPrefab;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform stacksParent;
    [SerializeField] private Transform garbageParent;

    //Last stack info
    private Vector3 lastStackPosition;
    private Vector3 lastStackScale;

    //For stack cube pooling
    private int stackCubePoolNumber = 5;
    private StackCube[] stackCubes;

    //To keep track of active stack cube
    private StackCube activeStackCube;
    private int activeStackCubeIndex = 0;

    //Game variables
    private Vector3 stackPieceHeigth = new Vector3(0, 1, 0);
    private int outCameraDistance = 10;

    //
    private int _stackNumber;
    private bool _leftRigth;

    private int gameSpeed;

    //To 
    private bool gameFail;
    private bool restarting;

    //To decrease gc
    private Vector3 objeDestination;
    private Vector3 stackParentPosition;

    private void Awake()
    {
        Instance = this;
        gameSpeed = 2;


        lastStackPosition = new Vector3(0, 0, 0);
        lastStackScale = new Vector3(5, 1, 5);
        objeDestination = new Vector3(0, 0, 0);
        _stackNumber = 0;
        _leftRigth = true;
        //Stack pooling
        CreatePool();

    }

    private void Start()
    {
        activeStackCubeIndex = 0;
        activeStackCube = stackCubes[0];
        SendStack();
    }


    private void Update()
    {
        GetInput();
    }


    public void SendStack()
    {
        //Change direction and add a stack
        _leftRigth = !_leftRigth;
        _stackNumber++;

        activeStackCube = GetStack();

        //Calculate and set position of last cube
        objeDestination = lastStackPosition
            - outCameraDistance * (_leftRigth ? Vector3.left : Vector3.back);

        objeDestination.y = _stackNumber;

        activeStackCube.SetPositionScale(objeDestination,lastStackScale);

        //Activate last cube
        activeStackCube.Active(true);

        //Calculate and set destination of last cube
        objeDestination = lastStackPosition;

        objeDestination.y = _stackNumber;

        activeStackCube.Move(objeDestination, gameSpeed);
    }

    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (gameFail && !restarting)
            {
                Restart();
                return;
            }
            StopLastStack();
        }
    }

    public void SetLastStackInfo(Vector3 position, Vector3 scale) 
    {
        lastStackScale = scale;

        lastStackPosition = position;
    }

    public void FailGame() 
    {
        Debug.Log("GameFail");
        gameFail = true;
    }

    public void Restart() 
    {
        restarting = true;
        stacksParent.DOMove(Vector3.zero, gameSpeed).OnComplete(EmptyAllObjects);
    }

    public void EmptyAllObjects() 
    {
        lastStackPosition = new Vector3(0, 0, 0);
        lastStackScale = new Vector3(5, 1, 5);
        objeDestination = new Vector3(0, 0, 0);
        _stackNumber = 0;
        _leftRigth = true;


        CleanObjects();


        activeStackCubeIndex = 0;
        activeStackCube = stackCubes[0];
        SendStack();

        restarting = false;
        gameFail = false;
    }

    private void CleanObjects()
    {
        for (int i = 0; i < garbageParent.transform.childCount; i++) 
        {
            Destroy(garbageParent.GetChild(i).gameObject);
        }
    }

    public void MoveBelow() 
    {
        stackParentPosition = stacksParent.transform.position;

        stackParentPosition -= stackPieceHeigth;

        stacksParent.DOMove(stackParentPosition,gameSpeed);
    }

    #region Cutting

    private void StopLastStack()
    {
        stackCubes[activeStackCubeIndex].Stop(lastStackPosition, lastStackScale);
        MoveBelow();
    }

    public void PlaceCutObje(Vector3 position,Vector3 scale) 
    {
        GameObject a = Instantiate(cutPrefab, garbageParent) as GameObject;

        a.transform.localPosition = position;
        a.transform.localScale = scale;

    }

    public void PlaceObje(Vector3 position, Vector3 scale)
    {
        GameObject a = Instantiate(cubePrefab, garbageParent) as GameObject;

        a.transform.localPosition = position;
        a.transform.localScale = scale;
    }

    private void GameLosse() 
    { 

    }

    #endregion

    #region StackCube Pooling
    public void CreatePool()
    {
        //Create a pool for stacking
        stackCubes = new StackCube[stackCubePoolNumber];

        for (int i = 0; i < stackCubePoolNumber; i++)
        {
            Instantiate(stackPrefab, stacksParent);
        }

    }

    public void Add(StackCube stackCube)
    {
        for (int i = 0; i < stackCubePoolNumber; i++)
        {
            if (stackCubes[i] == null)
            {
                stackCubes[i] = stackCube;
                stackCube.cubeId = i;
                break;
            }
        }
    }

    public StackCube GetStack()
    {
        activeStackCubeIndex++;
        if (activeStackCubeIndex >= stackCubes.Length)
        {
            activeStackCubeIndex = 0;
        }

        return stackCubes[activeStackCubeIndex];
    }
    #endregion

}
