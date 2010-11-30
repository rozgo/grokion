Shader "Rozgo/LiquidShade" {
Properties {
	_Color ("TintColor", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

Category {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	SubShader {
		
	Pass{
        SetTexture [_MainTex] {
            constantColor [_Color]
            Combine texture * constant DOUBLE, texture * constant
        }  
		}
	} 
}
}