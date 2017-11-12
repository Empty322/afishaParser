using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using afishaParser;
using System.IO;
using System.Reflection;

namespace test {
	class a { }
	class b : a { }

	[AttributeUsage(AttributeTargets.All)]
	class MyAttribute : Attribute {
		public string Supplement;
		public string Str { get; private set; }
		public MyAttribute(string s) {
			Str = s;
			Supplement = "nope";
		}
	}

	[My("useAttr attr")]
	class UseAttr {

	}

	class Program {
		static void testType() {
			a oa = new a();
			b o = oa as b;
			Type cl = o.GetType();
			Console.WriteLine(cl.Name);
			Type parserType = typeof(Parser);
			Parser obj = null;
			if(parserType.IsClass)
				Console.WriteLine(parserType.Name + " это класс");
			ConstructorInfo[] ci = parserType.GetConstructors();
			foreach(ConstructorInfo c in ci) {
				if(c.GetParameters().Length == 0) {
					obj = c.Invoke(null) as Parser;
					Console.WriteLine(obj.url);
					break;
				}
			}

			MethodInfo[] mi = parserType.GetMethods();
			foreach(MethodInfo m in mi) {
				Console.Write(m.ReturnType.Name + " ");
				Console.Write(m.Name + "(");
				ParameterInfo[] pi = m.GetParameters();
				for(int i = 0; i < pi.Length; i++) {
					Console.Write(pi[i].Name);
					if(i != pi.Length - 1)
						Console.WriteLine(", ");
				}
				Console.WriteLine(")");
			}
		}
		static void testAttr() {
			Type t = typeof(UseAttr);
			Console.Write("Атрибуты в классе " + t.Name + ": ");
			object[] attribs = t.GetCustomAttributes(false);
			foreach(object obj in attribs)
				Console.WriteLine(obj);
			Console.Write("Примечание: ");
			// Извлечь атрибут RemarkAttribute.
			Type tRemAtt = typeof(MyAttribute);
			MyAttribute ra = (MyAttribute)Attribute.GetCustomAttribute(t, tRemAtt);
			Console.WriteLine(ra.Str);
			Console.Write("Дополнение: ");
			Console.WriteLine(ra.Supplement);
		}
		static void Main(string[] args) {

			Console.ReadKey();
		}
	}
}
