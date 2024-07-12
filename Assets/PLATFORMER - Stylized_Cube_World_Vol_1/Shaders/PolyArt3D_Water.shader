// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PolyArt3D/Water"
{
	Properties
	{
		_DeepColor("Deep Color", Color) = (0,0,0,0)
		_ShalowColor("Shalow Color", Color) = (1,1,1,0)
		_NormalScale01("Normal Scale 01", Float) = 0.35
		_NormalScale02("Normal Scale 02", Float) = 0.35
		_WaterDepth("Water Depth", Float) = 0
		_WaterFalloff("Water Falloff", Float) = 0
		_WaterMetallic("Water Metallic", Float) = 0.2
		_WaterSmoothness("Water Smoothness", Float) = 0.95
		_Distortion("Distortion", Float) = 0.5
		_DepthFafeDistance("DepthFafeDistance", Float) = 0
		_T_Water_N("T_Water_N", 2D) = "bump" {}
		_Normal01X("Normal 01 X", Float) = -0.1
		_Normal01Y("Normal 01 Y", Float) = -0.3
		_Normal02X("Normal 02 X", Float) = -0.1
		_Normal02Y("Normal 02 Y", Float) = -0.3
		_Rotate("Rotate", Float) = 1.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Back
		ZWrite On
		ZTest LEqual
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform sampler2D _T_Water_N;
		uniform half _Normal01X;
		uniform half _Normal01Y;
		uniform half _Rotate;
		uniform float _NormalScale01;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform half _DepthFafeDistance;
		uniform half _Normal02X;
		uniform half _Normal02Y;
		uniform float _NormalScale02;
		uniform float4 _DeepColor;
		uniform float4 _ShalowColor;
		uniform float _WaterDepth;
		uniform float _WaterFalloff;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _Distortion;
		uniform float _WaterMetallic;
		uniform float _WaterSmoothness;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			half2 appendResult236 = (half2(_Normal01X , _Normal01Y));
			half2 panner22 = ( 1.0 * _Time.y * appendResult236 + i.uv_texcoord);
			float cos247 = cos( _Rotate );
			float sin247 = sin( _Rotate );
			half2 rotator247 = mul( panner22 - float2( 0,0 ) , float2x2( cos247 , -sin247 , sin247 , cos247 )) + float2( 0,0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			half4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth224 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			half distanceDepth224 = saturate( abs( ( screenDepth224 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFafeDistance ) ) );
			half DepthFades222 = distanceDepth224;
			half2 appendResult238 = (half2(_Normal02X , _Normal02Y));
			half2 panner19 = ( 1.0 * _Time.y * appendResult238 + i.uv_texcoord);
			float cos246 = cos( _Rotate );
			float sin246 = sin( _Rotate );
			half2 rotator246 = mul( panner19 - float2( 0,0 ) , float2x2( cos246 , -sin246 , sin246 , cos246 )) + float2( 0,0 );
			half3 temp_output_24_0 = BlendNormals( UnpackScaleNormal( tex2D( _T_Water_N, rotator247 ), ( _NormalScale01 * DepthFades222 ) ) , UnpackScaleNormal( tex2D( _T_Water_N, rotator246 ), ( _NormalScale02 * DepthFades222 ) ) );
			o.Normal = temp_output_24_0;
			half eyeDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			half temp_output_94_0 = saturate( pow( ( abs( ( eyeDepth1 - ase_screenPos.w ) ) + _WaterDepth ) , _WaterFalloff ) );
			half4 lerpResult13 = lerp( _DeepColor , _ShalowColor , temp_output_94_0);
			half4 lerpResult117 = lerp( lerpResult13 , float4(1,1,1,0) , float4( 0,0,0,0 ));
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			half4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor65 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( half3( (ase_grabScreenPosNorm).xy ,  0.0 ) + ( temp_output_24_0 * _Distortion ) ).xy);
			half4 lerpResult93 = lerp( lerpResult117 , screenColor65 , temp_output_94_0);
			o.Albedo = lerpResult93.rgb;
			o.Metallic = _WaterMetallic;
			o.Smoothness = _WaterSmoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1329;73;1051;615;753.9536;1759.016;1.078748;False;False
Node;AmplifyShaderEditor.RangedFloatNode;223;-743.0955,-1435.43;Inherit;False;Property;_DepthFafeDistance;DepthFafeDistance;9;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;224;-519.7004,-1491.403;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;166;-2010.745,-176.8604;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;240;-873.171,-720.8715;Inherit;False;Property;_Normal02Y;Normal 02 Y;14;0;Create;True;0;0;0;False;0;False;-0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;237;-1060.506,-1039.295;Inherit;False;Property;_Normal01Y;Normal 01 Y;12;0;Create;True;0;0;0;False;0;False;-0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;239;-871.809,-817.4904;Inherit;False;Property;_Normal02X;Normal 02 X;13;0;Create;True;0;0;0;False;0;False;-0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;235;-1059.144,-1135.914;Inherit;False;Property;_Normal01X;Normal 01 X;11;0;Create;True;0;0;0;False;0;False;-0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;236;-835.9766,-1081.48;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;238;-648.6416,-763.0565;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScreenDepthNode;1;-1781.601,-155.6997;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;-1993.601,-9.1996;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-1052.753,-951.9941;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;222;-214.1087,-1485.111;Inherit;False;DepthFades;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;243;-1064.363,-562.1664;Inherit;False;222;DepthFades;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-787.7744,-1262.742;Float;False;Property;_NormalScale01;Normal Scale 01;2;0;Create;True;0;0;0;False;0;False;0.35;0.35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;22;-613.2062,-1032.484;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;245;-654.5981,-624.6252;Inherit;False;Property;_Rotate;Rotate;15;0;Create;True;0;0;0;False;0;False;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;230;-796.2829,-1173.129;Inherit;False;222;DepthFades;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-1055.854,-651.7794;Float;False;Property;_NormalScale02;Normal Scale 02;3;0;Create;True;0;0;0;False;0;False;0.35;0.35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;3;-1574.201,-110.3994;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;19;-610.9061,-919.9849;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.04,0.04;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;231;-598.5461,-1229.927;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-866.6261,-618.9644;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;89;-1389.004,-112.5834;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-813.7005,-128.1996;Float;False;Property;_WaterDepth;Water Depth;4;0;Create;True;0;0;0;False;0;False;0;0.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;247;-417.5037,-1186.788;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;246;-447.5425,-691.1406;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;233;-245.0176,-806.1205;Inherit;True;Property;_T_Water_N;T_Water_N;10;0;Create;True;0;0;0;False;0;False;-1;b98a2d3f2caa21840a115639f49af256;b98a2d3f2caa21840a115639f49af256;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;88;-632.0056,-204.5827;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;234;-251.0176,-1049.12;Inherit;True;Property;_TextureSample0;Texture Sample 0;10;0;Create;True;0;0;0;False;0;False;-1;b98a2d3f2caa21840a115639f49af256;b98a2d3f2caa21840a115639f49af256;True;0;True;bump;Auto;True;Instance;233;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-636.2005,-79.20019;Float;False;Property;_WaterFalloff;Water Falloff;5;0;Create;True;0;0;0;False;0;False;0;-1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;24;170.697,-879.6849;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;97;710.096,-1203.183;Float;False;Property;_Distortion;Distortion;8;0;Create;True;0;0;0;False;0;False;0.5;0.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;164;511.3026,-1442.425;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;87;-455.8059,-118.1832;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;-455.0999,-328.3;Float;False;Property;_ShalowColor;Shalow Color;1;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.2539999,0.7767875,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;888.1974,-1279.783;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;94;-249.5044,-96.98394;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;-473.8899,-520.1789;Float;False;Property;_DeepColor;Deep Color;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0.2792452,0.6415094,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;165;814.6503,-1385.291;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;108;58.99682,146.0182;Float;False;Constant;_Color0;Color 0;-1;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;96;1041.296,-1346.683;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;13;151.7696,-195.6007;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;65;1232.797,-1350.483;Float;False;Global;_WaterGrab;WaterGrab;-1;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;117;333.6723,-24.78498;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;104;1540.134,-531.443;Float;False;Property;_WaterMetallic;Water Metallic;6;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;93;1559.196,-1006.285;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;26;1544.303,-432.4778;Float;False;Property;_WaterSmoothness;Water Smoothness;7;0;Create;True;0;0;0;False;0;False;0.95;0.95;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1878.601,-708.1998;Half;False;True;-1;2;ASEMaterialInspector;0;0;Standard;PolyArt3D/Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;1;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;False;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;224;0;223;0
WireConnection;236;0;235;0
WireConnection;236;1;237;0
WireConnection;238;0;239;0
WireConnection;238;1;240;0
WireConnection;1;0;166;0
WireConnection;222;0;224;0
WireConnection;22;0;21;0
WireConnection;22;2;236;0
WireConnection;3;0;1;0
WireConnection;3;1;2;4
WireConnection;19;0;21;0
WireConnection;19;2;238;0
WireConnection;231;0;48;0
WireConnection;231;1;230;0
WireConnection;241;0;242;0
WireConnection;241;1;243;0
WireConnection;89;0;3;0
WireConnection;247;0;22;0
WireConnection;247;2;245;0
WireConnection;246;0;19;0
WireConnection;246;2;245;0
WireConnection;233;1;246;0
WireConnection;233;5;241;0
WireConnection;88;0;89;0
WireConnection;88;1;6;0
WireConnection;234;1;247;0
WireConnection;234;5;231;0
WireConnection;24;0;234;0
WireConnection;24;1;233;0
WireConnection;87;0;88;0
WireConnection;87;1;10;0
WireConnection;98;0;24;0
WireConnection;98;1;97;0
WireConnection;94;0;87;0
WireConnection;165;0;164;0
WireConnection;96;0;165;0
WireConnection;96;1;98;0
WireConnection;13;0;12;0
WireConnection;13;1;11;0
WireConnection;13;2;94;0
WireConnection;65;0;96;0
WireConnection;117;0;13;0
WireConnection;117;1;108;0
WireConnection;93;0;117;0
WireConnection;93;1;65;0
WireConnection;93;2;94;0
WireConnection;0;0;93;0
WireConnection;0;1;24;0
WireConnection;0;3;104;0
WireConnection;0;4;26;0
ASEEND*/
//CHKSM=BC80A83FDFEE28BF15273FDAD820264C8989A114