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

        public static MySqlConnection Mysql = new MySqlConnection("Server=localhost;Database=methods;Uid=root;Pwd='1234';");
        public static MySqlCommand cmd = new MySqlCommand();
        public static MySqlDataReader reader;
        public static List<MethodInfo> MethodInfoList = new List<MethodInfo>();
        public static List<MethodBody> MethodBodyList = new List<MethodBody>();
        public static string path = @"C:\Users\Ruveyda Sultan Uzun\source\repos\denemeMethods\denemeMethods\bin\Debug\net6.0\denemeMethods.dll";
        public static Assembly SampleAssembly = Assembly.LoadFile(path);
        public static int counter = 1;
        public static Dictionary<string, List<MethodInfo>> searcherResultList = new Dictionary<string, List<MethodInfo>>();
        public static void getMethods()
        {
            int counter = 0;
            var types = SampleAssembly.GetTypes().Where(x => !(x.FullName.StartsWith("System.")) && !(x.FullName.StartsWith("Microsoft."))).ToList();
            string className;
            string namespaceValue;
            for (int i = types.Count - 1; i >= 0; i--)
            {
                    className = types[i].Name;
                    namespaceValue = types[i].Namespace;

                    foreach (var method in types[i].GetMethods())
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
        public static void getParameters()
        {
            var types = SampleAssembly.GetTypes().Where(x => !(x.FullName.StartsWith("System.")) && !(x.FullName.StartsWith("Microsoft."))).ToList();
            string parameters = "";

            for (int i = types.Count - 1; i >= 0; i--)
            {
                    foreach (var method in types[i].GetMethods())
                    {
                        foreach (var parameteres in method.GetParameters())
                        {
                            parameters += "," + parameteres.ParameterType.ToString().Substring(parameteres.ParameterType.ToString().IndexOf(".") + 1).ToLower();
                        }
                        try
                        {
                            parameters = parameters.Substring(parameters.IndexOf(",") + 1);
                            char.ToUpper(parameters[0]);
                        }
                        catch (Exception e) { }

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
                }
            }
        }
        public static void takeInstructions(Instruction instruction, int citem)
        {
            string[] instructionArray;
            string methodInfo;
            string className = "";
            string methodNamespace;
            string returnType;
            string parameter = "";
            string[] methodTitle;
            string[] parameters;

            try
            {
                if (instruction.Operand != null && !instruction.Operand.ToString().Contains("System.Func") &&
                    !instruction.Operand.ToString().StartsWith("V") && !instruction.Operand.ToString().Contains("Contains") && !instruction.Operand.ToString().Contains("Substring") && !instruction.Operand.ToString().Contains("IndexOf") && !instruction.Operand.ToString().Contains("Split")
                    && !instruction.Operand.ToString().Contains("<>c") && !instruction.Operand.ToString().Equals("(") && !instruction.Operand.ToString().Equals(",") && !instruction.Operand.ToString().Equals(")") && !instruction.Operand.ToString().Equals(" ") && !instruction.Operand.ToString().Contains("get_Item") && !instruction.Operand.ToString().Contains("get_Length")
                    && !instruction.Operand.ToString().Contains("Enumerator") && !instruction.Operand.ToString().Contains("ctor") && !instruction.Operand.ToString().Contains("get_Operand") && !instruction.Operand.ToString().Contains("StartsWith") && !instruction.Operand.ToString().Contains("ToUpper") &&
                    !instruction.Operand.ToString().Contains("ToLower") && !instruction.Operand.ToString().Contains("Equals")
                    && !instruction.Operand.ToString().Contains("Add") && !instruction.Operand.ToString().Contains("ToString") && !instruction.Operand.ToString().Contains("ToList") && !instruction.Operand.ToString().Contains("Write") && !instruction.Operand.ToString().Contains("Dispose") && !instruction.Operand.ToString().Contains("Enumerable") &&
                    !instruction.Operand.ToString().Contains("IL") && !instruction.Operand.ToString().Contains("Concat")){

                    instructionArray = (instruction.Operand).ToString().Split(":");
                    methodInfo = instructionArray[2].Substring(instructionArray[1].IndexOf(":") + 1);
                    methodNamespace = instructionArray[0].Substring(instructionArray[0].IndexOf(" ") + 1);
                    methodTitle = methodNamespace.Split(".");

                    if (methodTitle.Length == 3) { methodNamespace = methodTitle[0] + "." + methodTitle[1]; className = methodTitle[2]; } else { methodNamespace = methodTitle[0]; className = methodTitle[1]; }

                    returnType = instructionArray[0].Substring(0, instructionArray[0].IndexOf(" ") + 1);
                    parameters = methodInfo.Split(".");

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
                        MethodName = methodInfo.Substring(0, methodInfo.IndexOf("(")),
                        MethodType = returnType,
                        Parameters = parameter
                    };
                    MethodBodyList.Add(ci);
                }
            }
            catch (Exception e) { }
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
                    /*cmd.CommandText = "DELETE FROM methods.methodinfo";
                    cmd.CommandText = "";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DELETE FROM methods.methodbody";
                    cmd.ExecuteNonQuery();*/
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
                Query = $"INSERT INTO `methodinfo`(`method_id`,`method_namespace`, `method_class`,`method_name`, `method_parameters`, `method_returntype` ) VALUES ('" + method.Id + "','" + method.NameSpace + "','" + method.ClassName + "','" + method.MethodName + "','" + method.Parameters + "','" + method.ReturnType + "')";
                cmd.CommandText = Query;
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
        public static void SearchUserMethod(string Namespace, string className, string methodName, string parameters)
        {
            
            List<MethodInfo> methodList = new List<MethodInfo>();
            string Query = "Select m.method_namespace,m.method_class,m.method_name,m.method_parameters From methods.methodinfo m,  methods.methodbody b where m.method_id = b.mainmethodid and b.methodname = '"+ methodName+"' and b.classname = '" + className + "' and b.namespace = '" + Namespace + "' and b.parameters = '" + parameters + "' ";
            
            Console.WriteLine(counter);

            cmd.CommandText = Query;
            reader  = cmd.ExecuteReader();

            while (reader.Read())
            {
                MethodInfo method = new MethodInfo
                {
                    NameSpace = reader["method_namespace"].ToString(),
                    ClassName = reader["method_class"].ToString(),
                    MethodName = reader["method_name"].ToString(),
                    Parameters = reader["method_parameters"].ToString(),
                };
                methodList.Add(method);
            }
            reader.Close();
            counter++;
            searcherResultList.Add(methodName, methodList);

            foreach (MethodInfo m in methodList)
            {
                Console.WriteLine(m.MethodName);
                SearchUserMethod(m.NameSpace, m.ClassName, m.MethodName, m.Parameters);
            }
        }
        public static void getAllMethods()
        {
        
            getMethods();
            getParameters();
            getMethodBody();
            WriteDB();
        }
           

       static void Main(string[] args)
       {
                // method Info : 
        string nameSpace = "class3Namespace";
        string className = "Class3";
        string methodName = "class3Function2";
        string returnType = "System.Void";
        string parameters = "string,ınt32";

        getAllMethods();
        ConnectMysql();
        SearchUserMethod(nameSpace, className, methodName, parameters);
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
