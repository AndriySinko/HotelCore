// This file contains code for AppHost.
var builder = DistributedApplication.CreateBuilder(args);

















const bool useLocalDatabase = true;

var db = useLocalDatabase
    ? builder.AddPostgres("postgres")
        .WithPgAdmin()
        .AddDatabase("DefaultConnection")
    : builder.AddConnectionString("DefaultConnection");

var redis = builder.AddRedis("redis");
var seq = builder.AddSeq("seq");

var api = builder.AddProject<Projects.HotelCore_Api>("hotelCore-api")
    .WithReference(db)
    .WithReference(seq)
    .WithReference(redis)
    .WaitFor(seq)
    .WaitFor(redis);


if (useLocalDatabase)
{
    api.WaitFor(db);
}









builder.Build().Run();
