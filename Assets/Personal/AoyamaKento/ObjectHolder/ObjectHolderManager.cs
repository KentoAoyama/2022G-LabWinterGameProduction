using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RetainedObjectBehaviorを継承したクラスを持つゲームオブジェクトを管理するクラス
/// </summary>
public class ObjectHolderManager
{
    //シングルトン
    #region Singleton
    private static ObjectHolderManager _instance = new ObjectHolderManager();
    public static ObjectHolderManager Instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.LogError($"Error! Please correct!");
            }
            return _instance;
        }
    }
    private ObjectHolderManager(){}
    #endregion

    //メンバー変数
    #region Member Variables
    /// <summary>
    /// RetainedObjectBehaviorを継承したクラスを持つオブジェクトのリスト
    /// </summary>
    private List<GameObject> _objectHolder = new();

    /// <summary>
    /// PlayerのGameObjectを持つ変数
    /// </summary>
    private GameObject _playerHolder = null;

    #endregion

    //プロパティ
    #region Properties
    /// <summary>
    /// RetainedObjectBehaviorを継承したクラスを持つオブジェクトのリストのプロパティ
    /// </summary>
    public List<GameObject> ObjectHolder { get => _objectHolder; set => _objectHolder = value; }

    /// <summary>
    /// Playerの参照・取得を行うためのプロパティ
    /// </summary>
    public GameObject PlayerHolder { get => _playerHolder; set => _playerHolder = value; }
    #endregion

    //パブリックメソッド
    #region Public Methods
    /// <summary>
    /// ObjectHolderにオブジェクトを追加する際に呼び出すメソッド
    /// </summary>
    public void AddHolder(GameObject retainedObject)
    {
        if (retainedObject.TryGetComponent(out IPause pause))
        {
            PauseManager.Instance.OnPause += pause.Pause;
            PauseManager.Instance.OnResume += pause.Resume;
        }

        //if (retainedObject.TryGetComponent(out ))
    }

    /// <summary>
    /// ObjectHolderからオブジェクトを削除する際に呼び出すメソッド
    /// </summary>
    public void RemoveHolder(GameObject retainedObject)
    {

    }
    #endregion

    //プライベートメソッド
    #region Private Methods
    #endregion
}