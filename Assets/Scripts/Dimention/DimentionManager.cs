using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DimentionManager
{
    //シングルトン
    #region Singleton
    private static DimentionManager _instance = new DimentionManager();
    public static DimentionManager Instance
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
    private DimentionManager() { }
    #endregion

    //メンバー変数
    #region Member Variables  

    /// <summary>
    /// 敵のリスト
    /// </summary>
    private List<GameObject> _enemyHolder = new();

    /// <summary>
    /// Dimentionを行うオブジェクトのリスト
    /// </summary>
    private List<GameObject> _enemyBulletHolder = new();

    /// <summary>
    /// シーン遷移時に座標を保管しておくための変数
    /// </summary>
    private Vector3 _playerPosition;

    /// <summary>
    /// シーン遷移時に座標を保管しておくための変数
    /// </summary>
    private List<EnemyStatus> _enemyStatuses = new();

    /// <summary>
    /// シーン遷移時に座標を保管しておくための変数
    /// </summary>
    private List<EnemyBulletStatus> _enemyBulletStatuses = new();

    /// <summary>
    /// シーンの遷移を行う前のStateを保存しておく用の変数
    /// </summary>
    private GameStateManager.InGameState _beforeState;
    #endregion

    //プロパティ
    #region Properties

    /// <summary>
    /// シーンの遷移を行う前のStateのプロパティ
    /// </summary>
    public GameStateManager.InGameState BeforeState => _beforeState;
    #endregion

    //パブリックメソッド
    #region Public Methods

    /// <summary>
    /// DimentionHolderにGameObjectを追加するためのメソッド
    /// </summary>
    /// <param name="retainedObject">追加するオブジェクト</param>
    public void AddDimentionHolder(GameObject retainedObject)
    {
        //敵をEnemyHolderに登録
        if (retainedObject.GetComponent<RetainedEnemyBehavior>())
        {
            _enemyHolder.Add(retainedObject);
        }
        //敵の弾をObjectHolderに登録
        else if (retainedObject.GetComponent<RetainedEnemyBulletBehavior>())
        {
            _enemyBulletHolder.Add(retainedObject);
        }
    }

    /// <summary>
    /// DimentionHolderからGameObjectを削除するためのメソッド
    /// </summary>
    /// <param name="removeObject">削除するオブジェクト</param>
    public void RemoveDimentionHolder(GameObject removeObject)
    {
        //敵をEnemyHolderから削除
        if (removeObject.GetComponent<RetainedEnemyBehavior>())
        {
            _enemyHolder.Remove(removeObject);
        }
        //プレイヤー・敵の弾から削除
        else if (removeObject.GetComponent<RetainedEnemyBulletBehavior>())
        {
            _enemyBulletHolder.Remove(removeObject);
        }
    }

    /// <summary>
    /// シーンの遷移を開始するためのクラス
    /// </summary>
    /// <param name="sceneName">遷移するシーンの名前</param>
    /// <param name="changeInterval">シーンの遷移にかける時間</param>
    /// <returns></returns>
    public IEnumerator DimentionChangeStart(string sceneName, IEnumerator fadeDimention)
    {
        Debug.Log("DimentionChangeスタート");

        //ゲームオブジェクトの状態を保存
        SaveStatus();
        //現在のステートを2D3D切り替えのために保存
        _beforeState = GameStateManager.Instance.GameState;
        //ポーズの処理を実行
        PauseManager.Instance.OnPause();
        //シーン遷移後の処理のためステートを変更
        GameStateManager.Instance.GameStateChange(GameStateManager.InGameState.DimentionChange);
        //遷移演出が終わるまで待機
        yield return fadeDimention;

        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// シーンの変更完了後の処理
    /// </summary>
    /// <param name="finishInterval">シーン遷移後の演出にかける時間</param>
    public IEnumerator DimentionChangeFinish(DimentionPrefabs dimentionPrefabs, IEnumerator fadeDimention)
    {
        //遷移してすぐポーズを実行
        PauseManager.Instance.OnPause();
        //オブジェクトを前のシーンのように配置
        DimentionChange(dimentionPrefabs);

        yield return fadeDimention;

        //GameStateの3D2Dを判定してStateを変更
        if (_beforeState == GameStateManager.InGameState.Game2D)
        {
            GameStateManager.Instance.GameStateChange(GameStateManager.InGameState.Game3D);
        }
        else if (_beforeState == GameStateManager.InGameState.Game3D)
        {
            GameStateManager.Instance.GameStateChange(GameStateManager.InGameState.Game2D);
        }

        //演出終了後ポーズを解除
        PauseManager.Instance.OnResume();
        Debug.Log("DimentionChange終了");
    }
    #endregion

    //プライベートメソッド
    #region Private Methods

    /// <summary>
    /// ゲームオブジェクトの状態の保存を行うクラス
    /// </summary>
    private void SaveStatus()
    {
        //Playerの座標を保存
        _playerPosition = ObjectHolderManager.Instance.PlayerHolder.transform.position;
        SaveEnemyStatus();
        SaveEnemyBulletStatus();
    }

    /// <summary>
    /// Enemyの情報をEnemyStatus型のListに保存する処理
    /// </summary>
    private void SaveEnemyStatus() 
    {
        if (_enemyHolder == null)
        {
            return;
        }

        foreach (GameObject enemy in _enemyHolder)
        {
            //敵のステータスをListに格納していく
            EnemyStatus enemyStatus = new();
            RetainedEnemyBehavior enemyBehavior = enemy.GetComponent<RetainedEnemyBehavior>();
            enemyStatus.Id = enemyBehavior.Id;
            enemyStatus.Health = enemyBehavior.Health;
            enemyStatus.Position = enemy.transform.position;

            _enemyStatuses.Add(enemyStatus);
        }
    }

    /// <summary>
    /// ObjectHolderに格納されているGameObjectの座標をListに保存する処理
    /// </summary>
    private void SaveEnemyBulletStatus()
    {
        if (_enemyHolder == null)
        {
            return;
        }

        foreach (GameObject enemyBullet in _enemyBulletHolder)
        {
            //敵の弾のステータスをListに格納していく
            EnemyBulletStatus enemyBulletStatus = new();
            RetainedEnemyBulletBehavior enemyBulletBehavior = enemyBullet.GetComponent<RetainedEnemyBulletBehavior>();
            enemyBulletStatus.Id = enemyBulletBehavior.Id;
            enemyBulletStatus.Position = enemyBullet.transform.position;

            _enemyBulletStatuses.Add(enemyBulletStatus);
        }
    }

    /// <summary>
    /// シーンの遷移後にオブジェクトを前の状態にする処理
    /// </summary>
    private void DimentionChange(DimentionPrefabs dimentionPrefabs)
    {
        var player = Object.Instantiate(dimentionPrefabs.Player);
        player.transform.position = _playerPosition;
        DimentionChangeEnemy(dimentionPrefabs);
        DimentionChangeEnemyBullet(dimentionPrefabs);
    }

    /// <summary>
    /// DimentionChange後に敵に行う処理
    /// </summary>
    private void DimentionChangeEnemy(DimentionPrefabs dimentionPrefabs)
    {
        if (_enemyStatuses == null)
        {
            return;
        }

        foreach (EnemyStatus enemyStatus in _enemyStatuses)
        {
            //Idごとの敵を生成し、Hpと座標を変更する
            var enemy = Object.Instantiate(dimentionPrefabs.Enemy[enemyStatus.Id]);
            enemy.GetComponent<RetainedEnemyBehavior>().Health = enemyStatus.Health;
            enemy.transform.position = enemyStatus.Position;
        }
        _enemyStatuses.Clear();
    }

    /// <summary>
    /// DimentionChange後に敵の弾に行う処理
    /// </summary>
    private void DimentionChangeEnemyBullet(DimentionPrefabs dimentionPrefabs)
    {
        if (_enemyBulletStatuses == null)
        {
            return;
        }

        foreach (EnemyBulletStatus enemyBulletStatus in _enemyBulletStatuses)
        {
            var enemyBullet = Object.Instantiate(dimentionPrefabs.EnemyBullet[enemyBulletStatus.Id]);
            enemyBullet.transform.position = enemyBulletStatus.Position;
        }
        _enemyBulletStatuses.Clear();
    }
    #endregion
}