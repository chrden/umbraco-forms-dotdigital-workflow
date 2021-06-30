using CD.UmbracoForms.DotdigitalWorkflow.Models.Dto;
using CD.UmbracoForms.DotdigitalWorkflow.Services;
using System.Collections.Generic;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace CD.UmbracoForms.DotdigitalWorkflow.Controllers.Api
{
    [PluginController("CD")]
    public class DotdigitalApiController : UmbracoAuthorizedJsonController
    {
        private readonly IDotdigitalService _dotdigitalService;

        public DotdigitalApiController(IDotdigitalService dotdigitalService)
        {
            _dotdigitalService = dotdigitalService;
        }

        public IEnumerable<AddressBookDto> GetAddressBooks() => _dotdigitalService.GetAddressBooks();

        public IEnumerable<DataFieldDto> GetDataFields() => _dotdigitalService.GetDataFields();
    }
}