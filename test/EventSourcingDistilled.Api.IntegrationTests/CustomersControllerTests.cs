using EventSourcingDistilled.Core.Models;
using EventSourcingDistilled.Domain.Features;
using EventSourcingDistilled.Testing;
using EventSourcingDistilled.Testing.Builders;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xunit;
using static EventSourcingDistilled.Api.IntegrationTests.CustomersControllerTests.Endpoints;

namespace EventSourcingDistilled.Api.IntegrationTests
{
    public class CustomersControllerTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;
        public CustomersControllerTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async System.Threading.Tasks.Task Should_CreateCustomer()
        {
            var context = _fixture.Context;

            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(new { 
                Customer = new CustomerDto
                {
                    Email = "quinntynebrown@gmail.com",
                    PhoneNumber = "555-967-1111",
                    Firstname = "Quinntyne",
                    Lastname = "Brown"
                }
            }), Encoding.UTF8, "application/json");

            using var client = _fixture.CreateClient();

            var httpResponseMessage = await client.PostAsync(Endpoints.Post.CreateCustomer, stringContent);

            var response = JsonConvert.DeserializeObject<CreateCustomer.Response>(await httpResponseMessage.Content.ReadAsStringAsync());

            var sut = context.Customers.First();

            Assert.NotEqual(default, response.Customer.CustomerId);
        }



        [Fact]
        public async System.Threading.Tasks.Task Should_RemoveCustomer()
        {
            var customer = CustomerBuilder.WithDefaults();

            var context = _fixture.Context;

            var client = _fixture.CreateClient();

            await context.SaveChangesAsync(default);

            var httpResponseMessage = await client.DeleteAsync(Delete.By(customer.CustomerId));

            httpResponseMessage.EnsureSuccessStatusCode();

            var removedCustomer = await context.LoadAsync<Customer>(customer.CustomerId);

            Assert.NotEqual(default, removedCustomer.Deleted);
        }

        [Fact]
        public async System.Threading.Tasks.Task Should_UpdateCustomer()
        {
            var customer = CustomerBuilder.WithDefaults();

            var context = _fixture.Context;

            await context.SaveChangesAsync(default);

            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(new { customer = customer.ToDto() }), Encoding.UTF8, "application/json");

            var httpResponseMessage = await _fixture.CreateClient().PutAsync(Put.Update, stringContent);

            httpResponseMessage.EnsureSuccessStatusCode();

            var sut = await context.LoadAsync<Customer>(customer.CustomerId);

        }

        [Fact]
        public async System.Threading.Tasks.Task Should_GetCustomers()
        {
            var customer = CustomerBuilder.WithDefaults();

            var context = _fixture.Context;

            await context.SaveChangesAsync(default);

            var httpResponseMessage = await _fixture.CreateClient().GetAsync(Get.Customers);

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = JsonConvert.DeserializeObject<GetCustomers.Response>(await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.True(response.Customers.Any());
        }

        [Fact]
        public async System.Threading.Tasks.Task Should_GetCustomerById()
        {
            var customer = CustomerBuilder.WithDefaults();

            var context = _fixture.Context;


            await context.SaveChangesAsync(default);

            var httpResponseMessage = await _fixture.CreateClient().GetAsync(Get.By(customer.CustomerId));

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = JsonConvert.DeserializeObject<GetCustomerById.Response>(await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.NotNull(response);

        }

        internal static class Endpoints
        {
            public static class Post
            {
                public static string CreateCustomer = "api/customers";
            }

            public static class Put
            {
                public static string Update = "api/customers";
            }

            public static class Delete
            {
                public static string By(Guid customerId)
                {
                    return $"api/customers/{customerId}";
                }
            }

            public static class Get
            {
                public static string Customers = "api/customers";
                public static string By(Guid customerId)
                {
                    return $"api/customers/{customerId}";
                }
            }
        }
    }
}
