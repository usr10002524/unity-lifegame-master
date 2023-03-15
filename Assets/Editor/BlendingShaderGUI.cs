using UnityEditor;

public class BlendingShaderGUI : ShaderGUI
{
    public enum BlendMode
    {
        // ブレンドしない（上書き）
        None = 0,
        // 加算合成
        Add = 1,
        // アルファ値を乗算してから加算合成
        TransparentAdd = 2,
        // アルファブレンド
        AlphaBlend = 3,
        // 乗算合成
        Multiply = 4
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        var blendModePropName = "_BlendMode";
        var blendModeProp = FindProperty(blendModePropName, properties);
        var blendMode = (BlendMode)blendModeProp.floatValue;
        using (var scope = new EditorGUI.ChangeCheckScope())
        {
            blendMode = (BlendMode)EditorGUILayout.EnumPopup(ObjectNames.NicifyVariableName(blendModePropName), blendMode);
            if (scope.changed)
            {
                SetBlend(blendMode, properties);
                blendModeProp.floatValue = (float)blendMode;
            }
        }
    }

    private void SetBlend(BlendMode blendMode, MaterialProperty[] properties)
    {
        var blendSrc = FindProperty("_BlendSrc", properties);
        var blendDst = FindProperty("_BlendDst", properties);

        switch (blendMode)
        {
            case BlendMode.None:
                blendSrc.floatValue = (float)UnityEngine.Rendering.BlendMode.One;
                blendDst.floatValue = (float)UnityEngine.Rendering.BlendMode.Zero;
                break;
            case BlendMode.Add:
                blendSrc.floatValue = (float)UnityEngine.Rendering.BlendMode.One;
                blendDst.floatValue = (float)UnityEngine.Rendering.BlendMode.One;
                break;
            case BlendMode.TransparentAdd:
                blendSrc.floatValue = (float)UnityEngine.Rendering.BlendMode.SrcAlpha;
                blendDst.floatValue = (float)UnityEngine.Rendering.BlendMode.One;
                break;
            case BlendMode.AlphaBlend:
                blendSrc.floatValue = (float)UnityEngine.Rendering.BlendMode.SrcAlpha;
                blendDst.floatValue = (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                break;
            case BlendMode.Multiply:
                blendSrc.floatValue = (float)UnityEngine.Rendering.BlendMode.DstColor;
                blendDst.floatValue = (float)UnityEngine.Rendering.BlendMode.Zero;
                break;
            default:
                break;
        }
    }
}