Shader "Rozgo/MetalShade" { 
   Properties { 
      _MainTex ("Base (RGB)", 2D) = "white" 
      _MetalShade ("Metal Shade (RGBA)", 2D) = "white" { TexGen SphereMap}
   }
   SubShader { 
      Pass {
         SetTexture [_MainTex]
         
         SetTexture [_MetalShade]{
         combine texture + previous
         }	 
      } 
      
   } 
}
