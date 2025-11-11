using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// 🧱 Thêm SQL Server container
var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);

var fastVocab = sql.AddDatabase("FastVocab");

// 🧊 Thêm Redis container
var redis = builder.AddRedis("redis")
    .WithImage("redis:7-alpine");

// 🔗 Kết nối app hiện có
var api = builder.AddProject<Projects.FastVocab_API>("api")
    .WithReference(fastVocab)  // inject SQL Server connection
    .WaitFor(sql);            //.WithReference(redis);  // inject Redis connection

var webApp = builder.AddProject<Projects.FastVocab_BlazorWebApp>("webApp")
    .WaitFor(api);

builder.Build().Run();
