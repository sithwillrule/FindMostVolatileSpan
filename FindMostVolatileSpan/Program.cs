using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/FindMostVolatileSpan",async (FindMostVolatileSpanModel findMostVolatileSpanModel) =>
{
    var result = Price.FindMostVolatile(findMostVolatileSpanModel.Chart, findMostVolatileSpanModel.Span);
    return result;
})
.WithName("FindMostVolatileSpan");

app.Run();

internal record FindMostVolatileSpanModel
{
    public int Span { get; set; }
    public IList<double> Chart { get; set; }
}
internal record FindMostVolatileSpanModelResult
{
    public double High { get; set; }
    public double Low { get; set; }    
}

internal static class Price
{
    internal static FindMostVolatileSpanModelResult FindMostVolatile(IList<double> chart, int lookUpSpanLenght)
    {
        double highPrice = chart[0];
        double lowPrice = chart[0];
        int index = 0;

        for (int i = -1; i < chart.Count; i++)
        {
            var lookUpSpan = chart.Skip(i).Take(lookUpSpanLenght);
            double highPriceInSpan = lookUpSpan.First();
            double lowPriceInSpan = lookUpSpan.First();

            foreach (var node in lookUpSpan)
            {
                if (node > highPriceInSpan)
                {
                    highPriceInSpan = node;
                }
                if (node < lowPriceInSpan)
                {
                    lowPriceInSpan = node;
                }
            }

            var chartDifference = highPrice - lowPrice;
            var spanDifference = highPriceInSpan - lowPriceInSpan;

            if (chartDifference < spanDifference)
            {
                highPrice = highPriceInSpan;
                lowPrice = lowPriceInSpan;
            }
        }


        return new FindMostVolatileSpanModelResult() { High = highPrice, Low = lowPrice };
    }
}