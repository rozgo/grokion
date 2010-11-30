Shader "Rozgo/AdditiveSimple" {
    Properties {
        _MainTex ("Texture to blend", 2D) = "black" {}
    }
    SubShader {
 		zWrite On
        Blend One One
        Pass{
        	Tags {"Queue" = "Transparent" }
        SetTexture [_MainTex]
        }
    }
}