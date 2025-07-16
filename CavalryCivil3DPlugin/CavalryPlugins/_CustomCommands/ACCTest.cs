using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using CavalryCivil3DPlugin._Library._ACC;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins._CustomCommands
{
    public class ACCTest
    {
        [CommandMethod("ACCTest")]
        public async Task _ACCTest()
        {
            string clientId = "lGFFOxhKPtGneOmAsfnX6smn6iLAEUoUj8cVtVHVjIiADIGA";
            string clientSecret = "cQnZNwaZ29FEpYtqMgasiNwHxYTomZ0z3m103zyGGRbP1N90Faaox5nozNM8HgGe";
            string accountId = "4fbfcc3d-d3ab-48d6-a25c-82f0e7d9b84b";

            ACCManage accManage = new ACCManage(clientId, clientSecret, accountId, "WOF Demo");

            await accManage.Initialize();
            await accManage.GetProjects();
            var projects = accManage.Projects;
            accManage.Console.Print(projects);
            accManage.Console.Print($"Total Projects found: {projects.Count}");
            await accManage.GetForms();
        }
    }
}
