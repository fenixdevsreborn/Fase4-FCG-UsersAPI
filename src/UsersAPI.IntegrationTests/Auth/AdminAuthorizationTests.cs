using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using UsersAPI.IntegrationTests.Fixtures;
using UsersAPI.IntegrationTests.Helpers;

namespace UsersAPI.IntegrationTests.Auth
{
    public sealed class AdminAuthorizationTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AdminAuthorizationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Admin_endpoint_should_return_403_for_non_admin_user()
        {
            var token = await AuthHelper.AuthenticateAsUserAsync(_client);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/admin/test");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Admin_endpoint_should_allow_admin_user()
        {
            var token = await AuthHelper.AuthenticateAsAdminAsync(_client);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/admin/test");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }
}
