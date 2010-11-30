Shader "Rozgo/Desinigrate" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
	_BlendTex ("Blend Alpha (A)", 2D) = "white" {TexGen EyeLinear}
	_AlphaRange ("Alpha Range", Range (0.0, 1.0)) = 1.0
}
 Category {
 Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	ZWrite On
	Alphatest Greater [_AlphaRange]

SubShader {
    Pass {
		Cull Off
       
        ColorMaterial AmbientAndDiffuse
        Lighting off
        SetTexture [_MainTex] {
			constantColor [_Color]
            Combine constant * texture DOUBLE 
        }
		SetTexture [_BlendTex] {
			Combine previous - texture DOUBLE, texture
		}
        
    }
}
 
}
}


/*

Shader "Rozgo/Desinigrate" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
	_BlendTex ("Blend Alpha (A)", 2D) = "white" {TexGen EyeLinear}
	_AlphaRange ("Alpha Range", Range (0.0, 1.0)) = 1.0
}
 Category {
 Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	ZWrite On
	Alphatest Greater [_AlphaRange]

SubShader {
    Pass {
       
        ColorMaterial AmbientAndDiffuse
        Lighting off
        SetTexture [_MainTex] {
            
        }
		SetTexture [_BlendTex] {
			Combine previous - texture DOUBLE
		}
        
    }
}
 
}
}

*/