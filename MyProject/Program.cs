using System.Reflection;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        static string ObjectToString(object obj)
        {
            StringBuilder strbuilder = new StringBuilder();
            Type type = obj.GetType();
            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                CustomNameAttribute attribute = fieldInfo.GetCustomAttribute<CustomNameAttribute>();
                if (attribute != null)
                {
                    string fieldName = attribute.Name;
                    object fieldValue = fieldInfo.GetValue(obj);
                    strbuilder.AppendFormat("{0}:{1}", fieldName, fieldValue);
                    strbuilder.AppendLine();
                }
            }
            return strbuilder.ToString();
        }

        static void StringToObject(string str, object obj)
        {
            string[] lines = str.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] keyValue = line.Split(':');
                string fieldName = keyValue[0].Trim();
                string fieldValue = keyValue[1].Trim();
                foreach (FieldInfo fieldInfo in obj.GetType().GetFields())
                {
                    CustomNameAttribute attribute = fieldInfo.GetCustomAttribute<CustomNameAttribute>();
                    if (attribute?.Name == fieldName)
                    {
                        Type fieldType = fieldInfo.FieldType;
                        object parsedValue = Convert.ChangeType(fieldValue, fieldType);
                        fieldInfo.SetValue(obj, parsedValue);
                        break;
                    }
                }
            }
        }

        string str = ObjectToString(new MyClass());
        Console.WriteLine(str);
        Console.WriteLine(ObjectToString(new MyClass()));

        MyClass newObject = new MyClass();
        StringToObject(str, newObject);
        StringToObject(ObjectToString(new MyClass()), newObject);
        Console.WriteLine(newObject.I);
    }
}

public class CustomNameAttribute : System.Attribute
{
    public string Name { get; }
    public CustomNameAttribute(string name)
    {
        Name = name;
    }
}


public class MyClass
{
    [CustomName("CustomFieldName")]
    public int I = 0;
}