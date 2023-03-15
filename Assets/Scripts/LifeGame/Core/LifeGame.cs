using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LifeGameConsts;

// エイリアス
using CellStat = LifeGameConsts.Cell.Stat;

/// <summary>
/// ライフゲームコアクラス
/// </summary>
public class LifeGame
{

    /// <summary>
    /// 初期化パラメータ
    /// </summary>
    public class Initializer
    {
        public int cols; //横方向のセル数
        public int rows; //縦方向のセル数
        public bool loopWorld; //画面端でループするか
    }

    /// <summary>
    /// セル1つを制御するクラス
    /// </summary>
    public class Cell
    {
        public CellStat stat;   // セルの状態

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Cell()
        {
            stat = CellStat.Dead;
        }

        /// <summary>
        /// IntegerからCellStatへの変換
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public CellStat ToStat(int value)
        {
            switch (value)
            {
                case 1: return CellStat.Alive;
                default: return CellStat.Dead;
            }
        }
    }

    /// <summary>
    /// セルの更新情報
    /// </summary>
    public class UpdateInfo
    {
        public int col; // セルの位置
        public int row; // セルの位置
        public CellStat stat; // 更新前の状態
        public CellStat lastStat;   // 更新後の状態

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="_col">セルの位置</param>
        /// <param name="_row">セルの位置</param>
        /// <param name="_stat">更新前の状態</param>
        /// <param name="_lastStat">更新後の状態</param>
        public UpdateInfo(int _col, int _row, CellStat _stat, CellStat _lastStat)
        {
            col = _col;
            row = _row;
            stat = _stat;
            lastStat = _lastStat;
        }
    }

    private Initializer initializer;
    private int generation;
    private List<Cell> cells;


    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LifeGame()
    {
        initializer = new Initializer();
        generation = 0;
        cells = new List<Cell>();
    }

    /// <summary>
    /// コアの初期化を行う
    /// </summary>
    /// <param name="init">初期化パラメータ</param>
    public void Initialize(Initializer init)
    {
        initializer = init;
        generation = 0;

        //セルの初期化
        for (int r = 0; r < initializer.rows; r++)
        {
            for (int c = 0; c < initializer.cols; c++)
            {
                LifeGame.Cell cell = new LifeGame.Cell();
                cells.Add(cell);
            }
        }
    }

    /// <summary>
    /// 指定した位置のセルの状態を設定する
    /// </summary>
    /// <param name="col">セルの位置</param>
    /// <param name="row">セルの位置</param>
    /// <param name="stat">セルの状態</param>
    public void SetCellStat(int col, int row, CellStat stat)
    {
        int index = Coords2index(col, row);
        if (index < 0)
        {
            return; //範囲外なので無視
        }
        cells[index].stat = stat;
    }

    /// <summary>
    /// 指定した位置のセルの状態を取得する
    /// </summary>
    /// <param name="col">セルの位置</param>
    /// <param name="row">セルの位置</param>
    /// <returns>セルの状態</returns>
    public CellStat GetCellStat(int col, int row)
    {
        int index = Coords2index(col, row);
        if (index < 0)
        {
            return CellStat.None; //範囲外なので無視
        }
        return cells[index].stat;
    }

    /// <summary>
    /// 現在の世代数を取得する
    /// </summary>
    /// <returns>現在の世代数</returns>
    public int GetGeneration()
    {
        return generation;
    }

    /// <summary>
    /// 世代を進める
    /// </summary>
    /// <returns>更新のあったセルの更新情報リスト</returns>
    public List<UpdateInfo> Proceed()
    {
        List<UpdateInfo> updateInfo = new List<UpdateInfo>();

        for (int r = 0; r < initializer.rows; r++)
        {
            for (int c = 0; c < initializer.cols; c++)
            {
                CellStat lastStat = GetCellStat(c, r);
                CellStat stat = NextStat(c, r);
                if (stat == CellStat.None)
                {
                    continue;   //変化なし
                }
                if (lastStat != stat)
                {
                    updateInfo.Add(new UpdateInfo(c, r, stat, lastStat));
                }
            }
        }

        //変更後のステータスを設定する
        foreach (var info in updateInfo)
        {
            SetCellStat(info.col, info.row, info.stat);
        }

        //世代数を進める
        generation++;

        return updateInfo;
    }

    /// <summary>
    /// 初期状態に戻す
    /// </summary>
    public void Reset()
    {
        foreach (var cell in cells)
        {
            cell.stat = CellStat.Dead;
        }
    }

    /// <summary>
    /// 現在の生存セル数を取得する
    /// </summary>
    /// <returns>現在の生存セル数</returns>
    public int GetAliveCount()
    {
        int aliveCount = 0;

        foreach (var cell in cells)
        {
            if (cell.stat == CellStat.Alive)
            {
                aliveCount++;
            }
        }
        return aliveCount;
    }

    /// <summary>
    /// 現在の状態を文字列で返す
    /// セルの生存/死亡の状態を0or1で表し、4セル文をまとめて16進数1桁で表す。
    /// それを全セル分を文字列として取得する
    /// </summary>
    /// <returns>現在の状態</returns>
    public string GetData()
    {
        string text = "";

        int count = 0;
        int value = 0;

        foreach (var cell in cells)
        {
            //現在の状態を追加
            if (cell.stat == CellStat.Alive)
            {
                value |= (1 << count);
            }

            count++;
            if (count == 4)
            {
                //4つ揃ったので文字列化する
                text += System.Convert.ToString(value, 16);
                value = 0;
                count = 0;
            }
        }

        //文字列化できていない物があれば追加する
        if (count > 0 && count < 4)
        {
            text += System.Convert.ToString(value, 16);
        }

        return text;
    }

    /// <summary>
    /// GetData() で取得した文字列から状態を復帰させる
    /// </summary>
    /// <param name="text">状態をアワラした文字列</param>
    public void RestoreFromString(string text)
    {
        List<CellStat> statArray = new List<CellStat>();

        //文字列から隠せるのステータスを取得
        for (int i = 0; i < text.Length; i++)
        {
            string temp = text.Substring(i, 1);
            int value = System.Convert.ToInt32(temp, 16);

            for (int j = 0; j < 4; j++)
            {
                //下位1ビットを取り出して、ステータスに変換
                statArray.Add(Cell.ToStat((value >> j) & 1));
            }
        }

        //全セルをリセット
        Reset();
        //各セルに設定していく
        for (int i = 0; i < statArray.Count; i++)
        {
            cells[i].stat = statArray[i];
        }

    }

    /// <summary>
    /// 全セルをクリアする
    /// </summary>
    private void Clear()
    {
        cells.Clear();

        //セルの初期化
        for (int r = 0; r < initializer.rows; r++)
        {
            for (int c = 0; c < initializer.cols; c++)
            {
                Cell cell = new Cell();
                cells.Add(cell);
            }
        }
    }

    /// <summary>
    /// 縦横のセルの位置からリストのどの位置に当たるかのindexを取得する
    /// </summary>
    /// <param name="col">セルの位置</param>
    /// <param name="row">セルの位置</param>
    /// <returns>インデックス</returns>
    public int Coords2index(int col, int row)
    {
        if (col < 0 || col >= initializer.cols)
        {
            Debug.Log(string.Format("LifeGame.Coords2index x:{0} y:{1}. OutofRrange!", col, row));
            return -1;  //範囲外
        }
        if (row < 0 || row >= initializer.rows)
        {
            Debug.Log(string.Format("LifeGame.Coords2index x:{0} y:{1}. OutofRrange!", col, row));
            return -1;  //範囲外
        }

        int index = row * initializer.cols + col;
        // Debug.Log(string.Format("LifeGame.Coords2index x:{0} y:{1}.  index:{2}", col, row, index));
        return index;
    }

    /// <summary>
    /// 指定した位置のセルの次の世代の状態を取得する
    /// </summary>
    /// <param name="col">セルの位置</param>
    /// <param name="row">セルの位置</param>
    /// <returns>次の世代の状態</returns>
    protected CellStat NextStat(int col, int row)
    {
        // 指定位置のセルの状態を取得
        CellStat stat = GetCellStat(col, row);
        // 周囲八マスのセルの生存セル数を取得
        int aliveCount = GetNeighborAliveCount(col, row);

        // 次の世代の状態を決める
        switch (stat)
        {
            case CellStat.Alive:
                {
                    // セルが生存状態の場合、周囲のセルの生存数が2or3の場合に生存継続し、それ以外は死亡。
                    switch (aliveCount)
                    {
                        case 2:
                        case 3:
                            return CellStat.None;
                        default:
                            return CellStat.Dead;
                    }
                }

            case CellStat.Dead:
                {
                    // セルが死亡状態の場合、周囲のセルの生存数が3の場合に誕生し、それ以外は死亡。
                    switch (aliveCount)
                    {
                        case 3:
                            return CellStat.Alive;
                        default:
                            return CellStat.None;
                    }
                }

            default:
                return CellStat.None;
        }
    }

    /// <summary>
    /// 指定したセルの周囲のセルの生存数を取得する
    /// </summary>
    /// <param name="col">セルの位置</param>
    /// <param name="row">セルの位置</param>
    /// <returns>周囲のセルの生存数</returns>
    protected int GetNeighborAliveCount(int col, int row)
    {
        List<Vector2Int> indices = new List<Vector2Int>();
        indices.Add(new Vector2Int(-1, -1));
        indices.Add(new Vector2Int(0, -1));
        indices.Add(new Vector2Int(1, -1));
        indices.Add(new Vector2Int(-1, 0));
        // indices.Add(new Vector2Int(0, 0));
        indices.Add(new Vector2Int(1, 0));
        indices.Add(new Vector2Int(-1, 1));
        indices.Add(new Vector2Int(0, 1));
        indices.Add(new Vector2Int(1, 1));

        int aliveCount = 0;

        foreach (Vector2Int v in indices)
        {
            int c = col - v.x;
            int r = row - v.y;

            if (initializer.loopWorld)
            {
                if (c < 0) { c += initializer.cols; }
                if (r < 0) { r += initializer.rows; }
                c %= initializer.cols;
                r %= initializer.rows;
            }

            CellStat stat = GetCellStat(c, r);
            if (stat == CellStat.Alive)
            {
                aliveCount++;
            }
        }

        return aliveCount;
    }
}
