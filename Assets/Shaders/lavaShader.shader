Shader "Rozgo/LavaShader" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _FogColor ("CustomFogColor", Color) = (	1,0.20,0,1)
    _FogDense ("FogDensity", Range (0.0, 0.2)) = 0.05
}
 
SubShader {
    Pass {
    	Fog {
    		Mode Exp2
    		Color [_FogColor]
    		Density [_FogDense]
    	}
        Material {
            Shininess [_Shininess]
            Specular [_SpecColor]
            Emission [_Emission]    
        }
        ColorMaterial AmbientAndDiffuse
        Lighting Off
        SetTexture [_MainTex] {
            Combine texture * primary, texture * primary
        }
        SetTexture [_MainTex] {
            constantColor [_Color]
            Combine previous * constant DOUBLE, previous * constant
        } 
    }
}
 
Fallback " VertexLit", 1
}