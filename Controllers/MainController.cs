using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Administration;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IISSiteManagement.Controllers
{
    public class MainController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _path = "C:\\HostingSpaces\\";
        private readonly string _token = "sdfoihlsdfhtelr87ouygke";

        public MainController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return Content("Index");
        }

        [HttpGet]
        public IActionResult AddSite(string name, string token)
        {
            if(token != _token)
            {
                return Content("Wrong token");
            }
            name = ClearName(name);
            Log("AddSite: " + name);
            try
            {
                string path = $"{_path}{name}";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                using (System.IO.StreamWriter writer = System.IO.File.AppendText(path + "\\i.html"))
                {
                    writer.WriteLine($"{name} - is working");
                }


                ServerManager iisManager = new ServerManager();
                Site site = iisManager.Sites.FirstOrDefault(r => r.Name == name);
                if (site == null)
                {
                    Site newSite = iisManager.Sites.Add(name, "http", "*:80:" + name, path);
                    newSite.ServerAutoStart = true;
                    iisManager.CommitChanges();
                }

                ApplicationPool pool = iisManager.ApplicationPools.FirstOrDefault(r => r.Name == name);
                if (pool == null)
                {
                    iisManager.ApplicationPools.Add(name);
                    iisManager.Sites[name].Applications[0].ApplicationPoolName = name;
                    ApplicationPool apppool = iisManager.ApplicationPools[name];
                    apppool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                    apppool.ProcessModel.IdentityType = ProcessModelIdentityType.LocalSystem;
                    iisManager.CommitChanges();
                }
            }
            catch (Exception e)
            {
                Log($"{e.Message} - {e.InnerException}");
                return Content(e.Message);
            }
            Log("Ok");
            return Content("Ok");
        }

        [HttpGet]
        public IActionResult RemoveSite(string name)
        {
            name = ClearName(name);
            Log("AddSite: " + name);
            try
            {
                ServerManager iisManager = new ServerManager();

                Site site = iisManager.Sites.FirstOrDefault(r => r.Name == name);
                if (site != null)
                {
                    iisManager.Sites.Remove(site);
                    iisManager.CommitChanges();
                }
                else
                {
                    return Content("Not found");
                }

                ApplicationPool pool = iisManager.ApplicationPools.FirstOrDefault(r => r.Name == name);
                if (pool != null)
                {
                    iisManager.ApplicationPools.Remove(pool);
                    iisManager.CommitChanges();
                }

                System.Threading.Thread.Sleep(3000);

                string path = $"{_path}{name}";
                System.IO.DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                if (System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.Delete(path);
                }
            }
            catch (Exception e)
            {
                Log($"{e.Message} - {e.InnerException}");
                return Content(e.Message);
            }
            Log("Ok");
            return Content("Ok");
        }

        private void Log(string text)
        {
            string projectRootPath = _hostingEnvironment.ContentRootPath;
            using (System.IO.StreamWriter writer = System.IO.File.AppendText($"{projectRootPath}\\log.txt"))
            {
                writer.WriteLine($"{DateTime.Now.ToString()} - {text}");
            }
        }

        private string ClearName(string Name)
        {
            Regex rgx = new Regex("[^a-z0-9-.]");
            return rgx.Replace(Name, "-");
        }
    }
}
