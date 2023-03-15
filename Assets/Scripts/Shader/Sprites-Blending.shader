Shader "Sprites/Blending"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

        [HideInInspector]_BlendMode("Blend Mode", int)      = 0
        _BlendSrc("Blend Src", Float)                       = 1.0
        _BlendDst("Blend Dst", Float)                       = 0.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        // Blend One OneMinusSrcAlpha
        Blend [_BlendSrc] [_BlendDst]

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
        ENDCG
        }
    }
CustomEditor "BlendingShaderGUI"
    
    
//     Properties
//     {
//         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
//         [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
//         [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
//         [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
//         [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
//         [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
//         [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

//         [HideInInspector]_BlendMode("Blend Mode", int)      = 0
//         _BlendSrc("Blend Src", int)                         = 1
//         _BlendDst("Blend Dst", int)                         = 0
//     }

//     SubShader
//     {
//         Tags
//         {
//             "Queue"="Transparent"
//             "IgnoreProjector"="True"
//             "RenderType"="Transparent"
//             "PreviewType"="Plane"
//             "CanUseSpriteAtlas"="True"
//         }

//         Cull Off
//         Lighting Off
//         ZWrite Off
//         // Blend One OneMinusSrcAlpha
//         Blend [_BlendSrc] [_BlendDst]

//         CGPROGRAM
//         #pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
//         #pragma multi_compile_local _ PIXELSNAP_ON
//         #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
//         #include "UnitySprites.cginc"

//         struct Input
//         {
//             float2 uv_MainTex;
//             fixed4 color;
//         };

//         void vert (inout appdata_full v, out Input o)
//         {
//             v.vertex = UnityFlipSprite(v.vertex, _Flip);

//             #if defined(PIXELSNAP_ON)
//             v.vertex = UnityPixelSnap (v.vertex);
//             #endif

//             UNITY_INITIALIZE_OUTPUT(Input, o);
//             o.color = v.color * _Color * _RendererColor;
//         }

//         void surf (Input IN, inout SurfaceOutput o)
//         {
//             fixed4 c = SampleSpriteTexture (IN.uv_MainTex) * IN.color;
//             o.Albedo = c.rgb * c.a;
//             o.Alpha = c.a;
//         }
//         ENDCG
//     }

// Fallback "Transparent/VertexLit"
// CustomEditor "BlendingShaderGUI"
}
