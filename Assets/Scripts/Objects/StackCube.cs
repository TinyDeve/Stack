using UnityEngine;
using DG.Tweening;

public class StackCube : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private MeshRenderer _meshRenderer;

    private GameManager _gameManager;
    public int cubeId;
    

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
    }

    public void Stop(Vector3 lastStackPosition,Vector3 lastStackScale) 
    {
        _transform.DOKill();
        CutStackObje(lastStackPosition,lastStackScale);

        //Set some data for next move stop and cut operations
        _gameManager.SetLastStackInfo(_transform.localPosition, _transform.localScale);
    }

    private void CutStackObje(Vector3 lastStackPosition, Vector3 lastStackScale)
    {
        //Cut this stack obje create one piece trash obje
    }

    public void SetPosition(Vector3 destination) 
    {
        _transform.localPosition = destination;
    }

    public void Move(Vector3 destination,int gameSpeed)
    {
        _transform.DOLocalMove(destination,gameSpeed);
    }
}
