using CD.UmbracoForms.DotdigitalWorkflow.Models;
using CD.UmbracoForms.DotdigitalWorkflow.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace CD.UmbracoForms.DotdigitalWorkflow.Workflow
{
    public class DotdigitalWorkflow : WorkflowType
    {
        private readonly IDotdigitalService _dotdigitalService;

        private FieldMapping _fieldMapping
        {
            get { return JsonConvert.DeserializeObject<FieldMapping>(FieldMapping); }
        }

        [Setting("Field mapping", Alias = "fieldMapping", Name = "Push to Dotdigital Address Book", Description = "Map Umbraco Forms fields to Dotdigital data fields", View = "~/App_Plugins/CD.DotdigitalFieldMapping/fieldmapping.html")]
        public string FieldMapping { get; set; }

        public DotdigitalWorkflow(IDotdigitalService dotdigitalService)
        {
            _dotdigitalService = dotdigitalService;

            this.Id = new Guid("68bdc512-f67c-480f-921c-3a7a1b35c512");
            this.Name = "Push to Dotdigital Address Book";
            this.Description = "Map Umbraco Forms fields to Dotdigital data fields";
            this.Icon = "icon-mindmap";
        }

        public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e)
        {
            var emailAddressFormFieldAlias = _fieldMapping.FieldMappings.FirstOrDefault(m => m.DataFieldId == "email").FormFieldAlias;
            var emailAddressValue = record.GetValue<string>(emailAddressFormFieldAlias);

            var dataFields =
                _fieldMapping.FieldMappings
                    .Where(m => m.DataFieldId != "email")
                    .Select(m => {
                        if (m.IsStatic)
                        {
                            return new KeyValuePair<string, string>(m.DataFieldId, m.StaticValue);
                        }
                        else
                        {
                            return new KeyValuePair<string, string>(m.DataFieldId, record.GetValue<string>(m.FormFieldAlias));
                        }
                    })
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var success = 
                _dotdigitalService.AddToAddressBook(_fieldMapping.AddressBookId, emailAddressValue, dataFields);

            return success ? WorkflowExecutionStatus.Completed : WorkflowExecutionStatus.Failed;
        }

        public override List<Exception> ValidateSettings()
        {
            var result = new List<Exception>();

            if (_fieldMapping == null || string.IsNullOrWhiteSpace(_fieldMapping.AddressBookId))
            {
                result.Add(new Exception("Must choose an Address Book"));
                return result;
            }

            if (!_fieldMapping.FieldMappings.Any())
                result.Add(new Exception("Must add field mappings"));

            if(!_fieldMapping.FieldMappings.Any(m => m.DataFieldId.ToLower() == "email"))
                result.Add(new Exception("Must add a mapping for Email"));

            if (_fieldMapping.FieldMappings.Any(m => m.IsStatic && string.IsNullOrWhiteSpace(m.StaticValue)))
                result.Add(new Exception("Static mappings must have a value"));

            return result;
        }
    }
}