using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

//#if Nếu_dùng_docker

// 🧱 Thêm SQL Server container
var sql = builder.AddSqlServer("sql").WithDataVolume()
                 .WithLifetime(ContainerLifetime.Persistent);

var fastVocab = sql.AddDatabase("FastVocab");

// 🧊 Thêm Redis container
var redis = builder.AddRedis("redis");

//#endif
// 🔗 Kết nối app hiện có
var api = builder.AddProject<Projects.FastVocab_API>("api")
    .WithReference(fastVocab);  // inject SQL Server connection
    //.WaitFor(sql);            //.WithReference(redis);  // inject Redis connection


var webApp = builder.AddProject<Projects.FastVocab_BlazorWebApp>("webApp")
    .WaitFor(api);

builder.Build().Run();
