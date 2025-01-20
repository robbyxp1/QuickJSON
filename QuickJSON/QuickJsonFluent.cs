
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickJSON.Utils;
using System.IO;
namespace QuickJSON
{
    public partial class JSONFormatter
    {
        public class FormatterException : Exception
        {
            public FormatterException(string error)
            {
                Error = error;
            }
            public string Error { get; set; }
        }
        public JSONFormatter()
        {
            json = new StringBuilder(10000);
            stack = new List<StackEntry>();
            precomma = false;
        }
        public JSONFormatter V(string name, string data)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append('"');
            json.Append(data);
            json.Append('"');
            return this;
        }
        public JSONFormatter V(string data)
        {
            return V(null, data);
        }
        public JSONFormatter V(string name, float value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append(value.ToString("R"));
            return this;
        }
        public JSONFormatter V(float value)
        {
            return V(null, value);
        }
        public JSONFormatter V(string name, double value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append(value.ToString("R"));
            return this;
        }
        public JSONFormatter V(double value)
        {
            return V(null, value);
        }
        public JSONFormatter V(string name, int value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append(value.ToStringInvariant());
            return this;
        }
        public JSONFormatter V(int value)
        {
            return V(null, value);
        }
        public JSONFormatter V(string name, long value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append(value.ToStringInvariant());
            return this;
        }
        public JSONFormatter V(long value)
        {
            return V(null, value);
        }
        public JSONFormatter V(string name, bool value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append(value ? "true" : "false");
            return this;
        }
        public JSONFormatter V(bool value)
        {
            return V(null, value);
        }
        public JSONFormatter V(string name, DateTime value)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":");
            }
            json.Append('"');
            json.Append(value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            json.Append('"'); 
            return this;
        }
        public JSONFormatter V(DateTime value)
        {
            return V(null, value);
        }
        public JSONFormatter Array(string name = null)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":[");
            }
            else
                json.Append("[");
            stack.Add(new StackEntry(StackType.Array, precomma));
            precomma = false;
            return this;
        }
        public JSONFormatter Object(string name = null)
        {
            Prefix(name != null);
            if (name != null)
            {
                json.Append('"');
                json.Append(name);
                json.Append("\":{");
            }
            else
                json.Append("{");
            stack.Add(new StackEntry(StackType.Object, precomma));
            precomma = false;
            return this;
        }
        public JSONFormatter Close(int depth = 1)
        {
            while (depth-- > 0 && stack.Count > 0)
            {
                StackEntry e = stack.Last();
                if (lf)
                {
                    json.Append(Environment.NewLine);
                    lf = false;
                }
                if (e.stacktype == StackType.Array)
                    json.Append(']');
                else
                    json.Append('}');
                precomma = e.precomma;
                stack.RemoveAt(stack.Count - 1);
            }
            return this;
        }
        public JSONFormatter LF()
        {
            lf = true;
            return this;
        }
        public string Get()
        {
            Close(int.MaxValue);
            return CurrentText;
        }
        public void Clear()
        {
            json.Clear();
        }
        public string CurrentText { get { return json.ToString(); } }
        public int Length { get { return json.Length; } }
        private enum StackType { Array, Object };
        private class StackEntry
        {
            public bool precomma;
            public StackType stacktype;
            public StackEntry(StackType a, bool b)
            { precomma = b; stacktype = a; }
        }
        private StringBuilder json;
        private List<StackEntry> stack;
        private bool precomma;          // starts false, every value sets it true.
        private bool lf;                // want a lf next
        protected virtual void Prefix(bool named)
        {
            if (named)
            {
                if (stack.Count == 0)
                    throw new FormatterException("Can't start JSON with a property name");
                else if (stack.Last().stacktype == StackType.Array)
                    throw new FormatterException("Property names not allowed in arrays");
            }
            else
            {
                if (stack.Count > 0 && stack.Last().stacktype == StackType.Object)
                {
                    throw new FormatterException("Property name is required in an object");
                }
            }
            if (precomma)
                json.Append(',');
            if (lf)
            {
                json.Append(Environment.NewLine);
                lf = false;
            }
            precomma = true;
        }
    }
}
namespace QuickJSON
{
    public class JSONFormatterStreamer : JSONFormatter, IDisposable
    {
        public JSONFormatterStreamer(Stream stream, int writeblock = 1000, System.Text.Encoding encoding = null ) : base()
        {
            enc = encoding != null ? encoding : new System.Text.UTF8Encoding();
            this.stream = stream;
            this.writeblock = writeblock;
        }
        protected override void Prefix(bool named)
        {
            if ( Length >= writeblock )
            {
                var bytes = enc.GetBytes(CurrentText);
                stream.Write(bytes, 0, bytes.Length);
                Clear();
            }
            base.Prefix(named);
        }
        public void Dispose()
        {
            Close();
            if ( Length >= 0 )
            {
                var bytes = enc.GetBytes(CurrentText);
                stream.Write(bytes, 0, bytes.Length);
                Clear();
            }
        }
        private System.Text.Encoding enc;
        private Stream stream;
        private int writeblock;
    }
}
namespace QuickJSON
{
    public partial class JSONFormatter
    {
        public static string ToFluent(JToken tk, bool lf = false)
        {
            StringBuilder sb = new StringBuilder();
            ToFluent(tk, sb, lf);
            return sb.ToString();
        }
        public static void ToFluent(JToken tk, StringBuilder code, bool lf = false, string propertyname = null)
        {
            if (tk.IsObject)
            {
                if (lf)
                    NewLine(code);
                if (propertyname!= null)
                {
                    code.Append(".Object(");
                    code.Append(propertyname.AlwaysQuoteString());
                    code.Append(')');
                }
                else
                    code.Append(".Object()");
                foreach (var kvp in tk.Object())
                {
                    ToFluent(kvp.Value, code, lf, kvp.Key);
                }
                code.Append(".Close()");
                if (lf)
                    NewLine(code);
            }
            else if (tk.IsArray)
            {
                if (lf)
                    NewLine(code);
                if (propertyname != null)
                {
                    code.Append(".Array(");
                    code.Append(propertyname.AlwaysQuoteString());
                    code.Append(')');
                }
                else
                    code.Append(".Array()");
                foreach (var v in tk.Array())
                {
                    ToFluent(v, code, lf);
                }
                code.Append(".Close()");
                if (lf)
                    NewLine(code);
            }
            else
            {
                string vstring = tk.ToString();
                if (propertyname!=null)
                {
                    code.Append(".V(");
                    code.Append(propertyname.AlwaysQuoteString());
                    code.Append(',');
                    code.Append(vstring);
                    code.Append(')');
                }
                else
                {
                    code.Append(".V(");
                    code.Append(vstring);
                    code.Append(')');
                }
            }
        }
        private static void NewLine(StringBuilder code)
        {
            if (code.Length > 0 && code[code.Length - 1] != '\n')
                code.Append(Environment.NewLine);
        }
    }
}

