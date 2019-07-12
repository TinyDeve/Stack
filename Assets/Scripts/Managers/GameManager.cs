using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject stackPrefab;
    [SerializeField] private Transform stacksParent;

    //Last stack info
    private Vector3 lastStackPosition;
    private Vector3 lastStackScale;

    //For stack cube pooling
    private int stackCubePoolNumber = 50;
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

    //To decrease gc
    private Vector3 objeDestination;

    private void Awake()
    {
        Instance = this;
        lastStackPosition = new Vector3(0, 0, 0);
        objeDestination = new Vector3(0, 0, 0);
        _stackNumber = 0;
        _leftRigth = true;

        //Stacks
        stacksParent.DOMove(new Vector3(0, -1000, 0), 6000);

        //Stack pooling
        CreatePool();
    }

    private void Start()
    {
        activeStackCubeIndex = 0;
        activeStackCube = stackCubes[0];
        InvokeRepeating("SendStack",0,4);
    }


    private void Update()
    {
        GetInput();
    }


    private void SendStack()
    {
        //Change direction and add a stack
        _leftRigth = !_leftRigth;
        _stackNumber++;

        activeStackCube = GetStack();

        //Calculate and set position of last cube
        objeDestination = lastStackPosition
            - outCameraDistance * (_leftRigth ? Vector3.left : Vector3.back);

        objeDestination.y = _stackNumber;

        activeStackCube.SetPosition(objeDestination);

        //Activate last cube
        activeStackCube.Active(true);

        //Calculate and set destination of last cube
        objeDestination = lastStackPosition;

        objeDestination.y = _stackNumber;

        activeStackCube.Move(objeDestination, 2);
    }

    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StopLastStack();
        }
    }

    private void StopLastStack()
    {
        stackCubes[activeStackCubeIndex].Stop(lastStackPosition, lastStackScale);
    }

    public void SetLastStackInfo(Vector3 position, Vector3 scale) 
    {
        lastStackScale = scale;

        lastStackPosition = position;
    }

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
