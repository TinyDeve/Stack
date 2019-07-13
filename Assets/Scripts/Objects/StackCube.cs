using UnityEngine;
using DG.Tweening;

public class StackCube : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private BoxCollider _boxCollider;

    private GameManager _gameManager;
    public int cubeId;

    private Vector3 stackLocalPosition;
    private Vector3 stackLocalScale;

    private Vector3 ObjePosition;

    private bool isActive;

    private void Awake()
    {
        InitializeStack();
    }

    public void InitializeStack()
    {
        _gameManager = GameManager.Instance;
        _gameManager.Add(this);
        Active(false);
    }

    public void Active(bool active)
    {
        _meshRenderer.enabled = active;
        _boxCollider.enabled = active;
        isActive = active;
    }

    public void Stop(Vector3 lastStackPosition,Vector3 lastStackScale) 
    {
        if (!isActive)
        {
            return;
        }
        _transform.DOKill();
        CutStackObje(lastStackPosition,lastStackScale);

        //Set some data for next move stop and cut operations
    }

    private void CutStackObje(Vector3 lastStackPosition, Vector3 lastStackScale)
    {
        Active(false);

        //Cut this stack obje create one piece trash obje

        stackLocalPosition = _transform.localPosition;
        stackLocalScale = _transform.localScale;

        float zEnd = stackLocalPosition.z - lastStackPosition.z;
        float xEnd = stackLocalPosition.x - lastStackPosition.x;
        Vector3 objeScale = Vector3.zero;


        //Fail Game

        if (!(zEnd < stackLocalScale.z && zEnd > -stackLocalScale.z))
        {
            _gameManager.FailGame();
            _gameManager.PlaceCutObje(stackLocalPosition, stackLocalScale);
            return;
        }
        if (!(xEnd < stackLocalScale.x && xEnd > -stackLocalScale.x))
        {
            _gameManager.FailGame();
            _gameManager.PlaceCutObje(stackLocalPosition, stackLocalScale);
            return;
        }


        if (!(zEnd < 0.1 && zEnd > -0.1))
        {
            if (zEnd > 0) 
            {
                //Calculate position and scale to place obje
                ObjePosition = stackLocalPosition;
                ObjePosition.z += stackLocalScale.z / 2 - zEnd / 2;

                objeScale = stackLocalScale;
                objeScale.z = zEnd;

                _gameManager.PlaceCutObje(ObjePosition, objeScale);

                //Calculate position and scale to place obje
                ObjePosition = stackLocalPosition;
                ObjePosition.z += -zEnd / 2;
                objeScale = stackLocalScale;
                objeScale.z -= zEnd;
            }else
            {

                //Calculate position and scale to place obje
                ObjePosition = stackLocalPosition;
                ObjePosition.z -= (stackLocalScale.z / 2 + zEnd / 2); //xEnd Negatif

                objeScale = stackLocalScale;
                objeScale.z = -zEnd;//xEnd Negatif

                _gameManager.PlaceCutObje(ObjePosition, objeScale);


                //Calculate position and scale to place obje
                ObjePosition = stackLocalPosition;
                ObjePosition.z -= zEnd / 2;

                objeScale = stackLocalScale;
                objeScale.z += zEnd;
            }
        }
        else if(!(xEnd < 0.1 && xEnd > -0.1))
        {
            if (xEnd > 0)
            {
                Debug.Log("rigth");
                //Calculate position and scale to place obje
                ObjePosition = stackLocalPosition;
                ObjePosition.x += stackLocalScale.x / 2 - xEnd / 2;

                objeScale = stackLocalScale;
                objeScale.x = xEnd;

                _gameManager.PlaceCutObje(ObjePosition, objeScale);


                //Calculate position and scale to place obje
                ObjePosition = stackLocalPosition;
                ObjePosition.x += -xEnd / 2;

                objeScale = stackLocalScale;
                objeScale.x -= xEnd;

            }
            else
            {
                //Calculate position and scale to place obje
                ObjePosition = stackLocalPosition;
                ObjePosition.x -= (stackLocalScale.x / 2 + xEnd / 2); //xEnd Negatif

                objeScale = stackLocalScale;
                objeScale.x = -xEnd;//xEnd Negatif

                _gameManager.PlaceCutObje(ObjePosition, objeScale);


                //Calculate position and scale to place obje
                ObjePosition = stackLocalPosition;
                ObjePosition.x -= xEnd / 2;

                objeScale = stackLocalScale;
                objeScale.x += xEnd;
            }
        }
        else 
        {
            //Score
            ObjePosition = stackLocalPosition;
            objeScale = stackLocalScale;
        }

        _gameManager.PlaceObje(ObjePosition, objeScale);

        _gameManager.SetLastStackInfo(ObjePosition, objeScale);

        _gameManager.SendStack();

    }

    public void CheckFailed() 
    {
         
    }

    public void SetPositionScale(Vector3 destination,Vector3 scale) 
    {
        _transform.localPosition = destination;
        _transform.localScale = scale;

    }

    public void Move(Vector3 destination,int gameSpeed)
    {
        Vector3 direction = destination - _transform.localPosition;



        _transform
            .DOLocalMove(destination + direction, gameSpeed)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
