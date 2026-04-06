var builder = DistributedApplication.CreateBuilder(args);

// ============================================
// Database Configuration - Easy Switching
// ============================================
// Option 1: Local PostgreSQL (useLocalDatabase = true)
//   - Spins up PostgreSQL container automatically
//   - Includes pgAdmin for database management
//   - No external database needed
//   - Perfect for local development
//
// Option 2: External Database (useLocalDatabase = false)
//   - Uses connection string from appsettings.json or user secrets
//   - Connect to existing PostgreSQL instance
//   - Configure "DefaultConnection" in secrets.json
//
// To switch: Just change the boolean below
// ============================================
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

// Wait for database container if using local PostgreSQL
if (useLocalDatabase)
{
    api.WaitFor(db);
}

/*var frontend = builder.AddNpmApp("frontend", "../frontend", "dev")
    .WithReference(api)
    .WithEnvironment("VITE_API_URL", $"{api.GetEndpoint("https")}/api/v1")
    .WithHttpEndpoint(env: "PORT")
    .WaitFor(api);

// Pass the frontend URL to the API so it can redirect back after OAuth
api.WithEnvironment("FrontendUrl", frontend.GetEndpoint("http"));*/

builder.Build().Run();
