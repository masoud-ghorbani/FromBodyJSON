using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace System.Web.Http
{
    /// <summary>
    ///     An attribute that specifies that an action parameter comes only from the entity (JSON)
    ///     body of the incoming System.Net.Http.HttpRequestMessage.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public sealed class FromBodyJSONAttribute : ParameterBindingAttribute
    {
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            return new FromBodyJSONBinding(parameter);
        }
    }

    public class FromBodyJSONBinding : HttpParameterBinding
    {
        public FromBodyJSONBinding(HttpParameterDescriptor parameter)
        : base(parameter)
        {

        }

        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            try
            {
                var data = JObject.Parse(actionContext.Request.Content.ReadAsStringAsync().Result);
                if (data.ContainsKey(this.Descriptor.ParameterName))
                {
                    Type objectType = Type.GetType(this.Descriptor.ParameterType.FullName);
                    actionContext.ActionArguments[this.Descriptor.ParameterName] = data[this.Descriptor.ParameterName].ToObject(objectType);
                }
            }
            catch
            {
            }

            var taskSource = new TaskCompletionSource<object>();
            taskSource.SetResult(null);
            return taskSource.Task;
        }
    }
}