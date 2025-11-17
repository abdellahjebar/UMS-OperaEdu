#!/usr/bin/env dotnet-script
#r "nuget: BCrypt.Net-Next, 4.0.3"

using BCrypt.Net;

// Generate hashes for seeded users
var passwords = new Dictionary<string, string>
{
    { "Admin@123", "Admin password" },
    { "Faculty@123", "Faculty password" },
    { "Student@123", "Student password" }
};

Console.WriteLine("Generated BCrypt Password Hashes:");
Console.WriteLine("=================================\n");

foreach (var (password, description) in passwords)
{
    var hash = BCrypt.Net.BCrypt.HashPassword(password);
    Console.WriteLine($"{description}:");
    Console.WriteLine($"Password: {password}");
    Console.WriteLine($"Hash: {hash}");
    Console.WriteLine();
}
