using System;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MySql.Data.MySqlClient;

namespace deneme
{
    public class Program
    {
        public static bool isInclude = false;

        public static MySqlConnection Mysql = new MySqlConnection("Server=localhost;Database=methods;Uid=root;Pwd='1234';");
        public static MySqlCommand cmd = new MySqlCommand();
        public static List<MethodInfo> classItemList = new List<MethodInfo>();
        public static List<ClassMethodBody> classMethodBody = new List<ClassMethodBody>();


        public static Assembly SampleAssembly = Assembly.LoadFile(@"C:\Users\Ruveyda Sultan Uzun\source\repos\AssemblyDeneme\AssemblyDeneme\bin\Debug\net6.0/AssemblyDeneme.dll");

        public static void ConnectMysql()
        {
            try
            {
                Mysql.Open();
                if (Mysql.State != System.Data.ConnectionState.Closed)
                {
                    Console.WriteLine("SQL Bağlantısı Başarılı Bir Şekilde Gerçekleşti");
                    cmd.Connection = Mysql;
                    //cmd.CommandText = "DELETE FROM methods.method";
                    //cmd.CommandText = "";
                    //cmd.ExecuteNonQuery();
                }
                else { Console.WriteLine("Bağlantı Yapılamadı...!"); }
            }
            catch (Exception err) { Console.WriteLine("Hata! " + err.Message); }
        }
        public static void getMethods()
        {
            var item = SampleAssembly.GetTypes().Where(x => !(x.FullName.StartsWith("System.")) && !(x.FullName.StartsWith("Microsoft."))).ToList();
            string ownClassName = item[item.Count - 1].FullName.Substring(0, item[item.Count - 1].FullName.IndexOf("<") - 1);
            string className;
            string namespaceValue;
            for (int i = item.Count - 2; i > 0; i--)
            {
                if (item[i].Namespace == null || item[i].FullName == ownClassName)
                {
                    className = item[i].Name;
                    namespaceValue= item[i].Namespace;

                    foreach (var method in item[i].GetMethods())
                    {
                        if (!method.Name.Contains("GetHashCode") && !method.Name.Contains("GetType") && !method.Name.Contains("Equals") && !method.Name.Contains("GetType") && !method.Name.Contains("ToString"))
                        {
                            MethodInfo methodItem = new MethodInfo
                            {
                                ClassName = className,
                                MethodName = method.Name,
                                ReturnType = method.ReturnType.ToString(),
                                NameSpace = namespaceValue,
                                Parameters = "",
                            };
                            classItemList.Add(methodItem);
                        }
                    }
                }
            }
        }
        /*
        public static void getProperties()
        {
            var item = SampleAssembly.GetTypes().Where(x => !(x.FullName.StartsWith("System.")) && !(x.FullName.StartsWith("Microsoft."))).ToList();
            string ownClassName = item[item.Count - 1].FullName.Substring(0, item[item.Count - 1].FullName.IndexOf("<") - 1);
            for (int i = item.Count - 2; i > 0; i--) {
           
                if (item[i].Namespace == null || item[i].FullName == ownClassName)
                {
                        foreach (MethodInfo classitem in classItemList)
                        {
                            classitem.ClassName = item[i].Name;
                            classitem.NameSpace = item[i].Namespace;
                        }
                    }
            }
        }
        */
        public static void getParameters()
        {
            var item = SampleAssembly.GetTypes().Where(x => !(x.FullName.StartsWith("System.")) && !(x.FullName.StartsWith("Microsoft."))).ToList();
            string ownClassName = item[item.Count - 1].FullName.Substring(0, item[item.Count - 1].FullName.IndexOf("<") - 1);
            string parameters = "";
            for (int i = item.Count - 2; i > 0; i--)
            {
                if (item[i].Namespace == null || item[i].FullName == ownClassName)
                {
                    foreach (var method in item[i].GetMethods())
                    {
                        foreach (var parameteres in method.GetParameters())
                        {
                            parameters += parameteres.ParameterType.ToString().Substring(parameteres.ParameterType.ToString().IndexOf(".") + 1).ToLower();
                        }
                        
                        foreach (MethodInfo classitem in classItemList)
                        {
                            if (classitem.MethodName.Equals(method.Name))
                            {
                                classitem.Parameters = parameters;
                            }
                        }
                        parameters = "";
                    }
                }
            }
        }

        public static void getMethodBody()
        {
            var assembly = AssemblyDefinition.ReadAssembly(@"C:\Users\Ruveyda Sultan Uzun\source\repos\AssemblyDeneme\AssemblyDeneme\bin\Debug\net6.0/AssemblyDeneme.dll");

            foreach (MethodInfo citem in classItemList)
            {
                var toInspect = assembly.MainModule
            .GetTypes()
                .SelectMany(t => t.Methods
                 .Where(m => m.HasBody)
                    .Select(m => new { t, m }));
                toInspect = toInspect.Where(x => x.t.Name.EndsWith(citem.ClassName) && x.m.Name == citem.MethodName);

                foreach (var method in toInspect)
                {
                    Console.WriteLine($"\tType = {method.t.Name}\n\t\tMethod = {method.m.Name}");
                    foreach (var instruction in method.m.Body.Instructions)
                    {
                        takeInstructions(instruction, citem.MethodName);
                    }
                    // Console.WriteLine($"{instruction.OpCode} \"{instruction.Operand}\"");
                }
            }
        }
        public static void takeInstructions(Instruction instruction, string citem)
        {
          
            try
            {
                if (instruction.Operand != null && !instruction.Operand.ToString().Contains("System.Func") &&
                    !instruction.Operand.ToString().StartsWith("V") && !instruction.Operand.ToString().Contains("Contains") && !instruction.Operand.ToString().Contains("Substring") && !instruction.Operand.ToString().Contains("IndexOf") && !instruction.Operand.ToString().Contains("Split")
                    && !instruction.Operand.ToString().Contains("<>c") && !instruction.Operand.ToString().Equals("(") && !instruction.Operand.ToString().Equals(",") && !instruction.Operand.ToString().Equals(")") && !instruction.Operand.ToString().Equals(" ") && !instruction.Operand.ToString().Contains("get_Item") && !instruction.Operand.ToString().Contains("get_Length")
                    && !instruction.Operand.ToString().Contains("Enumerator") && !instruction.Operand.ToString().Contains("get_Operand") && !instruction.Operand.ToString().Contains("StartsWith") && !instruction.Operand.ToString().Contains("ToUpper") && !instruction.Operand.ToString().Contains("ToLower") && !instruction.Operand.ToString().Contains("Equals") && !instruction.Operand.ToString().Contains("Add") && !instruction.Operand.ToString().Contains("ToString") && !instruction.Operand.ToString().Contains("ToList") && !instruction.Operand.ToString().Contains("Write") && !instruction.Operand.ToString().Contains("Dispose") && !instruction.Operand.ToString().Contains("Enumerable") && !instruction.Operand.ToString().Contains("IL") && !instruction.Operand.ToString().Contains("Concat"))
                {
                    //{ System.Void Searcher.SearcherMethod.MethodSearcher::GetAllMethod(System.String)}

                    string[] instructionArray;
                    instructionArray = (instruction.Operand).ToString().Split(":");
                    string method = instructionArray[2].Substring(instructionArray[1].IndexOf(":") + 1);
                    Console.WriteLine(method);
                    string methodType = instructionArray[0].Substring(0, instructionArray[0].IndexOf(" ") + 1);
                    string parameter = "";
                    string[] parameters = method.Split(".");

                    for (int i = 1; i < parameters.Length; i++)
                    {
                        if (i != parameters.Length - 1)
                        {
                            parameter += parameters[i].Split(",")[0].ToLower();
                        }
                        else
                        {
                            parameter += parameters[i].Substring(0, parameters[i].IndexOf(")")).ToLower();
                        }
                    }
                    ClassMethodBody ci = new ClassMethodBody
                    {
                        MainMethodName = citem,
                        MethodName = method.Substring(0, method.IndexOf("(")),
                        MethodType = methodType,
                        Parameters = parameter

                    };
                    classMethodBody.Add(ci);
                }
            }
            catch (Exception e)
            {

            }
        }

        public static void findCurrentMethod()
        {
            Console.WriteLine("bitti");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Method İsimleri");
            getMethods();
            Console.WriteLine("Method namespace and class");
          //  getProperties();
            Console.WriteLine("Method parameters");
             getParameters();
            Console.WriteLine("Method body");
             getMethodBody();
            findCurrentMethod();
            //ConnectMysql();
        }
    }

}
public class MethodInfo
{
    public MethodInfo() { }
    public int Id { get; set; }
    public int SubId { get; set; }
    public string? NameSpace { get; set; }
    public string? ClassName { get; set; }
    public string? MethodName { get; set; }
    public string? Parameters { get; set; }
    public string? ReturnType { get; set; }

    public static MySqlConnection Mysql = new MySqlConnection("Server=localhost;Database=methods;Uid=root;Pwd='1234';");
    public static MySqlCommand cmd = new MySqlCommand();

    public static void ConnectMysql()
    {
        try
        {

            Mysql.Open();
            if (Mysql.State != System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("SQL Bağlantısı Başarılı Bir Şekilde Gerçekleşti");
                cmd.Connection = Mysql;
                //cmd.CommandText = "DELETE FROM methods.method";
                //cmd.CommandText = "";
                //cmd.ExecuteNonQuery();
            }
            else { Console.WriteLine("Bağlantı Yapılamadı...!"); }
        }
        catch (Exception err) { Console.WriteLine("Hata! " + err.Message); }
    }
}
public class ClassMethodBody
{
    public string? MainMethodName { get; set; }
    public string? MethodName { get; set; }
    public string? MethodType { get; set; }
    public string? Parameters { get; set; }

}
