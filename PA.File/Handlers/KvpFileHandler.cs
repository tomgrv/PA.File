using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.File.Handlers
{
    public class KvpFileHandler : IFileObject
    {
        public string FileName { get; private set; }
        public KeyValuePairList Data { get; private set; }
        public DateTime LastRefresh { get; private set; }
        public string[] Extensions { get; private set; }

        public class KeyValuePairList : List<KeyValuePairList>
        {
            public string Key { get; private set; }

            internal KeyValuePairList(string key, string value)
                : base(new KeyValuePairList(value))
            {
                this.Key = key;
            }

            internal KeyValuePairList(string key = null)
                : base()
            {
                this.Key = key;
            }

            public override string ToString()
            {
                return this.Key ?? base.ToString();
            }
        }

        private Func<string, IEnumerable<string>> parser;

        public KvpFileHandler(string file, Func<string, IEnumerable<string>> lineparser = null, params string[] extensions)
        {

            this.FileName = file;
            this.Data = new KeyValuePairList();
            this.Extensions = extensions;
            this.parser = lineparser;
        }

        public bool Refresh(bool force = false)
        {

            var p = new FileInfo(this.FileName);

            if (this.LastRefresh.AddHours(1) > DateTime.Now && !force)
            {
                return false;
            }

            if (p.Exists)
            {
                KeyValuePairList loaded = new KeyValuePairList();

                using (StreamReader sr = p.OpenText())
                {
                    while (!sr.EndOfStream)
                    {
                        KeyValuePairList tmp_l = loaded;
                        KeyValuePairList tmp_d = this.Data;

                        string parent = string.Empty;

                        foreach (string s in this.parser(sr.ReadLine()))
                        {  
                            if (!tmp_l.Exists(i => i.Key == s))
                            {
                                tmp_l.Add(new KeyValuePairList(s));
                            }

                            if (!tmp_d.Exists(i => i.Key == s))
                            {
                             //   yield return parent.Length > 0 ? parent + "_"+ s : s;
                            }

                            if (parent.Length == 0)
                            {
                                parent = s;
                            }

                            tmp_l = tmp_l.Find(i => i.Key == s);
                            tmp_d = tmp_d.Find(i => i.Key == s) ?? new KeyValuePairList(s);
                        }
                    }
                }


                this.LastRefresh = DateTime.Now;

                this.Data.Clear();
                this.Data = loaded;

               
            }

            return true;
        }
    }
}

