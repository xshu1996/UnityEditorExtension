using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils
{
    public static class RenderTexturePool
    {
        private static readonly Dictionary<int, Queue<RenderTexture>> _pool = new Dictionary<int, Queue<RenderTexture>>();

        public static RenderTexture Get(int width, int height)
        {
            int key = width * 1000 + height; // 生成一个唯一的键

            if (!_pool.ContainsKey(key))
            {
                _pool[key] = new Queue<RenderTexture>();
            }

            if (_pool[key].Count > 0)
            {
                // 从对象池中获取 RenderTexture
                return _pool[key].Dequeue();
            }
            else
            {
                // 创建新的 RenderTexture
                RenderTexture renderTexture = new RenderTexture(width, height, 0)
                {
                    enableRandomWrite = true
                };
                renderTexture.Create();
                return renderTexture;
            }
        }

        public static void Recycle(RenderTexture renderTexture)
        {
            if (renderTexture == null) return;

            int key = renderTexture.width * 1000 + renderTexture.height;

            if (!_pool.ContainsKey(key))
            {
                _pool[key] = new Queue<RenderTexture>();
            }

            // 将 RenderTexture 返回到对象池
            renderTexture.Release();
            _pool[key].Enqueue(renderTexture);
        }
    }
}