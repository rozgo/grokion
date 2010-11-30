Shader "Rozgo/Cut Simple" {
Properties {
      _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
   }
   Category {
      ZWrite Off
      Alphatest Greater 0
      Tags {Queue=Transparent}
      Blend SrcAlpha OneMinusSrcAlpha
       
      SubShader {
         Pass {
            Lighting Off
            SetTexture [_MainTex]
         }
      }
   }
}