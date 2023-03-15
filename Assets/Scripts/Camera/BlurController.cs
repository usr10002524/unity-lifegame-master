using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ブラーのポストエフェクトのコントロールクラス
/// </summary>
public class BlurController : MonoBehaviour
{
    private BlurEffect effect;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        GameObject camera = GameObject.Find("Main Camera");
        if (camera != null)
        {
            effect = camera.GetComponent<BlurEffect>();
            if (effect != null)
            {
                effect.SetEffectActive(true);
            }
        }
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     if (effect != null)
        //     {
        //         bool active = effect.GetEffectActive();
        //         effect.SetEffectActive(!active);
        //     }
        // }
    }
}
