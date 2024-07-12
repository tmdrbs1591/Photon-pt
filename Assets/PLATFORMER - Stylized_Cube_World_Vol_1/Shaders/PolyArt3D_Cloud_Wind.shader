// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PolyArt3D/Cloud Wind"
{
	Properties
	{
		_Smothness("Smothness", Float) = 1
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_Emission("Emission", Float) = 0.2
		_WindBend("WindBend", Range( 0 , 1)) = 0.02
		_WindFrequency("WindFrequency", Range( 0 , 1)) = 0.05
		_WindDirection("WindDirection", Range( 0 , 10)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
		};

		uniform float _WindDirection;
		uniform float _WindFrequency;
		uniform float _WindBend;
		uniform float4 _Color;
		uniform float _Emission;
		uniform float _Smothness;


		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
		{
			original -= center;
			float C = cos( angle );
			float S = sin( angle );
			float t = 1 - C;
			float m00 = t * u.x * u.x + C;
			float m01 = t * u.x * u.y - S * u.z;
			float m02 = t * u.x * u.z + S * u.y;
			float m10 = t * u.x * u.y + S * u.z;
			float m11 = t * u.y * u.y + C;
			float m12 = t * u.y * u.z - S * u.x;
			float m20 = t * u.x * u.z - S * u.y;
			float m21 = t * u.y * u.z + S * u.x;
			float m22 = t * u.z * u.z + C;
			float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float temp_output_122_0 = ( ( ase_vertex3Pos.y * cos( ( ( ( ase_worldPos.x + ase_worldPos.z ) * _WindFrequency ) + _Time.y ) ) ) * _WindBend );
			float4 appendResult123 = (float4(temp_output_122_0 , 0.0 , temp_output_122_0 , 0.0));
			float4 break127 = mul( appendResult123, unity_ObjectToWorld );
			float4 appendResult129 = (float4(break127.x , 0 , break127.z , 0.0));
			float3 rotatedValue130 = RotateAroundAxis( float3( 0,0,0 ), appendResult129.rgb, float3( 0,0,0 ), _WindDirection );
			v.vertex.xyz += rotatedValue130;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _Color.rgb;
			float3 temp_cast_1 = (_Emission).xxx;
			o.Emission = temp_cast_1;
			o.Smoothness = _Smothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-7;156;1920;863;3208.354;786.3152;2.334995;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;113;-2706.1,73.51656;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;116;-2601.395,256.4051;Inherit;False;Property;_WindFrequency;WindFrequency;4;0;Create;True;0;0;0;False;0;False;0.05;0.242;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;114;-2499.112,94.24118;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;118;-2412.395,351.4051;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-2344.396,108.4058;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;117;-2108.396,155.4057;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;121;-2002.396,26.40578;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CosOpNode;119;-1970.396,332.4052;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-1762.396,226.4057;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;126;-1667.396,370.4051;Inherit;False;Property;_WindBend;WindBend;3;0;Create;True;0;0;0;False;0;False;0.02;0.375;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-1493.396,243.4061;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;123;-1268.396,202.4056;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode;125;-1175.396,434.4052;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-1105.396,318.4051;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;127;-915.3966,318.4051;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.Vector3Node;132;-978.6069,130.0261;Inherit;False;Constant;_Vector0;Vector 0;12;0;Create;True;0;0;0;False;0;False;0,0,0;0,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;129;-671.5048,283.3531;Inherit;False;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;128;-874.945,537.9222;Inherit;False;Property;_WindDirection;WindDirection;5;0;Create;True;0;0;0;False;0;False;0;1.95;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;130;-501.5051,417.3532;Inherit;False;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-404.6007,73.1783;Inherit;False;Property;_Emission;Emission;2;0;Create;True;0;0;0;False;0;False;0.2;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;108;-394.1065,-111.6613;Inherit;False;Property;_Color;Color;1;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;61;-412.7964,151.5614;Inherit;False;Property;_Smothness;Smothness;0;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1.264471,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;PolyArt3D/Cloud Wind;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;7;False;-1;3;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;114;0;113;1
WireConnection;114;1;113;3
WireConnection;115;0;114;0
WireConnection;115;1;116;0
WireConnection;117;0;115;0
WireConnection;117;1;118;2
WireConnection;119;0;117;0
WireConnection;120;0;121;2
WireConnection;120;1;119;0
WireConnection;122;0;120;0
WireConnection;122;1;126;0
WireConnection;123;0;122;0
WireConnection;123;2;122;0
WireConnection;124;0;123;0
WireConnection;124;1;125;0
WireConnection;127;0;124;0
WireConnection;129;0;127;0
WireConnection;129;1;132;2
WireConnection;129;2;127;2
WireConnection;130;1;128;0
WireConnection;130;3;129;0
WireConnection;0;0;108;0
WireConnection;0;2;63;0
WireConnection;0;4;61;0
WireConnection;0;11;130;0
ASEEND*/
//CHKSM=C528E6E176EABF53637669C9D7B10CE4A403D900