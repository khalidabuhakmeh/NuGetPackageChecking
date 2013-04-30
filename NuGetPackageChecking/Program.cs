using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace NuGetPackageChecking
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = new NugetUpdateRequest()
                .AddPackage("Newtonsoft.Json", "4.0.1")
                .AddPackage("NUnit", "2.6.1")
                .Execute();

            Console.WriteLine(task.Result);
            Console.ReadLine();
        }
    }

    public class NugetUpdateRequest
    {
        // probably allow for different feed urls
        const string Url =
            "http://nuget.org/api/v2/GetUpdates()?" +
            "packageIds='{0}'" +
            "&versions='{1}'" +
            "&includePrerelease={2}" +
            "&includeAllVersions={3}" +
            "&targetFrameworks='{4}'";

        public NugetUpdateRequest()
        {
            PackageIds = new List<string>();
            Versions = new List<string>();
            TargetFrameworks = new List<string> { "net45" };
        }

        public bool IncludePrerelease { get; set; }
        public bool IncludeAllVersions { get; set; }

        protected IList<string> PackageIds { get; set; }
        protected IList<string> Versions { get; set; }
        protected IList<string> TargetFrameworks { get; set; }

        public NugetUpdateRequest AddPackage(string id, string version)
        {
            PackageIds.Add(id);
            Versions.Add(version);

            return this;
        }

        public NugetUpdateRequest AddTargetFramework(string framework)
        {
            TargetFrameworks.Add(framework);
            return this;
        }

        public async Task<string> Execute()
        {
            var client = new HttpClient();
            var resource = new Uri(string.Format(Url,
                string.Join("|", PackageIds.Distinct()),
                string.Join("|", Versions.Distinct()),
                IncludePrerelease.ToString().ToLower(),
                IncludeAllVersions.ToString().ToLower(),
                string.Join("|", TargetFrameworks)));

            var response = await client.GetAsync(resource);
            var result = await response.Content.ReadAsStringAsync();

            // do some querying of xml document
            //var doc = new XmlDocument();
            //doc.LoadXml(result);

            return result;
        }
    }
}
