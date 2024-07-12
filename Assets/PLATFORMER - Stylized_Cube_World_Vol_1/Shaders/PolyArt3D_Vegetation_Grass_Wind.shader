// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PolyArt3D/Vegetation Grass Wind"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		_Smothness("Smothness", Float) = 1
		_Normal("Normal", 2D) = "bump" {}
		_DeSaturate("DeSaturate", Float) = 0.2
		[HDR]_Color("Color", Color) = (1,1,1,1)
		[HDR]_SSS_Color("SSS_Color", Color) = (1,1,1,1)
		_Emission("Emission", Float) = 0.2
		_Wind_Movement("Wind_Movement", Vector) = (10,0,0,0)
		_Wind_Density("Wind_Density", Float) = 0.05
		_Wind_Strength("Wind_Strength", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Wind_Density;
		uniform float2 _Wind_Movement;
		uniform float _Wind_Strength;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Color;
		uniform float4 _SSS_Color;
		uniform float _Emission;
		uniform float _DeSaturate;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform float _Smothness;


		float2 voronoihash351( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi351( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -3; j <= 3; j++ )
			{
				for ( int i = -3; i <= 3; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash351( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.871 * pow( ( pow( abs( r.x ), 5 ) + pow( abs( r.y ), 5 ) ), 0.200 );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float time351 = 45.55;
			float2 voronoiSmoothId351 = 0;
			float2 coords351 = (ase_vertex3Pos*1.0 + float3( ( _Time.y * _Wind_Movement ) ,  0.0 )).xy * _Wind_Density;
			float2 id351 = 0;
			float2 uv351 = 0;
			float fade351 = 0.5;
			float voroi351 = 0;
			float rest351 = 0;
			for( int it351 = 0; it351 <4; it351++ ){
			voroi351 += fade351 * voronoi351( coords351, time351, id351, uv351, 0,voronoiSmoothId351 );
			rest351 += fade351;
			coords351 *= 2;
			fade351 *= 0.5;
			}//Voronoi351
			voroi351 /= rest351;
			float3 break355 = ase_vertex3Pos;
			float4 appendResult361 = (float4(( ( ( voroi351 - 0.5 ) * _Wind_Strength ) + break355.x ) , break355.y , break355.z , 0.0));
			float4 lerpResult363 = lerp( float4( ase_vertex3Pos , 0.0 ) , appendResult361 , v.texcoord.xy.y);
			v.vertex.xyz = lerpResult363.xyz;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode4 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ( tex2DNode4 * _Color ).rgb;
			float3 desaturateInitialColor15 = ( ( saturate( tex2DNode4 ) * _SSS_Color ) * _Emission ).rgb;
			float desaturateDot15 = dot( desaturateInitialColor15, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar15 = lerp( desaturateInitialColor15, desaturateDot15.xxx, _DeSaturate );
			o.Emission = desaturateVar15;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			float4 tex2DNode14 = tex2D( _Metallic, uv_Metallic );
			o.Metallic = tex2DNode14.r;
			o.Smoothness = ( tex2DNode14.a * _Smothness );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-7;96;1920;923;2668.805;957.6862;2.198045;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;345;-2772.245,1450.521;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;346;-2792.65,1522.625;Inherit;False;Property;_Wind_Movement;Wind_Movement;10;0;Create;True;0;0;0;False;0;False;10,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PosVertexDataNode;367;-2607.675,1021.067;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;347;-2521.351,1516.161;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;349;-2193.346,1653.531;Inherit;False;Property;_Wind_Density;Wind_Density;11;0;Create;True;0;0;0;False;0;False;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;350;-2274.328,1340.327;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;1;-1940.135,-500.0289;Inherit;False;1533.258;1229.961;Inspired by 2Side Sample by The Four Headed Cat;15;4;58;12;60;62;18;61;14;16;63;19;107;109;15;108;Two Sided Shader using Switch by Face;1,1,1,1;0;0
Node;AmplifyShaderEditor.VoronoiNode;351;-1908.633,1427.34;Inherit;True;2;4;5;0;4;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;45.55;False;2;FLOAT;5.13;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;352;-1646.045,1695.361;Inherit;False;Property;_Wind_Strength;Wind_Strength;12;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;354;-1684.135,1448.422;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-1693.466,-451.5098;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;368;-1761.181,2005.391;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;355;-1464.643,1838.133;Inherit;True;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;19;-1292.187,-355.5689;Inherit;False;Property;_SSS_Color;SSS_Color;8;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;107;-1204.526,-443.5308;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;356;-1442.925,1423.555;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;357;-1398.255,2097.389;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1040.758,-390.8941;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;358;-1202.747,1488.092;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-993.717,-270.2185;Inherit;False;Property;_Emission;Emission;9;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-1298.853,308.9009;Inherit;True;Property;_Metallic;Metallic;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;361;-1010.035,1806.422;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;108;-1711.462,-193.6971;Inherit;False;Property;_Color;Color;7;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-775.9842,-359.3428;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-901.9286,431.0112;Inherit;False;Property;_Smothness;Smothness;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-754.5618,-168.1839;Inherit;False;Property;_DeSaturate;DeSaturate;6;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;359;-1005.986,2070.822;Inherit;True;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;363;-504.9122,1382.11;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;12;-1305.661,521.1876;Inherit;True;Property;_OpacityMask;Opacity Mask;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;58;-1280.656,15.6074;Inherit;True;Property;_Normal;Normal;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-1414.55,-175.2541;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-619.7142,269.1137;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;15;-569.5038,-254.2792;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1.264471,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;PolyArt3D/Vegetation Grass Wind;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;7;False;-1;3;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Absolute;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;347;0;345;0
WireConnection;347;1;346;0
WireConnection;350;0;367;0
WireConnection;350;2;347;0
WireConnection;351;0;350;0
WireConnection;351;2;349;0
WireConnection;354;0;351;0
WireConnection;355;0;368;0
WireConnection;107;0;4;0
WireConnection;356;0;354;0
WireConnection;356;1;352;0
WireConnection;16;0;107;0
WireConnection;16;1;19;0
WireConnection;358;0;356;0
WireConnection;358;1;355;0
WireConnection;361;0;358;0
WireConnection;361;1;355;1
WireConnection;361;2;355;2
WireConnection;62;0;16;0
WireConnection;62;1;63;0
WireConnection;359;0;357;0
WireConnection;363;0;367;0
WireConnection;363;1;361;0
WireConnection;363;2;359;1
WireConnection;109;0;4;0
WireConnection;109;1;108;0
WireConnection;60;0;14;4
WireConnection;60;1;61;0
WireConnection;15;0;62;0
WireConnection;15;1;18;0
WireConnection;0;0;109;0
WireConnection;0;1;58;0
WireConnection;0;2;15;0
WireConnection;0;3;14;0
WireConnection;0;4;60;0
WireConnection;0;11;363;0
ASEEND*/
//CHKSM=671149BDAD0D7CDF61EEFAD0D30E823D16F3A5E6