﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace sui4.MaterialPropertyBaker
{
    public class Utils
    {
        public static void CreatePropertyBlockFromProfile(out MaterialPropertyBlock mpb,
            in BakedMaterialProperty preset)
        {
            mpb = new MaterialPropertyBlock();
            var matProps = preset.MaterialProps;
            UpdatePropertyBlockFromProps(ref mpb, matProps);
        }

        public static void CreatePropertyBlockFromProps(out MaterialPropertyBlock mpb, in MaterialProps props)
        {
            mpb = new MaterialPropertyBlock();
            UpdatePropertyBlockFromProps(ref mpb, props);
        }

        // 既存のPropertyBlockに追加する, プロパティが重複している場合は上書きする
        public static void UpdatePropertyBlockFromProps(ref MaterialPropertyBlock mpb, in MaterialProps props)
        {
            foreach (var c in props.Colors)
                mpb.SetColor(c.ID, c.Value);
            foreach (var f in props.Floats)
                mpb.SetFloat(f.ID, f.Value);
            foreach (var t in props.Textures)
            {
                if (t.Value != null)
                    mpb.SetTexture(t.ID, t.Value);
            }
        }

        public static void UpdatePropertyBlockFromDict(ref MaterialPropertyBlock mpb, Dictionary<int, Color> cPropMap,
            Dictionary<int, float> fPropMap, Dictionary<int, Texture> tPropMap)
        {
            foreach (var (shaderID, value) in cPropMap) mpb.SetColor(shaderID, value);
            foreach (var (shaderID, value) in fPropMap) mpb.SetFloat(shaderID, value);
            foreach (var (shaderID, value) in tPropMap)
            {
                if (value != null)
                    mpb.SetTexture(shaderID, value);
            }
        }

        public static string UnderscoresToSpaces(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // アンダースコアをスペースに置き換え
            var replaced = input.Replace('_', ' ');

            // 先頭がスペースの場合は消去
            if (replaced.Length > 0 && replaced[0] == ' ') replaced = replaced.Substring(1);

            return replaced;
        }

        public static string MakeFileNameSafe(string fileName)
        {
            // ファイル名に使えない記号: \ / : * ? " < > |
            var pattern = "[\\\\/:*?\"<>|]";

            // 正規表現を使って、指定した記号をハイフンに置き換える
            return Regex.Replace(fileName, pattern, "-");
        }
    }
}