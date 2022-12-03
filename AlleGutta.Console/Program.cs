using System;
using AlleGutta.Nordnet;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, "../.env");
DotEnv.Load(dotenv);

var username = Environment.GetEnvironmentVariable("NORDNET_USERNAME");
var password = Environment.GetEnvironmentVariable("NORDNET_PASSWORD");

var nordnetProcessor = new NordnetProcessor(new("https://www.nordnet.no/login-next", username, password));
var data = await nordnetProcessor.GetBatchData();
Console.WriteLine(data);
