using MudBlazor;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Client.Infrastructure.Configuration;
using Client.Infrastructure.ApiClientManagers;
using Contracts = Client.Infrastructure.Contracts;
using Client.Infrastructure.Security.Configurations;
using Client.Infrastructure.Security.AuthHandlers.AzureAD;

namespace MudBlazorClient.Pages.Features
{
    [Authorize]
    public partial class User
    {
        private bool _loaded;
        private string _searchString = "";
        private Contracts.User _user = new();
        private List<Contracts.User>? _userList = new();

        [Inject] private ITokenAcquisition TokenAcquisition { get; set; }
        [Inject] private IOptions<AuthConfig> AuthConfig { get; set; }
        [Inject] private IOptions<ApiClientConfig> ApiClientConfig { get; set; }

        private UserApiClientManager UserApiClientManager;

        private AzureAdAuthHandler AuthHandler() => new AzureAdAuthHandler(AuthConfig?.Value, TokenAcquisition);
        
        protected override async Task OnInitializedAsync()
        {
            UserApiClientManager = new UserApiClientManager(ApiClientConfig.Value, AuthConfig.Value.IsAuthRequired ? AuthHandler() : null);


            await GetUserAsync();
            _loaded = true;
        }

        private async Task GetUserAsync()
        {
            var response = await UserApiClientManager.GetAllAsync();
            if (response?.Status == StatusCodes.Status200OK)
            {
                _userList = response.Data.Data?.ToList();
            }
            else
            {
                _snackBar.Add(response?.ServerError.ToString(), Severity.Error);
            }
        }



        private bool Search(Contracts.User user)
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;
            var propertiesToSearch = new[]
            {
                user.FirstName,
                user.LastName,
                user.UserId,
                user.Mobile.ToString(),
                user.Address,
                user.Status,
                user.Gender
            };

            return propertiesToSearch.Any(property => property?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
        }
    }
}