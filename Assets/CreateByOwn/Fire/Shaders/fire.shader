Shader "Unlit/fire"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    [HDR]_Color("Main Color", Color) = (1,0,0,1)
    	_lerpUVNoise("lerpUVNoise",Range(0,1))=0.1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"
        }
        Cull Off
        Lighting Off
        ZWrite Off
    	// ZTest Less
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
            
            float snoise( float2 v )
			{
				const float4 C = float4( 0.21132, 0.3660, -0.577350, 0.02439);
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284 - 0.8537 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
            float2 hash22(float2 p)
			{
			    p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
			    return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
			}

			float SimplexNoise(float2 p)
			{
			    const float K1 = 0.366025404; // (sqrt(3)-1)/2;
			    const float K2 = 0.211324865; // (3-sqrt(3))/6;
			    
			    float2 i = floor(p + (p.x + p.y) * K1);
			    
			    float2 a = p - (i - (i.x + i.y) * K2);
			    float2 o = (a.x < a.y) ? float2(0, 1) : float2(1, 0);
			    float2 b = a - o + K2;
			    float2 c = a - 1 + 2 * K2;
			    
			    float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0);
			    float3 n = pow(h, 4) * float3(dot(a, hash22(i)), dot(b, hash22(i + o)),
			        dot(c, hash22(i + 1)));
			    
			    return dot(float3(70, 70, 70), n);
			}
            
            float2 random2(float2 p)
			{
			    return frac(sin(float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)))) *
			        43758.5453);
			}

			float voronoi(float2 p,float num)
			{
			    float min_dist = 1000;
			    float2 pi = floor(p);
			    
			    for (int m = -1; m <= 1; m++)
			    {
			        for (int n = -1; n <= 1; n++)
			        {
			            float2 sp = (pi + float2(m, n));
			            float2 pos = 0;
			            float factor = num;
			            pos = (sp + factor) % factor;
			            
			            sp += random2(pos);
			            float dist = distance(p, sp);
			            min_dist = min(min_dist, dist);
			        }
			    }
			    return min_dist;
			}
			            
            v2f vert (appdata v)
			{
   				v2f o;
   				float3 forward=mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1)).xyz;
   				forward.y=0;
   				forward=normalize(forward);
   				float3 up=abs(forward.y)>0.999?float3(0,0,1):float3(0,1,0);
   				float3 right=normalize(cross(up,forward));
   				up=normalize(cross(forward,right));
   				float3 vertex=v.vertex.x*right+v.vertex.y*up;
    			o.vertex = UnityObjectToClipPos(vertex);
    			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    			return o;
			}
			
            fixed4 _Color;
            float _lerpUVNoise;
            fixed4 frag (v2f i) : SV_Target
            {
                
            	float2 uv=i.uv;
            	float2 uv2=i.uv;
            	float2 uv3=i.uv;
            	float2 uv4=i.uv;
            	uv=uv+float2(0,-0.5*_Time.y);
            	float scale_noise=1.329;
            	//float lerp_uv_noise=0.117;
            	float noise=SimplexNoise(uv*scale_noise);
            	noise=noise*0.5+0.5;
            	uv=lerp(i.uv,float2(noise,noise),_lerpUVNoise);

            	fixed4 col = tex2D(_MainTex, uv);
            	float time24 = 0.0;
				float2 voronoiSmoothId0 = 0;
            	uv2=uv2+float2(0,-0.5*_Time.y);
				float2 coords24 = uv2 * 2.972135;
				float2 id24 = 0;
				float2 uv24 = 0;
				float voroi24 = voronoi(coords24,2.97);
            	float a1= (1-voroi24)*noise;
            	float4 col2=fixed4(col.rgb*a1,col.a*a1);
            	
            	
            	return col2*_Color;
            }
            ENDCG
        }
    }
}
