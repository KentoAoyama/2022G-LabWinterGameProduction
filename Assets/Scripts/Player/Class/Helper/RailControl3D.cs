using System;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class RailControl3D
{
    /// <summary>
    /// レールの最大本数 : <br/>
    /// レールの数え方について <br/>
    /// レールは最も下のレールが0番目で、<br/>
    /// 上のレールに行くほど1ずつ増えてゆく <br/>
    /// </summary>
    [Tooltip("レールの最大本数"), SerializeField]
    private int _maxRails = 1;
    [InputName, SerializeField]
    private string _upStepButtonName = default;
    [InputName, SerializeField]
    private string _downStepButtonName = default;
    [Tooltip("レール間の幅"), SerializeField, Range(1f, 10f)]
    private float _widthBetweenRails = 1f;
    [SerializeField]
    private float _jumpPower = 1f;
    [Tooltip("ステップに掛ける時間"), SerializeField]
    private float _stepDuration = 1.5f;
    [SerializeField]
    private bool _isReadyStep = true;

    private Transform _transform = default;
    private bool _isStepNow = false;
    public bool IsStepNow => _isStepNow;

    private PlayerStateController _stateController = null;

    public bool IsReadyStep
    {
        get => _isReadyStep;
        set
        {
            if (value)
            {
                Debug.Log("ステップ可能にします");
            }
            else
            {
                Debug.LogWarning("ステップ不可にします");
            }
            _isReadyStep = value;
        }
    }

    /// <summary>
    /// 現在いるレールの位置
    /// </summary>
    private static int _nowPos = 1;

    public void Init(Transform transform, PlayerStateController stateController)
    {
        _transform = transform;
        _stateController = stateController;
    }
    public void Update()
    {
        if (IsRun())
        {
            if (Input.GetButtonDown(_upStepButtonName))
            {
                MovementOrder(MovementDirection.MOVE_UP);
            }
            else if (Input.GetButtonDown(_downStepButtonName))
            {
                MovementOrder(MovementDirection.MOVE_DOWN);
            }
        }
    }
    private void MovementOrder(MovementDirection direction)
    {
        if (direction == MovementDirection.MOVE_UP)
        {
            Debug.Log("「上」のレールへ移動します");
            if ((_nowPos + 1) >= _maxRails)
            {
                Debug.LogWarning("それ以上上へステップできないよ");
            }
            else
            {
                _nowPos++;
                Step(_transform.position + new Vector3(0, 0, _widthBetweenRails),
                    _stepDuration);
            }
        }
        else if (direction == MovementDirection.MOVE_DOWN)
        {
            Debug.Log("「下」のレールへ移動します");
            if ((_nowPos - 1) < 0)
            {
                Debug.LogWarning("それ以上上へステップできないよ");
            }
            else
            {
                _nowPos--;
                Step(_transform.position - new Vector3(0, 0, _widthBetweenRails),
                    _stepDuration);
            }
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("無効な値が渡されました！修正してください！");
        }
#endif
    }
    private void Step(Vector3 targetPos, float seconds)
    {
        _isReadyStep = false;
        _isStepNow = true;
        _transform.DOJump(targetPos, _jumpPower, 1, _stepDuration).
            OnComplete(() =>
            {
                _isReadyStep = true;
                _isStepNow = false;
            });
    }
    private bool IsRun()
    {
        bool result = false;

        result =
            _isReadyStep &&
            _stateController.CurrentState == PlayerState.IDLE ||
            _stateController.CurrentState == PlayerState.MOVE;

        return result;
    }
}
public enum MovementDirection
{
    MOVE_UP,
    MOVE_DOWN
}