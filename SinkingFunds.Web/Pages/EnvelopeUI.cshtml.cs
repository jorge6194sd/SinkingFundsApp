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


        // Build out a subset of the envelope's properties,
        // suitable for grid/display purposes
        public class EnvelopeGridRow
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public decimal Balance { get; set; }

            public decimal? TargetAmount { get; set; }

            public decimal? RuleAmount { get; set; }

            public string MonthsRemaining { get; set; }
        }


        public List<EnvelopeGridRow> Envelopes { get; set; }
            = new List<EnvelopeGridRow>();


        private async Task LoadEnvelopes(HttpClient client)
        {
            Envelopes =
                await client.GetFromJsonAsync<List<EnvelopeGridRow>>(
                    "api/envelopes"
                )
                ?? new List<EnvelopeGridRow>();
        }


        [BindProperty]
        public decimal Amount { get; set; }

        [BindProperty]
        public string Description { get; set; }


        public async Task<IActionResult> OnPostCreate()
        {
            using HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7005/");

            var content = new
            {
                Name = Name
            };

            using var response = await client.PostAsync(
                "api/envelopes",
                JsonContent.Create(content)
            );

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var validation = await response.Content
                    .ReadFromJsonAsync<ValidationProblemDetails>();

                if (validation != null && validation.Errors.Count > 0)
                {
                    foreach (var field in validation.Errors)
                    {
                        foreach (var message in field.Value)
                        {
                            ModelState.AddModelError(field.Key, message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Please check your envelope name.");
                }

                await LoadEnvelopes(client);
                return Page();
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the envelope. Please try again.");

                await LoadEnvelopes(client);
                return Page();
            }

            CreatedEnvelopeId =
                await response.Content.ReadFromJsonAsync<Guid>();

            await LoadEnvelopes(client);
            return Page();
        }


        public async Task<IActionResult> OnPostDeposit(Guid envelopeId)
        {
            using HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7005/");

            var content = new
            {
                Amount = Amount,
                Description = Description
            };

            var response = await client.PostAsync(
                $"api/envelopes/{envelopeId}/deposit",
                JsonContent.Create(content)
            );

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var validation =
                    await response.Content
                        .ReadFromJsonAsync<ValidationProblemDetails>();

                if (validation != null && validation.Errors.Count > 0)
                {
                    foreach (var field in validation.Errors)
                    {
                        foreach (var message in field.Value)
                        {
                            ModelState.AddModelError(field.Key, message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Please check your deposit details.");
                }

                // Page() won't load envelopes by itself so manually call
                await LoadEnvelopes(client);
                // Page() is the default for showing why deposit failed
                return Page();
            }

            // Unexpected API failures get a generic user-facing message.
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to complete the deposit. Please try again.");

                await LoadEnvelopes(client);
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostWithdraw(Guid envelopeId)
        {
            ModelState.Remove(nameof(Name));

            using HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7005/");

            // ===== ADDED: Stop here if Razor found invalid input =====
            if (!ModelState.IsValid)
            {
                await LoadEnvelopes(client);
                return Page();
            }
            // ===== END ADDITION =====

            var content = new
            {
                Amount = Amount,
                Description = Description
            };

            var response = await client.PostAsync(
                $"api/envelopes/{envelopeId}/withdraw",
                JsonContent.Create(content)
            );

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var validation = await response.Content
                    .ReadFromJsonAsync<ValidationProblemDetails>();

                if (validation != null && validation.Errors.Count > 0)
                {
                    foreach (var field in validation.Errors)
                    {
                        foreach (var message in field.Value)
                        {
                            ModelState.AddModelError(field.Key, message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Please check your withdrawal details.");
                }

                await LoadEnvelopes(client);
                return Page();
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to complete the withdrawal. Please try again.");

                await LoadEnvelopes(client);
                return Page();
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDelete(Guid envelopeId)
        {
            using HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7005/");

            using var response = await client.DeleteAsync(
                $"api/envelopes/{envelopeId}"
            );

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var validation = await response.Content
                    .ReadFromJsonAsync<ValidationProblemDetails>();

                if (validation != null && validation.Errors.Count > 0)
                {
                    foreach (var field in validation.Errors)
                    {
                        foreach (var message in field.Value)
                        {
                            ModelState.AddModelError(field.Key, message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Unable to delete this envelope. Please check your request.");
                }

                await LoadEnvelopes(client);
                return Page();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This envelope could not be found. It may already have been deleted.");

                await LoadEnvelopes(client);
                return Page();
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to delete the envelope. Please try again.");

                await LoadEnvelopes(client);
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateTargetAmount(Guid envelopeId)
        {
            // Only validate relevant form field entries
            ModelState.Remove(nameof(Name));
            ModelState.Remove(nameof(Description));

            using HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7005/");

            var content = new
            {
                TargetAmount = Amount
            };

            using var response = await client.PutAsync(
                $"api/envelopes/{envelopeId}/target",
                JsonContent.Create(content)
            );

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var validation = await response.Content
                    .ReadFromJsonAsync<ValidationProblemDetails>();

                if (validation != null && validation.Errors.Count > 0)
                {
                    foreach (var field in validation.Errors)
                    {
                        foreach (var message in field.Value)
                        {
                            ModelState.AddModelError(field.Key, message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Please check your target amount.");
                }

                await LoadEnvelopes(client);
                return Page();
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the target amount. Please try again.");

                await LoadEnvelopes(client);
                return Page();
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostUpdateMonthlyAmount(Guid envelopeId)
        {
            using HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7005/");

            var content = new
            {
                Amount = Amount
            };

            using var response = await client.PutAsync(
                $"api/envelopes/{envelopeId}/monthly-contribution",
                JsonContent.Create(content)
            );

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var validation = await response.Content
                    .ReadFromJsonAsync<ValidationProblemDetails>();

                if (validation != null && validation.Errors.Count > 0)
                {
                    foreach (var field in validation.Errors)
                    {
                        foreach (var message in field.Value)
                        {
                            ModelState.AddModelError(field.Key, message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Please check your monthly contribution.");
                }

                await LoadEnvelopes(client);
                return Page();
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the monthly contribution. Please try again.");

                await LoadEnvelopes(client);
                return Page();
            }

            return RedirectToPage();
        }


        public async Task OnGet()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress =
                new Uri("https://localhost:7005/");

            await LoadEnvelopes(client);
        }
    }
}