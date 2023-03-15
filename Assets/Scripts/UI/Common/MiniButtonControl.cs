using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// サマリーパネル ミニボタンの制御を行うクラス
/// </summary>
public class MiniButtonControl : MonoBehaviour
{
    [SerializeField] protected List<Sprite> sprites;

    protected Image image;

    /// <summary>
    /// Start 相当の処理。
    /// 派生クラスから呼ぶ。
    /// </summary>
    protected void BaseStart()
    {
        image = GetComponent<Image>();
    }

    /// <summary>
    /// 指定した名前のスプライトがリスト内にある場合、スプライトの画像を差し替える。
    /// </summary>
    /// <param name="name">差し替えるスプライト名</param>
    protected void ReplaceSprite(string name)
    {
        // リストを名前で検索し、一致すれば差し替える。
        bool replaced = false;
        foreach (var item in sprites)
        {
            if (0 == name.CompareTo(item.name))
            {
                image.sprite = item;
                image.enabled = true;
                replaced = true;
            }
        }

        // 一致するものがなかった場合は非表示にする
        if (!replaced)
        {
            image.sprite = null;
            image.enabled = false;
        }
    }
}
