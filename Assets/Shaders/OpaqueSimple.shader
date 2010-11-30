Shader "Rozgo/Opaque Simple" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
 
SubShader {
    Pass {
        Material {
        	Ambient(1,1,1,1)
        }
        ColorMaterial AmbientAndDiffuse
        Lighting off
        SetTexture [_MainTex] {
            Combine texture * primary, texture * primary
        }
        SetTexture [_MainTex] {
            constantColor [_Color]
            Combine previous * constant DOUBLE, previous * constant
        } 
    }
}
 
}