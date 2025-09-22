using psa_legacy_sis;
using psa_legacy_sis.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/api/customers", async (psa_legacy_sis.Domain.Customer customer, psa_legacy_sis.Domain.Repositories.ICustomerRepository repo, CancellationToken ct) =>
{
    var created = await repo.AddAsync(customer, ct);
    return Results.Created($"/api/customers/{created.Id}", created);
});

app.Run();