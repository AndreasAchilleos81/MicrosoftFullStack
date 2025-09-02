using EfCore3;
using HrDbContext.DatabaseContext;

Config config = new Config();
var connectionString = config.ConnectionString;

var hrDbContext = new HrDatabase(config);
#if DEBUG
hrDbContext.SeedIfNecessary(false);
#endif