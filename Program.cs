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
        public static List<MethodInfo> MethodInfoList = new List<MethodInfo>();
        public static List<MethodBody> MethodBodyList = new List<MethodBody>();
        public static string path = @"C:\Users\Ruveyda Sultan Uzun\source\repos\MethodSearcher\MethodSearcher\bin\Debug\net6.0\MethodSearcher.dll";
        public static Assembly SampleAssembly = Assembly.LoadFile(path);


        public static void getMethods()
        {
            int counter = 0;
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
                                Id = counter,
                                ClassName = className,
                                MethodName = method.Name,
                                ReturnType = method.ReturnType.ToString(),
                                NameSpace = namespaceValue,
                                Parameters = "",
                            };
                            MethodInfoList.Add(methodItem);
                            counter++;
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
                            parameters += "," + parameteres.ParameterType.ToString().Substring(parameteres.ParameterType.ToString().IndexOf(".") + 1).ToLower();
                            
                        }
                        try
                        {
                            parameters = parameters.Substring(parameters.IndexOf(",")+1);
                            char.ToUpper(parameters[0]);

                        }
                        catch (Exception e)
                        {

                        }
                        
                        foreach (MethodInfo classitem in MethodInfoList)
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
            var assembly = AssemblyDefinition.ReadAssembly(path);

            foreach (MethodInfo citem in MethodInfoList)
            {
                var toInspect = assembly.MainModule
                .GetTypes()
                .SelectMany(t => t.Methods
                .Where(m => m.HasBody)
                    .Select(m => new { t, m }));
                toInspect = toInspect.Where(x => x.t.Name.EndsWith(citem.ClassName) && x.m.Name == citem.MethodName);

                foreach (var method in toInspect)
                {
                   
                    foreach (var instruction in method.m.Body.Instructions)
                    {
                        takeInstructions(instruction, citem.Id);
                    }
                    // Console.WriteLine($"{instruction.OpCode} \"{instruction.Operand}\"");
                }
            }
        }
        public static void takeInstructions(Instruction instruction, int citem)
        {
            //string[] userParameter = {"String","String","String","String","String"};
            try
            {
                if (instruction.Operand != null && !instruction.Operand.ToString().Contains("System.Func") &&
                    !instruction.Operand.ToString().StartsWith("V") && !instruction.Operand.ToString().Contains("Contains") && !instruction.Operand.ToString().Contains("Substring") && !instruction.Operand.ToString().Contains("IndexOf") && !instruction.Operand.ToString().Contains("Split")
                    && !instruction.Operand.ToString().Contains("<>c") && !instruction.Operand.ToString().Equals("(") && !instruction.Operand.ToString().Equals(",") && !instruction.Operand.ToString().Equals(")") && !instruction.Operand.ToString().Equals(" ") && !instruction.Operand.ToString().Contains("get_Item") && !instruction.Operand.ToString().Contains("get_Length")
                    && !instruction.Operand.ToString().Contains("Enumerator") && !instruction.Operand.ToString().Contains("ctor") && !instruction.Operand.ToString().Contains("get_Operand") && !instruction.Operand.ToString().Contains("StartsWith") && !instruction.Operand.ToString().Contains("ToUpper") &&
                    !instruction.Operand.ToString().Contains("ToLower") && !instruction.Operand.ToString().Contains("Equals")
                    && !instruction.Operand.ToString().Contains("Add") && !instruction.Operand.ToString().Contains("ToString") && !instruction.Operand.ToString().Contains("ToList") && !instruction.Operand.ToString().Contains("Write") && !instruction.Operand.ToString().Contains("Dispose") && !instruction.Operand.ToString().Contains("Enumerable") && 
                    !instruction.Operand.ToString().Contains("IL") && !instruction.Operand.ToString().Contains("Concat"))
                {
                    //{ System.Void Searcher.SearcherMethod.MethodSearcher::GetAllMethod(System.String)}
                    
                    string[] instructionArray;
                    instructionArray = (instruction.Operand).ToString().Split(":");
                    string method = instructionArray[2].Substring(instructionArray[1].IndexOf(":") + 1);
                  
                    string className = "";
                    string methodNamespace = instructionArray[0].Substring(instructionArray[0].IndexOf(" ") +1);
                    string[] namee = methodNamespace.Split(".");
                    if(namee.Length == 3) { methodNamespace = namee[0] + "." + namee[1]; className = namee[2]; } else { methodNamespace = namee[0]; className = namee[1]; }
                    
                   // 
                    string methodType = instructionArray[0].Substring(0, instructionArray[0].IndexOf(" ") + 1);
                    string parameter = "";
                    string[] parameters = method.Split(".");

                    for (int i = 1; i < parameters.Length; i++)
                    {
                        if (i != parameters.Length - 1)
                        {
                            parameter += parameters[i].Split(",")[0].ToLower() + ",";
                        }
                        else
                        {
                            parameter += parameters[i].Substring(0, parameters[i].IndexOf(")")).ToLower();
                        }
                    }
                    MethodBody ci = new MethodBody
                    {
                        NameSpace = methodNamespace,
                        Classname = className,
                        MainMethodID = citem,
                        MethodName = method.Substring(0, method.IndexOf("(")),
                        MethodType = methodType,
                        Parameters = parameter

                    };
                    MethodBodyList.Add(ci);
                }
            }
            catch (Exception e)
            {

            }
        }
        public static void ConnectMysql()
        {
            try
            {
                Mysql.Open();
                if (Mysql.State != System.Data.ConnectionState.Closed)
                {
                    Console.WriteLine("SQL Bağlantısı Başarılı Bir Şekilde Gerçekleşti");
                    cmd.Connection = Mysql;
                    cmd.CommandText = "DELETE FROM methods.method";
                    //cmd.CommandText = "";
                    cmd.ExecuteNonQuery();
                }
                else { Console.WriteLine("Bağlantı Yapılamadı...!"); }
            }
            catch (Exception err) { Console.WriteLine("Hata! " + err.Message); }
        }

        public static void WriteDB()
        {
            ConnectMysql();
            string Query;
            int counter = 0;
            foreach (MethodInfo method in MethodInfoList)
            {
                Query = $"INSERT INTO `methodinfo`(`method_id`,`method_namespace`, `method_class`,`method_name`, `method_parameters`, `method_returntype` ) VALUES ('" + method.Id+ "','" + method.NameSpace + "','" + method.ClassName + "','" + method.MethodName + "','" + method.Parameters + "','" + method.ReturnType + "')";
                                  
                cmd.CommandText =Query;
                cmd.ExecuteNonQuery();
               
               
            }
            foreach (MethodBody methodBody in MethodBodyList)
            {
                Query = $"INSERT INTO `methodbody`(`idmethodbody`,`mainmethodid`,`namespace`, `classname`,`methodname`, `returntype`, `parameters` ) VALUES ('" + counter + "','" + methodBody.MainMethodID + "','" + methodBody.NameSpace + "','" + methodBody.Classname + "','" + methodBody.MethodName + "','" + methodBody.MethodType + "','" + methodBody.Parameters + "')";
                cmd.CommandText = Query;
                cmd.ExecuteNonQuery();
                counter++;


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
            WriteDB();
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
}
public class MethodBody
{
    public string? NameSpace { get; set; }
    public string? Classname { get; set; }
    public int? MainMethodID { get; set; }
    public string? MethodName { get; set; }
    public string? MethodType { get; set; }
    public string? Parameters { get; set; }
}
