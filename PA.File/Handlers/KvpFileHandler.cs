using System;
using System.Collections.Generic;
using System.IO;

namespace PA.File.Handlers
{
    public class KvpFileHandler : IFileObject
    {
        private readonly Func<string, IEnumerable<string>> parser;

        public KvpFileHandler(string file, Func<string, IEnumerable<string>> lineparser = null,
            params string[] extensions)
        {
            FileName = file;
            Data = new KeyValuePairList();
            Extensions = extensions;
            parser = lineparser;
        }

        public KeyValuePairList Data { get; private set; }
        public DateTime LastRefresh { get; private set; }
        public string[] Extensions { get; }
        public string FileName { get; }

        public bool Refresh(bool force = false)
        {
            var p = new FileInfo(FileName);

            if (LastRefresh.AddHours(1) > DateTime.Now && !force) return false;

            if (p.Exists)
            {
                var loaded = new KeyValuePairList();

                using (var sr = p.OpenText())
                {
                    while (!sr.EndOfStream)
                    {
                        var tmp_l = loaded;
                        var tmp_d = Data;

                        var parent = string.Empty;

                        foreach (var s in parser(sr.ReadLine()))
                        {
                            if (!tmp_l.Exists(i => i.Key == s)) tmp_l.Add(new KeyValuePairList(s));

                            if (!tmp_d.Exists(i => i.Key == s))
                            {
                                //   yield return parent.Length > 0 ? parent + "_"+ s : s;
                            }

                            if (parent.Length == 0) parent = s;

                            tmp_l = tmp_l.Find(i => i.Key == s);
                            tmp_d = tmp_d.Find(i => i.Key == s) ?? new KeyValuePairList(s);
                        }
                    }
                }


                LastRefresh = DateTime.Now;

                Data.Clear();
                Data = loaded;
            }

            return true;
        }

        public class KeyValuePairList : List<KeyValuePairList>
        {
            internal KeyValuePairList(string key, string value)
                : base(new KeyValuePairList(value))
            {
                Key = key;
            }

            internal KeyValuePairList(string key = null)
            {
                Key = key;
            }

            public string Key { get; }

            public override string ToString()
            {
                return Key ?? base.ToString();
            }
        }
    }
}