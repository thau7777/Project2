using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
public abstract class FarmAnimal : MonoBehaviour
{
    [SerializeField] private Gender _gender;
    [SerializeField] protected string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] protected Rigidbody2D _body;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected FarmAnimalSO _animalInfo;

    [SerializeField]
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set
        {
            if (_isFacingRight != value)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

            _isFacingRight = value;
        }
    }

    [SerializeField] protected int _currentEatCount;
    [SerializeField] protected int _eatTimesNeededToGrow;
    [SerializeField] protected int _eatTimesNeededToMakeProduct;
    [SerializeField] protected bool _canMakeProduct;
    [SerializeField] protected string[] _growStages;
    [SerializeField] protected int _stageIndex;
    [SerializeField] protected string _currentStage;
    public string CurrentStage
    {
        get { return _currentStage; }
        private set { _currentStage = value; }
    }
    public bool IsMature => _stageIndex == _growStages.Length-1;

    protected float maxRadius = 5f; // Maximum movement radius
    protected float speed = 2f; // Movement speed


    protected Vector2 targetPosition;

    [SerializeField] protected bool isMoving = false;
    [SerializeField] protected bool canMove = true;

    protected Vector2 _lastMovement;
    public Vector2 LastMovement
    {
        get { return _lastMovement; }
        set
        {
            _lastMovement = value;
            _animator.SetFloat("Horizontal", Mathf.Abs(_lastMovement.x));
            _animator.SetFloat("Vertical", _lastMovement.y);
        }

    }

    private Coroutine stopMovingCoroutine; 

    protected void Start()
    {

        Initial();
    }

    protected void Update()
    {
        if (canMove)
        {
            if(!isMoving)
            {
                ChooseNewTarget();
            }
            else
            {
                MoveToTarget();
            }
        }
        SetAnimator();
        CheckFacing();
    }

    private void FixedUpdate()
    {
        //if (canMove)
        //{
        //    if (isMoving)
        //    {
        //        MoveToTarget();
        //    }
        //}
        
    }
    [ContextMenu("Eat")]
    protected void Eat()
    {
        if(CurrentStage != "Egg")
        {
            _animator.SetTrigger("Eat");
            Debug.Log("Eat");
            StartCoroutine(WaitForEatAnimation());  
            
        }

    }

    private IEnumerator WaitForEatAnimation()
    {
        // Wait until the Blend Tree tagged "Eat" is active
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsTag("Eat"));

        float animationDuration = _animator.GetCurrentAnimatorStateInfo(0).length;
        StartStopMoving((int)animationDuration + 1);
    }

    public virtual void EndOfEat()
    {
        _currentEatCount++;
        if (!IsMature)
        {
            if (_currentEatCount >= _eatTimesNeededToGrow)
            {
                IncreaseGrowStage();
            }
        }
        else
        {
            if (_currentEatCount >= _eatTimesNeededToMakeProduct && !_canMakeProduct)
            {
                _currentEatCount = 0;
                MakeProduct();

            }
        }
        
    }

    public void IncreaseGrowStage()
    {
        _currentEatCount = 0;
        _stageIndex++;
        _currentStage = _growStages[_stageIndex];
        ApplyStage();
    }

    protected void Initial()
    {
        GenerateGuid();
        _currentEatCount = 0;
        _eatTimesNeededToGrow = _animalInfo.EatTimesNeededToGrow;
        _eatTimesNeededToMakeProduct = _animalInfo.EatTimesNeededToMakeProduct;
        _growStages = _animalInfo.GrowStages;
        _stageIndex = 0;
        _currentStage = _growStages[_stageIndex];
        _gender = _animalInfo.Gender;
        FarmAnimalManager.Instance.RegisterAnimal(this);
        ApplyStage();
    }
    private void ChooseNewTarget()
    {
        Vector2 currentPosition = transform.position;

        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        float randomDistance = Random.Range(0f, maxRadius);
        Vector2 offset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomDistance;

        targetPosition = currentPosition + offset;
        isMoving = true;
    }

    private void MoveToTarget()
    {
        if (!canMove) return;

        Vector2 currentPosition = _body.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        _body.velocity = direction * speed;

        if (direction != Vector2.zero)
            LastMovement = direction;

        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            isMoving = false; 
            StartStopMoving(5);
        }
    }

    private IEnumerator StopMoving(int minute)
    {
        _body.velocity = Vector2.zero; 
        canMove = false; 

        yield return new WaitForSeconds(minute); 

        canMove = true; 
        stopMovingCoroutine = null; 
    }

    private void StartStopMoving(int minute)
    {
        if (stopMovingCoroutine != null)
        {
            StopCoroutine(stopMovingCoroutine);
        }

        stopMovingCoroutine = StartCoroutine(StopMoving(minute));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Collision")
        {
            StartStopMoving(5);
        }
        
    }
    protected void CheckFacing()
    {
        if (IsFacingRight && _lastMovement.x < 0)
            IsFacingRight = !IsFacingRight;
        else if(!IsFacingRight && _lastMovement.x > 0)
            IsFacingRight = !IsFacingRight;
    }

    public void SetAnimator()
    {
        _animator.SetFloat("Speed", _body.velocity.magnitude);

    }
    protected virtual void ApplyStage() { }
    protected virtual void MakeProduct() { } // each animal will have different way to product
    protected virtual void GetProduct() { }
    protected virtual void InteractWithAnimal() { }

}
