using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Triggerless.XAFLib
{
    public class ChknFile
    {
        public static void CreateChkn(string folder, Template template, IEnumerable<string> filenames = null)
        {
            var filename = Path.Combine(folder, "index.xml");
            if (File.Exists(filename)) File.Delete(filename);
            File.WriteAllText(filename, template.GetIndexXml());

            var jsonFile = Path.Combine(folder, "imvu-internal.json");
            if (File.Exists(jsonFile)) File.Delete(jsonFile);
            File.WriteAllText(jsonFile, "{}");

            var chkn = Path.Combine(Directory.GetParent(folder).FullName, $"{Path.GetFileName(folder)}.chkn");
            if (File.Exists(chkn)) File.Delete(chkn);
            var fz = new FastZip();

            if (filenames == null) {
                fz.CreateZip(chkn, folder, false, null);
                return;
            }
            var ff = new FilenameFilter(filenames);
            fz.CreateZip(chkn, folder, false, ff, null);            
        }
    }

    public class FilenameFilter : IScanFilter
    {
        private IEnumerable<string> _filenames;

        public FilenameFilter(IEnumerable<string> filenames)
        {
            _filenames = filenames.Select(f => f.ToLower());
        }
        public bool IsMatch(string name)
        {
            return _filenames.Contains(name.ToLower());
        }
    }
}
