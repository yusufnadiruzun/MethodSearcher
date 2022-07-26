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

        MySqlConnection Mysql = new MySqlConnection("Server=localhost;Database=methods;Uid=root;Pwd='1234';");
        MySqlCommand cmd = new MySqlCommand();
        ClassItem[] classItemList;  
        public void ConnectMysql()
        {
            try
            {
                Mysql.Open();
                if (Mysql.State != System.Data.ConnectionState.Closed)
                {
                    Console.WriteLine("SQL Bağlantısı Başarılı Bir Şekilde Gerçekleşti");
                    cmd.Connection = Mysql;
                    //cmd.CommandText = "";
                    //cmd.ExecuteNonQuery();
                }
                else { Console.WriteLine("Bağlantı Yapılamadı...!"); }
            }
            catch (Exception err) { Console.WriteLine("Hata! " + err.Message); }
        }


        public static void Write()
        {
            Assembly SampleAssembly = Assembly.LoadFile(@"C:\Users\Ruveyda Sultan Uzun\source\repos\MethodSearcher\MethodSearcher\bin\Debug\net6.0/MethodSearcher.dll");
            Type[] types = SampleAssembly.GetTypes();
            List<MemberInfo[]> methods = new List<MemberInfo[]>();

            foreach (var type in types)
            {
                MemberInfo[] ass = type.GetMethods() as MemberInfo[];
                foreach (MemberInfo member in ass)
                {
                    Console.WriteLine(type.Name + "." + member.Name + " type " + type);
                }
                methods.Add(ass);
            }
        }

       
       
        public static void assemblyProperties()
        {
            Assembly SampleAssembly = Assembly.LoadFile(@"C:\Users\Ruveyda Sultan Uzun\source\repos\MethodSearcher\MethodSearcher\bin\Debug\net6.0/MethodSearcher.dll");
            foreach (var item in SampleAssembly.GetTypes())
            {
                //program içindeki propertyinin tiplerini alıyoruz. class içindeki değişken tipleri. content, namespace, classname, methodname
                foreach (var property in item.GetProperties())
                {
                    Console.WriteLine("property type : " +property.PropertyType + "property name : " + property.Name);
                }
            }
        }
        public static void getMethodBody()
        {

            var assembly = AssemblyDefinition.ReadAssembly(@"C:\Users\Ruveyda Sultan Uzun\source\repos\MethodSearcher\MethodSearcher\bin\Debug\net6.0/MethodSearcher.dll");
            var toInspect = assembly.MainModule
            .GetTypes()
                .SelectMany(t => t.Methods
                 .Where(m => m.HasBody)
                    .Select(m => new { t, m }));

            toInspect = toInspect.Where(x => x.t.Name.EndsWith("MethodSearcher") && x.m.Name == "GetAllMethod");
          
                foreach (var method in toInspect)
                {
                    Console.WriteLine($"\tType = {method.t.Name}\n\t\tMethod = {method.m.Name}");
                    foreach (var instruction in method.m.Body.Instructions)
                    // Console.WriteLine($"{instruction.OpCode} \"{instruction.Operand}\"");
                    if (!isInclude)
                    {
                        takeInstructions(instruction);
        } else

            {
                Console.WriteLine("işlem bitti" + isInclude);
                        break;
            }
            }
           
        }
           
           
        public static void takeInstructions(Instruction instruction)
        {
            string[] userParameter = {"String", "String", "int", "String","String"};
            try
            {
                if (instruction.Operand != null)
                {
                    string[] instructionArray;
                    instructionArray = (instruction.Operand).ToString().Split(":");
                    string method = instructionArray[2].Substring(instructionArray[1].IndexOf(":") + 1);
                    findCurrentMethod(method, userParameter);
                }
            }
            catch (Exception e){}   
        }
        public static void findCurrentMethod(string name, string[] userParameter)
        {
            string parameter;
            string [] parameters = name.Split(".");
            int j = 1;
            for(int i = 0; i<parameters.Length; i++)
            {
                try
                {
                    if (i != userParameter.Length - 1) { parameter = parameters[j].Split(",")[0]; }
                    else{parameter = parameters[j].Substring(0, parameters[j].IndexOf(")"));}
                    /*
                           if (userParameter[i] == parameter)
                           {
                              isInclude = true; 
                           }
                           else
                           {
                               isInclude = false;
                               break;
                           }
                           j++;
                       }
                    */
                }
                catch (Exception e){ Console.WriteLine("catch error");}
            }      } 
        public static void assemblyParameter()
        {
            Assembly SampleAssembly = Assembly.LoadFile(@"C:\Users\Ruveyda Sultan Uzun\source\repos\MethodSearcher\MethodSearcher\bin\Debug\net6.0/MethodSearcher.dll");
            Console.WriteLine(SampleAssembly.DefinedTypes);
            //    var method = Type.GetType(SampleAssembly.);
            //    type name = sınıfın ismi
            //    Member info ile methodları al
            // member.name = method ismi
            Type[] types = SampleAssembly.GetTypes();
            List<MemberInfo[]> methods = new List<MemberInfo[]>();
            foreach (var type in types)
            {
                //  MemberInfo[] ass = type.GetMethods() as MemberInfo[];
                MethodInfo[] ass = type.GetMethods();
                foreach (MethodInfo member in ass)
                {
                    // return parameter ile fonksiyorunun hangi değeri döndürdüğü bulunur. Void, string, int ...
                    // Console.WriteLine(member.ReturnParameter + "  : : " + member.Name);
                    foreach(var item in member.GetParameters())
                    {
                        Console.WriteLine(item.ParameterType + " :: " + item.Name  );
                    }
                }
            }
        }
        public static void assemblyFunction()
        {
            Assembly SampleAssembly = Assembly.LoadFile(@"C:\Users\Ruveyda Sultan Uzun\source\repos\MethodSearcher\MethodSearcher\bin\Debug\net6.0/MethodSearcher.dll");
            Console.WriteLine(SampleAssembly.DefinedTypes);
            //    var method = Type.GetType(SampleAssembly.);
            //    type name = sınıfın ismi 
            //    Member info ile methodları al 
            // member.name = method ismi
            Type[] types = SampleAssembly.GetTypes();
           

            foreach (var type in types)
            {
                var methods = type.GetProperties(BindingFlags.InvokeMethod);
                foreach (var method in methods)
                {
                    Console.WriteLine(method.Name);
                }
            }
        }
        public static void assemblyMethodBody()
        {
            Assembly SampleAssembly = Assembly.LoadFile(@"C:\Users\Ruveyda Sultan Uzun\source\repos\MethodSearcher\MethodSearcher\bin\Debug\net6.0/MethodSearcher.dll");
            Console.WriteLine(SampleAssembly.DefinedTypes);
            
            //    var method = Type.GetType(SampleAssembly.);
            //    type name = sınıfın ismi 
            //    Member info ile methodları al 
            // member.name = method ismi
            Type[] types = SampleAssembly.GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetProperties(BindingFlags.InvokeMethod);
                foreach (var method in methods)
                {
                    Console.WriteLine(method);
                }
            }
        }
        static void Main(string[] args)
        {
           getMethodBody();
          //  assemblyProperties();
        }
    }
    
}
    public class ClassItem
    {
        public ClassItem() { }
        public string? MethodName { get; set; }
        public string? NameSpace { get; set; }
        public string[]? Parameters { get; set; }
        public string? Content { get; set; }
        public string? ClassName { get; set; }
    }