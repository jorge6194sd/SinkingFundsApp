using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace SinkingFunds.Web.Pages
{
    public class EnvelopeUIModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }
        public Guid? CreatedEnvelopeId { get; set; }
        public decimal? CurrentBalance { get; set; }
        
        //Build out a subset of the envelope's properties, suitable for grid display
        public class EnvelopeGridRow
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public decimal Balance { get; set; }

            public decimal? TargetAmount {  get; set; }

            public decimal? RuleAmount { get; set; }
        }

        public List<EnvelopeGridRow> Envelopes { get; set; } = new List<EnvelopeGridRow>();

        private async Task LoadEnvelopes(HttpClient client)
        {
            Envelopes = await client.GetFromJsonAsync<List<EnvelopeGridRow>>("api/envelopes")
                ?? new List<EnvelopeGridRow>();
        }

        [BindProperty]
        public Guid EnvelopeId { get; set; }

        [BindProperty]
        public decimal Amount { get; set; }

        [BindProperty]
        public string Description { get; set; }

        public async Task OnPostCreate()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7005/");
            var content = new { Name = Name };
            var postContent = JsonContent.Create(content);
            var response = await client.PostAsync("api/envelopes", postContent);
            response.EnsureSuccessStatusCode();
            CreatedEnvelopeId = await response.Content.ReadFromJsonAsync<Guid>();
            await LoadEnvelopes(client);
        }

        public async Task OnPostDeposit()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7005/");
            var content = new { Amount = Amount, Description = Description };
            var postContent = JsonContent.Create(content);
            var response = await client.PostAsync($"api/envelopes/{EnvelopeId}/deposit", postContent);
            response.EnsureSuccessStatusCode();

            CurrentBalance = await client.GetFromJsonAsync<decimal>(
                $"api/envelopes/{EnvelopeId}/balance"
            );
            await LoadEnvelopes(client);
        }

        public async Task OnPostWithdraw()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7005/");
            var content = new { Amount = Amount, Description = Description };
            var postContent = JsonContent.Create(content);
            var response = await client.PostAsync($"api/envelopes/{EnvelopeId}/withdraw", postContent);
            response.EnsureSuccessStatusCode();

            CurrentBalance = await client.GetFromJsonAsync<decimal>(
                $"api/envelopes/{EnvelopeId}/balance"
            );
            await LoadEnvelopes(client);
        }


        public async Task OnPostDelete()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7005/");
            var response = await client.DeleteAsync($"api/envelopes/{EnvelopeId}");
            response.EnsureSuccessStatusCode();

            await LoadEnvelopes(client);
        }

        public async Task OnPostUpdateTargetAmount()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7005/");
            var content = new { TargetAmount = Amount};
            var postContent = JsonContent.Create(content);
            var response = await client.PutAsync($"api/envelopes/{EnvelopeId}/target", postContent);
            response.EnsureSuccessStatusCode();

            CurrentBalance = await client.GetFromJsonAsync<decimal>(
                $"api/envelopes/{EnvelopeId}/balance"
            );
            await LoadEnvelopes(client);
        }

        public async Task OnPostUpdateMonthlyAmount()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7005/");
            var content = new { Amount = Amount };
            var postContent = JsonContent.Create(content);
            var response = await client.PutAsync($"api/envelopes/{EnvelopeId}/monthly-contribution", postContent);
            response.EnsureSuccessStatusCode();

            CurrentBalance = await client.GetFromJsonAsync<decimal>(
                $"api/envelopes/{EnvelopeId}/balance"
            );
            await LoadEnvelopes(client);
        }

        public async Task OnGet()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7005/");

            await LoadEnvelopes(client);
        }
    }
}