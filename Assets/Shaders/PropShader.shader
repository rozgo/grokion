Shader "Rozgo/Prop Shader" { 
   Properties { 
      _MainTex ("Base (RGBA)", 2D) = "white" {} 
      _Color ("TintColor", Color) = (1,1,1,1)
   }
   SubShader { 
      Pass {
         SetTexture [_MainTex] {
         constantColor [_Color]
         Combine constant lerp(texture) texture}
      } 
   } 
}
