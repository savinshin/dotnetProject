using Aspire.Hosting;
using System.Runtime.InteropServices;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("postgresdb");

var api  = builder.AddProject<Projects.WebApplication1>("api")
                            .WithReference(postgresdb)
                            .WithEnvironment("ApplyMigrationsOnStartup", "true");

builder.AddNpmApp("web", "../reactproject1/reactproject1", "dev")
           .WithReference(api)
           .WaitFor(api)
           .WithEnvironment("BROWSER", "none")
           .WithHttpEndpoint(port: 5173, env: "PORT")
           .WithExternalHttpEndpoints()
           .PublishAsDockerFile();

builder.Build().Run();
