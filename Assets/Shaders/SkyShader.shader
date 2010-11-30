Shader "Rozgo/SkyShader"{

	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Rotation ("CloudMap (RGB)", 2D) = "white" {}
   
	}
	SubShader {
		pass{
			Fog {
    		Mode Off
			}
			
			BindChannels{
				    Bind "vertex", vertex
					Bind "texcoord", texcoord0
					Bind "texcoord1", texcoord1
				}
				
			SetTexture[_MainTex]{
				
				ConstantColor[_Color]
				combine constant * texture	
			}	
			SetTexture[_Rotation]{
				
				combine previous + texture
			}
		}
	} 
}
