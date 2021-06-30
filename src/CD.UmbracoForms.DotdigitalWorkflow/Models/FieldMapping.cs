using System.Collections.Generic;

namespace CD.UmbracoForms.DotdigitalWorkflow.Models
{
    internal class FieldMapping
    {
        public string AddressBookId { get; set; }
        public IEnumerable<FieldMap> FieldMappings { get; set; }

        public class FieldMap
        {
            public string FormFieldAlias { get; set; }
            public string StaticValue { get; set; }
            public bool IsStatic { get; set; }
            public string DataFieldId { get; set; }
        }
    }
}