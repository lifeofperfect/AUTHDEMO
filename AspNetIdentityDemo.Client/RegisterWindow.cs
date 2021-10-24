using AspNetIdentityDemo.Api.Models.Request.Dto;
using AspNetIdentityDemo.Api.Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AspNetIdentityDemo.Client
{
    public class RegisterWindow
    {
        public RegisterWindow()
        {

        }

        private async void btnRegister()
        {
            HttpClient client = new HttpClient();

            var model = new RegisterViewModel
            {

            };

            var jsonData = JsonConvert.SerializeObject(model);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:44392/api/auth/register", content);

            var responseBody = await response.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<UserManagerResponse>(responseBody);

            if (responseObject.IsSuccess)
            {
                var res = "success";
            }
        }
    }
}
