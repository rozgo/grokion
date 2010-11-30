Shader "Rozgo/WaterLightMapped"{

	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("LightMap (RGB)", 2D) = "white" {}
		_Rotation ("Water(RGB)", 2D) = "white" {}
		_FogColor ("CustomFogColor", Color) = (	1,1,1,1)
    	_FogDense ("FogDensity", Range (0.0, 0.2)) = 0.05
   
	}
	SubShader {
		pass{
			Fog {
    		Mode Exp2
    		Color [_FogColor]
    		Density [_FogDense]
    	}
			
			BindChannels{
				    Bind "vertex", vertex
					Bind "texcoord", texcoord0
					Bind "texcoord1", texcoord1
				}
				
			SetTexture[_Rotation]{
				
				ConstantColor[_Color]
				combine constant * texture	
			}	
			SetTexture[_MainTex]{
				
				combine previous * texture DOUBLE
			}
		}
	} 
}
