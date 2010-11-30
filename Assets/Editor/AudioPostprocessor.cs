using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

class AudioPostprocessor : AssetPostprocessor {
	
#if UNITY_3_0
#else
//	void OnPreprocessAudio () {
//
//        string ext = System.IO.Path.GetExtension(assetPath);
//        if (ext == ".mp3") {
//            AudioImporter importer = assetImporter as AudioImporter;
//            if (importer.format == AudioImporterFormat.Uncompressed) {
//                importer.format = AudioImporterFormat.Automatic;
//                importer.decompressOnLoad = false;
//                AssetDatabase.ImportAsset(assetPath);
//            }
//        }
//        
//	}
#endif
	
}
