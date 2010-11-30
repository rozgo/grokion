Shader "Rozgo/Enemy Shader" { 
   Properties { 
      _MainTex ("Base (RGBA)", 2D) = "black" {} 
      _Color ("AmbientColor", Color) = (1,1,1,1)
	  _TintColor ("TintColor", Color) = (0,0,0,0)
   }
   SubShader { 
      Pass {
         SetTexture [_MainTex] {
         constantColor [_TintColor]
         Combine constant lerp(texture) texture}
		 
		 SetTexture [_MainTex] {
         constantColor [_Color]
         combine constant * previous  DOUBLE}
      } 
   } 
}
