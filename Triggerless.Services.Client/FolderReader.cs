using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Models;

namespace Triggerless.Services.Client
{
    public class FolderReader
    {

        private string _directory;
        private string[] _filenames;

        private string DefaultDir => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\IMVU";
        private string[] DefaultFiles => new [] { "IMVULog.log.5", "IMVULog.log.4", "IMVULog.log.3", "IMVULog.log.2", "IMVULog.log.1", "IMVULog.log", };

    public FolderReader(string directory, string[] filenames)
        {
            _directory = directory;
            _filenames = filenames;
        }

        public FolderReader(string directory)
        {
            _directory = directory;
            _filenames = DefaultFiles;
        }

        public FolderReader()
        {
            _directory = DefaultDir;
            _filenames = DefaultFiles;
        }

        public LogConversation Read()
        {
            var folderConvo = new LogConversation();
            var errors = new List<string>();

            foreach (var filename in _filenames)
            {
                var path = Path.Combine(_directory, filename);
                try
                {
                    var logReader = new FileReader();
                    var fileConvo = logReader.ReadFile(path);
                    folderConvo.Events.AddRange(fileConvo.Events);
                } catch (Exception exc)
                {
                    errors.Add($"{filename}: {exc.Message}");
                }
            }
            folderConvo.Errors = errors;

            var names = new Dictionary<long, ImvuUser>();

            foreach (var @event in folderConvo.Events)
            {
                if (!names.ContainsKey(@event.Author.Id))
                {
                    var store = new Store();
                    var user = store.GetUser(@event.Author.Id).Result;

                    if (user == null)
                    {
                        throw new ArgumentException($"No user found with Id {@event.Author.Id}");
                    }
                    

                    names[@event.Author.Id] = user;

                }
                @event.Author = names[@event.Author.Id];
            }

            return folderConvo;
        }

    }
}
