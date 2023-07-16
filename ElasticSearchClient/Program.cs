// See https://aka.ms/new-console-template for more information
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Reflection;

Console.WriteLine("Hello, World!");

var builder = new ConfigurationBuilder();


builder.AddEnvironmentVariables();
builder.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
IConfiguration configuration = builder.Build();

var client = new ElasticsearchClient(
    configuration["Elastic:CloudId"],
    new ApiKey(configuration["Elastic:ApiKey"]));
var response1 = await client.Indices.CreateAsync("my_index");

var doc1 = new MyDoc
{
    Id = 1,
    User = "flobernd",
    Message = "Trying out the client, so far so good?"
};

var response2 = await client.IndexAsync(doc1, "my_index");

var response3 = await client.GetAsync<MyDoc>(response2.Id, idx => idx.Index("my_index"));

if (response3.IsValidResponse)
{
    var doc2 = response3.Source;
}

var response4 = await client.SearchAsync<MyDoc>(s => s
    .Index("my_index")
    .From(0)
    .Size(10)
    .Query(q => q
        .Term(t => t.User, "flobernd")
    )
);

if (response4.IsValidResponse)
{
    var doc3 = response4.Documents.FirstOrDefault();
}

doc1.Message = "This is a new message";

var response5 = await client.UpdateAsync<MyDoc, MyDoc>("my_index", 1, u => u
    .Doc(doc1));

var response6 = await client.DeleteAsync("my_index", 1);

var response7 = await client.Indices.DeleteAsync("my_index");

internal class MyDoc
{
    public int Id { get; set; }
    public string User { get; set; }
    public string Message { get; set; }
}