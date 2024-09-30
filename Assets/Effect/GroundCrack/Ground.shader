Shader "Unlit/Ground"
{
    Properties
    {
	    _DepthTex("DepthTex", 2D) = "white" {}
	    _EmissionTex("Emission", 2D) = "white" {}
	    _MaskTex("Mask", 2D) = "white" {}
	    _Depth("depth", Range( 0 , 0.5)) = 0
	    _Aphla_scale("aphla_scale", Range( 0 , 1)) = 1
	    [HDR]_EmissionColor("Emission Color", Color) = (1,1,0,1)
    }
    SubShader
    {
	    Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	    LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
    	
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            	float4 tangent : TANGENT;
            	float3 normal : NORMAL;
            	
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

            	float4 tangent : TEXCOORD1;
            	float4 worldNormal : TEXCOORD2;
            	float4 worldBitangent : TEXCOORD3;
            	float3 worldPos : TEXCOORD4;
            };
            sampler2D _DepthTex;
            sampler2D _MaskTex;
            sampler2D _EmissionTex;
            float4 _DepthTex_ST;
            float _Aphla_scale;
            float _Depth;
            fixed4 _EmissionColor;
            float2 POM( sampler2D heightMap, float2 uvs, float2 dx, float2 dy, float3 normalWorld, float3 viewWorld, float3 viewDirTan, int minSamples, int maxSamples, float parallax, float refPlane, float2 tilling, float2 curv, int index )
			{
				float3 result = 0;
				int stepIndex = 0;
				int numSteps = ( int )lerp( (float)maxSamples, (float)minSamples, saturate( dot( normalWorld, viewWorld ) ) );
				float layerHeight = 1.0 / numSteps;
				float2 plane = parallax * ( viewDirTan.xy / viewDirTan.z );
				uvs.xy += refPlane * plane;
				float2 deltaTex = -plane * layerHeight;
				float2 prevTexOffset = 0;
				float prevRayZ = 1.0f;
				float prevHeight = 0.0f;
				float2 currTexOffset = deltaTex;
				float currRayZ = 1.0f - layerHeight;
				float currHeight = 0.0f;
				float intersection = 0;
				float2 finalTexOffset = 0;
				while ( stepIndex < numSteps + 1 )
				{
				 	currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).r;
				 	if ( currHeight > currRayZ )
				 	{
				 	 	stepIndex = numSteps + 1;
				 	}
				 	else
				 	{
				 	 	stepIndex++;
				 	 	prevTexOffset = currTexOffset;
				 	 	prevRayZ = currRayZ;
				 	 	prevHeight = currHeight;
				 	 	currTexOffset += deltaTex;
				 	 	currRayZ -= layerHeight;
				 	}
				}
				int sectionSteps = 10;
				int sectionIndex = 0;
				float newZ = 0;
				float newHeight = 0;
				while ( sectionIndex < sectionSteps )
				{
				 	intersection = ( prevHeight - prevRayZ ) / ( prevHeight - currHeight + currRayZ - prevRayZ );
				 	finalTexOffset = prevTexOffset + intersection * deltaTex;
				 	newZ = prevRayZ - intersection * layerHeight;
				 	newHeight = tex2Dgrad( heightMap, uvs + finalTexOffset, dx, dy ).r;
				 	if ( newHeight > newZ )
				 	{
				 	 	currTexOffset = finalTexOffset;
				 	 	currHeight = newHeight;
				 	 	currRayZ = newZ;
				 	 	deltaTex = intersection * deltaTex;
				 	 	layerHeight = intersection * layerHeight;
				 	}
				 	else
				 	{
				 	 	prevTexOffset = finalTexOffset;
				 	 	prevHeight = newHeight;
				 	 	prevRayZ = newZ;
				 	 	deltaTex = ( 1 - intersection ) * deltaTex;
				 	 	layerHeight = ( 1 - intersection ) * layerHeight;
				 	}
				 	sectionIndex++;
				}
				return uvs.xy + finalTexOffset;
			}
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _DepthTex);
            	float3 ase_worldTangent = UnityObjectToWorldDir(v.tangent);
				o.tangent.xyz = ase_worldTangent;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldNormal.xyz = worldNormal;
            	float vertexTangentSign = v.tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 worldBitangent = cross( worldNormal, ase_worldTangent )*vertexTangentSign;
				o.worldBitangent.xyz = worldBitangent;
            	o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            	float2 uv_DepthTex = i.uv.xy;

            	float3 ase_worldTangent = i.tangent.xyz;
				float3 ase_worldNormal = i.worldNormal.xyz;
				float3 ase_worldBitangent = i.worldBitangent.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(i.worldPos.xyz);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_tanViewDir =  tanToWorld0 * ase_worldViewDir.x + tanToWorld1 * ase_worldViewDir.y  + tanToWorld2 * ase_worldViewDir.z;
				ase_tanViewDir = normalize(ase_tanViewDir);
            	float2 depth_uv = POM( _DepthTex, uv_DepthTex, ddx(uv_DepthTex), ddy(uv_DepthTex), ase_worldNormal, ase_worldViewDir, ase_tanViewDir, 128, 128, _Depth, 0.5, _DepthTex_ST.xy, float2(0,0), 0 );
                fixed4 col = tex2D(_EmissionTex, depth_uv);
            	col*=_EmissionColor;
				float4 rs = (float4(col.rgb , ( tex2D( _MaskTex, depth_uv ).a * _Aphla_scale )));

            	
                return rs;
            }
            ENDCG
        }
    }
}
