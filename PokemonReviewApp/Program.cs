using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Repository;
using System;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//altdaki bitisik setrleri men yazmisam
//json ile bagli olan code setri ona gore yazilib ki, many to many relationship elaqesinde reviewer controller'indan
// 2.olandan xeta aldim, GetReviewer ucun olandan sohbet gedir
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();//repository'i islemesi ucun
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();//repository'i islemesi ucun
builder.Services.AddScoped<ICountryRepository, CountryRepository>();//repository'i islemesi ucun
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewerRepository, ReviewerRepository>();  
builder.Services.AddTransient<Seed>();  //seed class'ini elave etmek ucun
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());//automapper'i istifade etmek ucun bu setrden istifade olunur
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();


//asagidaki setr'de yazilib
if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);  //21.setr'de olan code'dan evvel yazilanda qirmizi verir

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedDataContext();
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
