using Newtonsoft.Json;
using System.Collections.Generic;

namespace CD.UmbracoForms.DotdigitalWorkflow.Models.Api.Request
{
    public class AddContactToAddressBook
    {
        public string Email { get; set; }
        public string OptInType { get; set; }
        public string EmailType { get; set; }
        public IEnumerable<object> DataFields { get; set; }
        public string Status { get; set; }
    }
}