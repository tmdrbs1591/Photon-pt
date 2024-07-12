// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PolyArt3D/Vegetation Wind"
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
		_Wind("Wind", 2D) = "white" {}
		_WindSpeed("Wind Speed", Range( 0 , 5)) = 0
		_WindHeight("Wind Height", Range( 0 , 5)) = 0
		_Emission("Emission", Float) = 0.2
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

		uniform sampler2D _Wind;
		uniform float _WindSpeed;
		uniform float _WindHeight;
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

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 speed83 = ( _Time * _WindSpeed );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float2 uv_TexCoord89 = v.texcoord.xy + ( speed83 + (ase_vertex3Pos).y ).xy;
			float3 ase_vertexNormal = v.normal.xyz;
			float3 VertexAnimation96 = ( ( tex2Dlod( _Wind, float4( uv_TexCoord89, 0, 1.0) ).r - 0.5 ) * ( ase_vertexNormal * _WindHeight ) );
			v.vertex.xyz += VertexAnimation96;
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
-7;108;1920;911;2369.262;503.5315;1.593102;True;False
Node;AmplifyShaderEditor.CommentaryNode;80;-3291.481,877.7915;Inherit;False;914.394;362.5317;Comment;4;92;87;84;83;Wind Speed;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-3241.481,1125.324;Float;False;Property;_WindSpeed;Wind Speed;10;0;Create;True;0;0;0;False;0;False;0;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;92;-3170.583,927.7917;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-2841.546,1062.813;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;79;-3424.973,1420.379;Inherit;False;2332.979;427.6435;Comment;12;96;95;94;93;88;90;81;89;86;85;91;82;Vertex Animation;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;82;-3374.973,1569.842;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;83;-2620.086,970.4675;Float;False;speed;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-3051.351,1509.561;Inherit;False;83;speed;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;91;-3136.208,1589.114;Inherit;False;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-2850.53,1506.278;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;89;-2661.918,1470.379;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;1;-1914.802,-500.0289;Inherit;False;1533.258;1229.961;Inspired by 2Side Sample by The Four Headed Cat;15;4;58;12;60;62;18;61;14;16;63;19;107;108;109;15;Two Sided Shader using Switch by Face;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;4;-1889.862,-432.7598;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;81;-2027.232,1576.555;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;88;-2382.454,1476.401;Inherit;True;Property;_Wind;Wind;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;90;-2071.548,1732.366;Float;False;Property;_WindHeight;Wind Height;11;0;Create;True;0;0;0;False;0;False;0;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;93;-1694.874,1503.971;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-1700.178,1657.637;Inherit;False;2;2;0;FLOAT3;1,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;107;-1179.193,-443.5308;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;19;-1238.862,-365.5661;Inherit;False;Property;_SSS_Color;SSS_Color;8;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;-968.3844,-270.2185;Inherit;False;Property;_Emission;Emission;12;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1532.87,1636.502;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1015.425,-390.8941;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;108;-1831.332,-191.4299;Inherit;False;Property;_Color;Color;7;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-750.6516,-359.3428;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;96;-1320.534,1642.031;Float;False;VertexAnimation;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-729.2292,-168.1839;Inherit;False;Property;_DeSaturate;DeSaturate;6;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-1273.52,308.9009;Inherit;True;Property;_Metallic;Metallic;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;61;-876.596,431.0112;Inherit;False;Property;_Smothness;Smothness;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;58;-1255.324,15.6074;Inherit;True;Property;_Normal;Normal;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-594.3816,269.1137;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;15;-544.1712,-254.2792;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;97;-731.1131,1053.22;Inherit;False;96;VertexAnimation;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;12;-1280.328,521.1876;Inherit;True;Property;_OpacityMask;Opacity Mask;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-1446.417,-220.7542;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1.264471,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;PolyArt3D/Vegetation Wind;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;7;False;-1;3;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;84;0;92;0
WireConnection;84;1;87;0
WireConnection;83;0;84;0
WireConnection;91;0;82;0
WireConnection;86;0;85;0
WireConnection;86;1;91;0
WireConnection;89;1;86;0
WireConnection;88;1;89;0
WireConnection;93;0;88;1
WireConnection;94;0;81;0
WireConnection;94;1;90;0
WireConnection;107;0;4;0
WireConnection;95;0;93;0
WireConnection;95;1;94;0
WireConnection;16;0;107;0
WireConnection;16;1;19;0
WireConnection;62;0;16;0
WireConnection;62;1;63;0
WireConnection;96;0;95;0
WireConnection;60;0;14;4
WireConnection;60;1;61;0
WireConnection;15;0;62;0
WireConnection;15;1;18;0
WireConnection;109;0;4;0
WireConnection;109;1;108;0
WireConnection;0;0;109;0
WireConnection;0;1;58;0
WireConnection;0;2;15;0
WireConnection;0;3;14;0
WireConnection;0;4;60;0
WireConnection;0;11;97;0
ASEEND*/
//CHKSM=A7E6A21549B389719B12D1BCBE9AB4A1956F6A48