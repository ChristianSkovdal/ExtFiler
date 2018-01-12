using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtFiler
{
    class Program
    {
        const string Version = "1.0.0";

        static bool quiet = false;
        static bool scss = false;                
        static bool ctrl = false;                
        static bool model = false;               
        static bool overWrite = false;           
        static string tplDir;
        static string tplName;
        static string appName;
        static string prefix;
        static string ns;
        static string outputdir;
                                                                            
        static string fullNS;
        static string outDir;

        static int Main(string[] args)
        {
            int res = ParseArguments(args);

            if (!quiet)
                Console.WriteLine("ExtFiler v. " + Version + "\nBy Christian Skovdal Andersen, 2018.\n" +
                              "A utility for writing ExtJS files based on templates");
            if (res > 0)
                return res;

            try
            {
                //if (string.IsNullOrEmpty(outputdir)) throw new Exception($"Output dir -O not specified");
                if (string.IsNullOrEmpty(tplDir)) throw new Exception($"Template dir -T not specified");
                if (string.IsNullOrEmpty(appName)) throw new Exception($"Application name -A not specified");
                if (string.IsNullOrEmpty(prefix)) throw new Exception($"Class prefix -P not specified");
                //if (string.IsNullOrEmpty(ns)) throw new Exception($"Namespace -N not specified");
                if (string.IsNullOrEmpty(tplName)) throw new Exception($"Template name -F not specified");

                if (string.IsNullOrEmpty(outputdir))
                {
                    outputdir = Directory.GetCurrentDirectory();
                }

                if (!Directory.Exists(outputdir)) throw new Exception($"Output dir {outputdir} does not exist");
                if (!Directory.Exists(tplDir)) throw new Exception($"Template dir {tplDir} does not exist");


                string fn;
                if (!string.IsNullOrEmpty(ns))
                    fullNS = appName + "." + ns;
                else
                    fullNS = appName;


                outDir = Path.Combine(outputdir, ns.Replace(".", "\\"));

                Directory.CreateDirectory(outDir);

                if (scss)
                {
                    fn = Path.Combine(outDir, prefix + ".scss");
                    CreateFile(fn, Path.Combine(tplDir, tplName + ".scss"));
                }
                if (ctrl)
                {
                    fn = Path.Combine(outDir, prefix + "Controller.js");
                    CreateFile(fn, Path.Combine(tplDir, tplName + "Controller.js"));
                }
                if (model)
                {
                    fn = Path.Combine(outDir, prefix + "Model.js");
                    CreateFile(fn, Path.Combine(tplDir, tplName + "Model.js"));
                }

                fn = Path.Combine(outDir, prefix + ".js");
                CreateFile(fn, Path.Combine(tplDir, tplName + ".js"), true);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            if (!quiet)
                Console.WriteLine("Done.");
            return 0;
        }

        private static void CreateFile(string outfile, string tplfile, bool view = false)
        {
            if (File.Exists(outfile) && !overWrite)
            {
                Console.WriteLine($"File {outfile} exists. Ignoring.");
                return;
            }

            if (!File.Exists(tplfile)) throw new Exception($"Template file {tplfile} not found");

            var text = File.ReadAllText(tplfile);
            text = text.Replace("{{NAMESPACE}}", fullNS);
            text = text.Replace("{{NAME}}", prefix);
            text = text.Replace("{{ALIAS}}", prefix.ToLower());

            if (view)
            {
                if (ctrl)
                {
                    text = text.Replace("{{REQUIRE_CONTROLLER}}", $"'{fullNS}.{prefix}Controller',");
                    text = text.Replace("{{ALIAS_CONTROLLER}}", $"controller: '{prefix.ToLower()}',");
                }

                if (model)
                {
                    text = text.Replace("{{REQUIRE_MODEL}}", $"'{fullNS}.{prefix}Model',");
                    text = text.Replace("{{ALIAS_MODEL}}", $"model: '{prefix.ToLower()}',");
                }
                text = text.Replace("{{REQUIRE_CONTROLLER}}", "");
                text = text.Replace("{{ALIAS_CONTROLLER}}", "");
                text = text.Replace("{{REQUIRE_MODEL}}", "");
                text = text.Replace("{{ALIAS_MODEL}}", "");
            }

            File.WriteAllText(outfile, text);
            if (!quiet)
                Console.WriteLine("Creating " + outfile);

        }


        private static int ParseArguments(string[] args)
        {

            quiet = CommandLineUtil.HasFlag(args, "q");
            scss = CommandLineUtil.HasFlag(args, "s");
            ctrl = CommandLineUtil.HasFlag(args, "c");
            model = CommandLineUtil.HasFlag(args, "m");
            overWrite = CommandLineUtil.HasFlag(args, "w");

            if (args.SingleOrDefault(a => a == "-?" || a == "/?") != null)
            {
                ShowError("");
                return 0;
            }

            CommandLineUtil.ParseArg(args, "t", out tplDir);
            CommandLineUtil.ParseArg(args, "f", out tplName);
            CommandLineUtil.ParseArg(args, "a", out appName);
            CommandLineUtil.ParseArg(args, "p", out prefix);
            CommandLineUtil.ParseArg(args, "n", out ns);
            CommandLineUtil.ParseArg(args, "o", out outputdir);


            return 0;
        }

        private static void ShowError(string msg)
        {
            Console.WriteLine(msg + "\r\n\r\nUsage:\r\n" 
                //"-a:<path>\tPath to assembly dll\r\n" +
                //"-o:<path>\tPath to output directory\r\n" +
                //"-d:<data>\tDatabase connection string\r\n" +
                //"-v:<fields>\tA comma separated list of field names of fields that should have validators. If empty all applicable fields get validators\r\n" +
                //"-vx:<fields>\tA comma separated list of field names of fields that should have no validators.\r\n" +
                //"-x:<fields>\tExclude field(s) with the specified names from the model\r\n" +
                //"-q\tQuiet mode. Do not display banner\r\n" +
                //"-t:<e>\tComma separated list of database tables or .Net entitites\r\n" +
                //"-n:<namespace>\tNamespace name\r\n" +
                //"-m:\tInclude table meta-data information on the model in the field 'tableMetaData'\r\n" +
                //"-i:<id>\tName of ID field\r\n\r\n" +
                //"Examples:\r\n" +
                //"SuperModel -a:c:\\myapp\\bin\\myapp.dll -o:c:\\myapp\\js\\model -c:Customer,Company,Employee -n:MyApp\r\n\r\n" +
                //"SuperModel -d:\"Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Foo;Data Source=myserver\\sqlexpress\" -o:c:\\myapp\\js\\model -t:Customer,Company,Employee -n:MyApp"
                );
        }
    }


}
