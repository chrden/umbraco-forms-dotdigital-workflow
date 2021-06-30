using CD.UmbracoForms.DotdigitalWorkflow.Models.Api.Request;
using CD.UmbracoForms.DotdigitalWorkflow.Models.Api.Response;
using CD.UmbracoForms.DotdigitalWorkflow.Models.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Umbraco.Core.Logging;

namespace CD.UmbracoForms.DotdigitalWorkflow.Services
{
    public interface IDotdigitalService
    {
        IEnumerable<Models.Dto.AddressBookDto> GetAddressBooks();
        IEnumerable<Models.Dto.DataFieldDto> GetDataFields();
        bool AddToAddressBook(string addressBookId, string emailAddress, Dictionary<string, string> dataFields);
    }

    public class DotdigitalService : IDotdigitalService
    {
        private readonly ILogger logger;

        public DotdigitalService(ILogger logger)
        {
            this.logger = logger;
        }

        #region GET

        public IEnumerable<AddressBookDto> GetAddressBooks()
        {
            try
            {
                using (var client = GetClient())
                {
                    var endpoint = GenerateEndpoint("/address-books");

                    var response = client.GetStringAsync(endpoint).Result;

                    var addressBooksResponse = JsonConvert.DeserializeObject<IEnumerable<AddressBook>>(response);

                    if (addressBooksResponse != null && addressBooksResponse.Any())
                    {
                        return addressBooksResponse
                            .Select(l =>
                                new AddressBookDto
                                {
                                    Id = l.Id.ToString(),
                                    Name = l.Name
                                }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error<DotdigitalService>(ex);
            }

            return null;
        }

        public IEnumerable<DataFieldDto> GetDataFields()
        {
            try
            {
                using (var client = GetClient())
                {
                    var endpoint = GenerateEndpoint("/data-fields");
                    var response = client.GetStringAsync(endpoint).Result;
                    var dataFieldsResponse = JsonConvert.DeserializeObject<IEnumerable<DataField>>(response);

                    if (dataFieldsResponse != null && dataFieldsResponse.Any())
                    {
                        var dataFields =
                            dataFieldsResponse
                                .Select(df =>
                                    new DataFieldDto
                                    {
                                        Id = df.Name,
                                        Name = df.Name,
                                    }
                                )
                                .ToList();

                        dataFields.Add(new DataFieldDto { Id = "email", Name = "EMAIL" });

                        return dataFields.OrderBy(df => df.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error<DotdigitalService>(ex);
            }

            return null;
        }

        #endregion

        #region POST

        public bool AddToAddressBook(string addressBookId, string emailAddress, Dictionary<string, string> dataFields)
        {
            try
            {
                using (var client = GetClient())
                {
                    var endpoint = GenerateEndpoint("/address-books/", addressBookId, "/contacts");

                    var contact =
                        new AddContactToAddressBook
                        {
                            Email = emailAddress,
                            DataFields =
                                dataFields.Select(df =>
                                    new
                                    {
                                        Key = df.Key,
                                        Value = df.Value
                                    }
                                )
                        };

                    var contactJson = JsonConvert.SerializeObject(contact);
                    
                    var response = client.PostAsync(endpoint, new StringContent(contactJson, Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        var errorResponseString = response.Content.ReadAsStringAsync();
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(errorResponseString.Result);
                        
                        logger.Error<DotdigitalService>("Dotdigital Workflow error: {ErrorMessage}). Address Book Id: {AddressBookId}", error.Message, addressBookId);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error<DotdigitalService>(ex);
            }

            return false;
        }

        #endregion

        private HttpClient GetClient()
        {
            var client = new HttpClient();
            var apiUsername = ConfigurationManager.AppSettings.Get("Dotdigital.Api.Username");
            var apiPassword = ConfigurationManager.AppSettings.Get("Dotdigital.Api.Password");
            var credentials = Encoding.ASCII.GetBytes(string.Concat(apiUsername, ":", apiPassword));

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Dotdigital.Api.BaseAddress"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));

            return client;
        }

        public string GenerateEndpoint(params string[] enpointSegments)
        {
            var segments = new List<string>() { "/" + ConfigurationManager.AppSettings.Get("Dotdigital.Api.Version") };
            segments.AddRange(enpointSegments);

            return string.Concat(segments);
        }
    }
}