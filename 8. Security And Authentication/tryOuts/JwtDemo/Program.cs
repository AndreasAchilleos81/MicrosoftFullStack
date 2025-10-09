using JwtDemo;

var creator = new JwtCreator();
var decoder = new JwtDecoder();

var token = creator.CreateToken("123", "testuser");
Console.WriteLine($"Generated Token: {token}");

var principal = decoder.DecodeToken(token);
Console.WriteLine(principal != null ? "Token    is valid." : "Token is invalid.");
