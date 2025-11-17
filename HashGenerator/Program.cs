using System;

Console.Write("Enter password to hash: ");
string? password = Console.ReadLine();

if (!string.IsNullOrEmpty(password))
{
    string hash = BCrypt.Net.BCrypt.HashPassword(password, 12);
    
    Console.WriteLine($"\nPassword: {password}");
    Console.WriteLine($"BCrypt Hash: {hash}\n");
    
    Console.WriteLine("Copy this hash for your database:");
    Console.WriteLine(hash);
}
else
{
    Console.WriteLine("Password cannot be empty!");
}