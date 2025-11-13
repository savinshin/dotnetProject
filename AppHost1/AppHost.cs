using Aspire.Hosting;
using System.Runtime.InteropServices;

var builder = DistributedApplication.CreateBuilder(args);

var pgPassword = builder.AddParameter("postgres-password", secret: true);

var postgres = builder.AddPostgres("postgres")
    .WithPassword(pgPassword)
    .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("postgresdb");

var api  = builder.AddProject<Projects.WebApplication1>("api")
                            .WithReference(postgresdb)
                            .WaitFor(postgresdb)
                            .WithEnvironment("ApplyMigrationsOnStartup", "true");

builder.AddNpmApp("web", "../reactproject1/reactproject1", "dev")
           .WithReference(api)
           .WaitFor(api)
           .WithEnvironment("BROWSER", "none")
           .WithHttpEndpoint(port: 5173, env: "PORT")
           .WithExternalHttpEndpoints()
           .PublishAsDockerFile();

builder.Build().Run();
