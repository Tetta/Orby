Shader "Depth Mask" 
{
	SubShader 
	{
		Tags {"Queue" = "Geometry-100" }
		ZTest On
		ZWrite On
		ColorMask 0
		Pass {}
	}
}