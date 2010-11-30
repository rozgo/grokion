using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

class TexturePostprocessor : AssetPostprocessor {
	
//#if UNITY_3_0
//#else
//	void OnPreprocessTexture () {
//		//Debug.Log(assetPath);
//        TextureImporter importer = assetImporter as TextureImporter;
//        if (importer.textureFormat == TextureImporterFormat.DXT1) {
//            importer.textureFormat = importer.recommendedTextureFormat;
//        }
//        else if (importer.textureFormat == TextureImporterFormat.DXT5) {
//            importer.textureFormat = importer.recommendedTextureFormat;
//        }
//	}
//	
//	bool IsSq2 (int num) {
//		for (int i = 0; i < 13; ++i) {
//			//Debug.Log(Mathf.Pow(2, i));
//			if (num == Mathf.Pow(2, i)) {
//				return true;
//			}
//		}
//		return false;
//	}
//	
//	void OnPostprocessTexture (Texture2D texture) {
//		//Debug.Log(texture.width);
//		//Debug.Log(IsSq2(texture.width));
//		TextureImporter importer = assetImporter as TextureImporter;
//		if (!IsSq2(texture.width) || !IsSq2(texture.height)) {
//			//Debug.Log(importer.textureFormat);
//			if (importer.textureFormat == TextureImporterFormat.Automatic) {
//				
//	        	importer.textureFormat = TextureImporterFormat.RGB16;
//	        	importer.mipmapEnabled = false;
//	        	AssetDatabase.ImportAsset(assetPath);
//	        }
//		}
//	}
//#endif
	
}
