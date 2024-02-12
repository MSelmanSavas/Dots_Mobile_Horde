Shader "Custom/InstancedFinalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
 
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Cull Back
        ZWrite On
        Blend One OneMinusSrcAlpha 
 
        Pass
        {
            

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
 
            sampler2D _MainTex;
            //float4 _MainTex_UV;
 
            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _MainTex_UV)
            UNITY_INSTANCING_BUFFER_END(Props)
 
            v2f vert (appdata v)
            {
                v2f o;
 
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
 
                float3 worldPosition = float3(v.vertex.x, v.vertex.y, -v.vertex.y / 100);
 
                // o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = UnityObjectToClipPos(float4(worldPosition, 1.0f));
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = (v.uv * UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex_UV).xy) + UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex_UV).zw;
 
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
 
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                clip(col.a - 30.0 / 255.0);
                col.rgb *= col.a;
 
                return col;
            }
            ENDCG
        }
    }
}