using CD.UmbracoForms.DotdigitalWorkflow.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace CD.UmbracoForms.DotdigitalWorkflow.Composers
{
    public class IoC : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IDotdigitalService, DotdigitalService>();
        }
    }
}