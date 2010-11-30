Shader "OssianShaders/LightMap" {
	Properties {
		_Color ("Exposure", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Diffuse (RGB)", 2D) = "white" {}
		_LightMap ("Lightmap" , 2D) = "white"
	    _Texture2 ("Texture 2  (RGB)", 2D) = ""
   
	}
	SubShader {
		pass{
			BindChannels{
				    Bind "vertex", vertex
					Bind "texcoord", texcoord0
					Bind "texcoord1", texcoord1
				}
				
			SetTexture[_MainTex]{
				
				ConstantColor[_Color]
				combine constant * texture	
			}	
			SetTexture[_LightMap]{
				
				combine previous * texture DOUBLE
			}
		}
	} 
}
