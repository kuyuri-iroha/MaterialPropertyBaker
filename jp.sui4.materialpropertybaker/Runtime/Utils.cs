﻿using System.Text.RegularExpressions;
using UnityEngine;

namespace sui4.MaterialPropertyBaker
{
    public class Utils
    {
        public static void CreatePropertyBlockFromProfile(in BakedProperties preset, out MaterialPropertyBlock mpb)
        {
            mpb = new MaterialPropertyBlock();
            var matProps = preset.MaterialProps;
            foreach (var cProp in matProps.Colors)
            {
                mpb.SetColor(cProp.id, cProp.value);
            }

            foreach (var fProp in matProps.Floats)
                mpb.SetFloat(fProp.id, fProp.value);

            foreach (var iProp in matProps.Ints)
                mpb.SetInt(iProp.id, iProp.value);
        }
        
        public void CreatePropertyBlockFromProps(in MaterialProps props, out MaterialPropertyBlock mpb)
        {
            mpb = new MaterialPropertyBlock();
            UpdatePropertyBlockFromProps(props, ref mpb);
        }

        // 既存のPropertyBlockに追加する, プロパティが重複している場合は上書きする
        public void UpdatePropertyBlockFromProps(in MaterialProps props, ref MaterialPropertyBlock mpb)
        {
            foreach (var c in props.Colors)
                mpb.SetColor(c.id, c.value);
            foreach (var f in props.Floats)
                mpb.SetFloat(f.id, f.value);
            foreach (var i in props.Ints)
                mpb.SetInt(i.id, i.value);
        }
        
        public static string UnderscoresToSpaces(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // アンダースコアをスペースに置き換え
            string replaced = input.Replace('_', ' ');

            // 先頭がスペースの場合は消去
            if (replaced.Length > 0 && replaced[0] == ' ')
            {
                replaced = replaced.Substring(1);
            }

            return replaced;
        }
        
        public static string MakeFileNameSafe(string fileName)
        {
            // ファイル名に使えない記号: \ / : * ? " < > |
            string pattern = "[\\\\/:*?\"<>|]";

            // 正規表現を使って、指定した記号をハイフンに置き換える
            return Regex.Replace(fileName, pattern, "-");
        }
    }
}