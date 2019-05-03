using System.Collections.Generic;

namespace DbReader.Construction
{

    public interface IObjectConverter
    {
        Dictionary<string, object> Convert(object obj);
    }


    public class ObjectConverter : IObjectConverter
    {
        private readonly IObjectConverterMethodBuilder methodBuilder;

        public ObjectConverter(IObjectConverterMethodBuilder methodBuilder)
        {
            this.methodBuilder = methodBuilder;
        }

        public Dictionary<string, object> Convert(object obj)
        {
            return methodBuilder.CreateMethod(obj.GetType())(obj);
        }
    }
}