using Microsoft.CodeAnalysis.Operations;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Luxcinder
{
    internal static class AssetExtensions
    {
        /// <summary>
        /// 将类型全名转换为相对路径（如"Luxcinder.Content.Menu.LuxcinderModMenu" -> "Content/Menu/"）
        /// </summary>
        private static string TypeFullNameToRelativePath(string fullName)
        {
            // 去除主命名空间
            int firstDot = fullName.IndexOf('.');
            if (firstDot < 0)
                return string.Empty;

            // 去掉类型名，只保留命名空间部分
            int lastDot = fullName.LastIndexOf('.');
            if (lastDot <= firstDot)
                return string.Empty;

            string ns = fullName.Substring(firstDot + 1, lastDot - firstDot - 1);
            // 替换.为/
            return ns.Replace('.', '/');
        }

        /// <summary>
        /// 将以当前类型命名空间所代表路径为相对路径，加载Texture2D资源
        /// </summary>
        /// <param name="modType">基本类型（注意是以这个类型的命名空间为准）</param>
        /// <param name="path">相对路径</param>
        /// <returns></returns>
        public static Asset<Texture2D> RequestModRelativeTexture(this IModType modType, string path)
        {
            string relativePath = TypeFullNameToRelativePath(modType.GetType().FullName);
            return modType.Mod.Assets.Request<Texture2D>(relativePath + "/" + path, AssetRequestMode.AsyncLoad);
        }

		/// <summary>
		/// 返回当前类型命名空间所代表相对路径
		/// </summary>
		/// <param name="modType">基本类型（注意是以这个类型的命名空间为准）</param>
		/// <returns></returns>
		public static string GetModRelativePath<T>()
        {
            return TypeFullNameToRelativePath(typeof(T).FullName);
        }

		/// <summary>
		/// 返回当前类型命名空间所代表相对路径，带Mod名
		/// </summary>
		/// <param name="modType">基本类型（注意是以这个类型的命名空间为准）</param>
		/// <returns></returns>
		public static string GetModRelativePathFull<T>()
		{
			return nameof(Luxcinder) + "/" + TypeFullNameToRelativePath(typeof(T).FullName);
		}

		public static Asset<Texture2D> RequestModRelativeTexturePathFull<T>(string path)
		{
			return ModContent.Request<Texture2D>(nameof(Luxcinder) + "/" + TypeFullNameToRelativePath(typeof(T).FullName) + "/" + path, AssetRequestMode.AsyncLoad);
		}

		public static Asset<Texture2D> RequestModRelativeTexture(this object obj, string path)
		{
			return ModContent.Request<Texture2D>(nameof(Luxcinder) + "/" + TypeFullNameToRelativePath(obj.GetType().FullName) + "/" + path, AssetRequestMode.AsyncLoad);
		}
	}
}
