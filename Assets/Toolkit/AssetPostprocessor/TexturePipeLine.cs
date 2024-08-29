using UnityEditor;
using UnityEngine;

namespace Toolkit.AssetPostprocessor
{
    /// <summary>
    /// 图片导入自动设置
    /// </summary>
    public class TexturePipeLine : UnityEditor.AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            TextureImporter importer = assetImporter as TextureImporter;

            if (importer == null) return;
            
            if(importer.filterMode == FilterMode.Bilinear) return;

            importer.spriteImportMode = SpriteImportMode.Single;

            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = 2048;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            // UI图片关闭mipmap
            importer.mipmapEnabled = false;

            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.ApplyTextureType(TextureImporterType.Sprite);
            importer.SetTextureSettings(settings);
            
            Debug.Log($"{importer.assetPath} auto standardized setting");
        }
    }
}