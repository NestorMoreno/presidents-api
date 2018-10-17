using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using PresidentsList.API.Models;
using Newtonsoft.Json;

namespace PresidentsList.API
{
    public class GenericStorage
    {
        private string _filePath;

        public GenericStorage()
        {
            var webAppsHome = Environment.GetEnvironmentVariable("HOME");
            if (String.IsNullOrEmpty(webAppsHome))
            {
                _filePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\";
            }
            else
            {
                _filePath = webAppsHome + "\\site\\wwwroot\\";
            }
        }

        public async Task<IEnumerable<President>> Save(IEnumerable<President> target, string filename)
        {
            var json = JsonConvert.SerializeObject(target);
            File.WriteAllText(_filePath + filename, json);
            return target;
        }

        public async Task<IEnumerable<President>> Get(string filename)
        {
            var presidentsText = String.Empty;
            if (File.Exists(_filePath + filename))
            {
                presidentsText = File.ReadAllText(_filePath + filename);
            }

            var presidents = JsonConvert.DeserializeObject<President[]>(presidentsText);
            return presidents;
        }
    }
}
