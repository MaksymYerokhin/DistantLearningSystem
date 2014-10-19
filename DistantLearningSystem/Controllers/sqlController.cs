using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using DistantLearningSystem.Models;

namespace DistantLearningSystem.Controllers
{
    public class sqlController : Controller
    {
        //
        // GET: /sql/

        [HttpGet]
        public ActionResult Index()
        {
            List<IDictionary<string, object>> v = null;
            return View(v);
        }

        [HttpPost]
        public ActionResult Index(string command)
        {
            try
            {
                string connection_string = @"Data Source=SQL5013.Smarterasp.net;Initial Catalog=DB_9B1265_DLS;User Id=DB_9B1265_DLS_admin;Password=learningsystem;MultipleActiveResultSets=True;App=EntityFramework&quot;";
                SqlConnection connection = new SqlConnection(connection_string);
                connection.Open();
                SqlCommand cmd = new SqlCommand(command, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                List<IDictionary<string, object>> result = new List<IDictionary<string, object>>();
                if (!command.ToLower().Trim().StartsWith("select"))
                {
                    var a = new Dictionary<string, object>();
                    a.Add("success", "successfuly executed!");
                    result.Add(a);
                    return View(result);
                }
                foreach (var item in reader)
                {
                    result.Add(new Dictionary<string, object>());
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        result.Last().Add(reader.GetName(i), reader.GetValue(i));
                    }
                }
                #region
                /*
            command = command.Trim().ToLower();
            Type t;
            CourseDataBaseEntities db = new CourseDataBaseEntities();
            if (command.StartsWith("select"))
            {
                string fieldsString = command.Substring(6, command.IndexOf(" from ") - 6);
                string[] fields = fieldsString.Split(',');
                AssemblyName an = new AssemblyName("MyAssembly");

                AssemblyBuilder ab;
                ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Save);

                ModuleBuilder mb = ab.DefineDynamicModule("MyModule");

                TypeBuilder tb = mb.DefineType("temp");

                foreach (string item in fields)
                {
                    FieldBuilder fb = tb.DefineField(item.Trim(), typeof(int), FieldAttributes.Public);
                }               
                t = tb.CreateType();
                var result = db.Database.SqlQuery(t, command);            
            } */
                #endregion
                return View(result);
            }
            catch 
            {
                List<IDictionary<string, object>> result = new List<IDictionary<string, object>>();
                var a = new Dictionary<string, object>();
                a.Add("fail", "something went wrong!");
                result.Add(a);
                return View(result);
            }            
        }
    }
}
