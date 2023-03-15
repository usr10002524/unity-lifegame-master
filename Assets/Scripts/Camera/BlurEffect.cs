using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シェーダに対してブラー効果を行うためのクラス。
/// GameObjectにアタッチされたMaterialに対して操作を行う。
/// </summary>
public class BlurEffect : MonoBehaviour
{
    [SerializeField]
    private Material _material;

    private int _Direction;
    private bool useEffect = false;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        _Direction = Shader.PropertyToID("_Direction");    //シェーダのプロパティIDを検索
    }

    /// <summary>
    /// OnRenderImage
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (useEffect)
        {
            //シェーダを使用して、横半分のテクスチャに書き込み
            var rth = RenderTexture.GetTemporary(source.width / 2, source.height);
            var h = new Vector2(1, 0);  //ブラー方向のベクトル(U方向)
            _material.SetVector(_Direction, h);
            Graphics.Blit(source, rth, _material);

            //シェーダを使用して、更に縦半分のテクスチャに書き込み
            var rtv = RenderTexture.GetTemporary(rth.width, rth.height / 2);
            var v = new Vector2(0, 1);  //ブラー方向のベクトル(V方向)
            _material.SetVector(_Direction, v);
            Graphics.Blit(rth, rtv, _material);

            //出力テクスチャに書き込み
            Graphics.Blit(rtv, dest);

            //テンポラリテクスチャの解放
            RenderTexture.ReleaseTemporary(rth);
            RenderTexture.ReleaseTemporary(rtv);
        }
        else
        {
            Graphics.Blit(source, dest);
        }
    }

    /// <summary>
    /// エフェクトの有効/無効を切り替える
    /// </summary>
    /// <param name="active">有効にする場合はtrue、無効にする場合はfalse</param>
    public void SetEffectActive(bool active)
    {
        useEffect = active;
    }

    /// <summary>
    /// 現在エフェクトが有効かどうかを確認する
    /// </summary>
    /// <returns>有効の場合はtrue、無効の場合はfalse</returns>
    public bool GetEffectActive()
    {
        return useEffect;
    }
}
