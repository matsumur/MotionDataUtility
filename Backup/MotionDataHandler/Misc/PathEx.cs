using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;
using System.Text.RegularExpressions;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 内部で用いるオブジェクトパスの操作クラス
    /// </summary>
    public static class PathEx {
        /// <summary>
        /// 名前を正規化して返します．
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string NormalizePath(string name) {
            if(name == null)
                name = "";
            foreach(char c in InvalidChars) {
                name = name.Replace(c, ' ');
            }
            name = _removeSpaces.Replace(name, " ");
            name = name.Replace(AltPathSeparator, PathSeparator);
            var elems = name.Split(PathSeparator).Select(p => p.Trim()).Where(p => p != "" && p != ".").ToList();

            int upIndex;
            while(-1 != (upIndex = elems.IndexOf(".."))) {
                if(upIndex == 0) {
                    elems.RemoveAt(0);
                } else {
                    elems.RemoveRange(upIndex - 1, 2);
                }
            }
            if(elems.Count == 0)
                return "";
            name = CollectionEx.Join(PathSeparator, elems);
            return name;
        }

        public static readonly ReadOnlyCollection<char> InvalidChars = new ReadOnlyCollection<char>("*<>".ToList());
        private static readonly Regex _removeSpaces = new Regex("  +");
        /// <summary>
        /// 名前からディレクトリ名を返します．
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string DirName(string name) {
            name = NormalizePath(name);
            int index = name.LastIndexOf(PathSeparator);
            if(index == -1)
                return "";
            return name.Substring(0, index);
        }
        /// <summary>
        /// 名前からディレクトリ名を除いたものを返します．
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string BaseName(string name) {
            name = NormalizePath(name);
            int index = name.LastIndexOf(PathSeparator);
            if(index == -1)
                return name;
            return name.Substring(index + 1);
        }
        /// <summary>
        /// 指定された名前が指定されたグループに属するかを返します，
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="group">グループ</param>
        /// <returns></returns>
        public static bool IsSubPath(string name, string group) {
            group = NormalizePath(group);
            name = NormalizePath(name);
            if(group == "")
                return true;
            return name.StartsWith(group + PathSeparator);
        }
        /// <summary>
        /// 指定されたパスから指定された名前への相対名を返します．
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="from">相対名の元となる名前</param>
        /// <returns></returns>
        public static string GetRelativePath(string path, string fromPath) {
            path = NormalizePath(path);
            fromPath = NormalizePath(fromPath);
            if(fromPath == "")
                return path;
            if(path == fromPath)
                return "";
            if(IsSubPath(path, fromPath)) {
                return path.Substring(fromPath.Length + 1);
            }
            string common = GetCommonDir(path, fromPath);
            string toFrom = GetRelativePath(fromPath, common);
            string toName = GetRelativePath(path, common);
            int count = toFrom.Split(PathSeparator).Length;

            StringBuilder ret = new StringBuilder();
            for(int i = 0; i < count; i++) {
                if(i != 0)
                    ret.Append(PathSeparator);
                ret.Append("..");
            }
            ret.Append(toName);
            return ret.ToString();
        }


        /// <summary>
        /// 名前をPathSeparatorで結合します    
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static string CombineName(IList<string> names) {
            if(names == null)
                throw new ArgumentNullException("names", "'names' cannot be null");
            if(names.Count == 0)
                throw new ArgumentException("'names' must be have one or more names", "names");
            string ret = null;
            foreach(string name in names) {
                if(name == null)
                    throw new ArgumentException("'names' cannot contain null", "names");

                if(ret == null) {
                    ret = name;
                } else {
                    ret += PathSeparator + name;
                }
            }
            return NormalizePath(ret);
        }
        /// <summary>
        /// 名前をPathSeparatorで結合します    
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static string CombineName(params string[] names) {
            return CombineName((IList<string>)names);
        }

        /// <summary>
        /// 名前をPathSeparatorで分割します
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string[] SplitName(string name) {
            return NormalizePath(name).Split(PathSeparator);
        }

        /// <summary>
        /// 複数の名前に共通するディレクトリを返します．
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static string GetCommonDir(IList<string> names) {
            if(names == null)
                throw new ArgumentNullException("names", "'names' cannot be null");
            if(names.Count == 0)
                throw new ArgumentException("'names' must be have one or more names", "names");
            List<string[]> elemsList = names.Select(name => SplitName(DirName(name))).ToList();
            int minLength = elemsList.Select(elem => elem.Length).Min();
            List<string> commonDir = new List<string>();
            for(int i = 0; i < minLength; i++) {
                // 不一致のディレクトリ名があったら脱出
                string elem = elemsList[0][i];
                if(elemsList.Any(elems => elems[i] != elem))
                    break;
                commonDir.Add(elem);
            }
            if(commonDir.Count == 0)
                return "";
            return CombineName(commonDir);
        }
        /// <summary>
        /// 複数の名前に共通するディレクトリを返します．
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static string GetCommonDir(params string[] names) {
            return GetCommonDir((IList<string>)names);
        }

        /// <summary>
        /// 入力に応じて名前を返します
        /// </summary>
        /// <param name="prefix">新しい名前につける接頭語</param>
        /// <param name="refNames">関連するオブジェクトの名前のリスト</param>
        /// <returns></returns>
        public static string GiveName(string prefix, IEnumerable<string> refNames) {
            List<string> refNameList = refNames.ToList();
            if(refNameList == null)
                throw new ArgumentNullException("refNames", "'refNames' cannot be null");
            if(refNameList.Count == 0)
                return prefix;
            string dir = GetCommonDir(refNameList);
            IList<string> relRefNames = refNameList.Select(n => GetRelativePath(n, dir).Replace('/', '.')).ToList();
            if(refNameList.Count == 1) {
                return CombineName(dir, prefix + "(" + relRefNames[0] + ")");
            } else if(refNameList.Count == 2) {
                return CombineName(dir, prefix + "(" + relRefNames[0] + ", " + relRefNames[1] + ")");
            } else {
                return CombineName(dir, prefix + "(" + relRefNames[0] + ", ...)");
            }
        }

        public static string GiveName(string prefix, params string[] refNames) {
            return GiveName(prefix, (IList<string>)refNames);
        }

        public const char PathSeparator = '/';
        public const char AltPathSeparator = '\\';

    }
}
