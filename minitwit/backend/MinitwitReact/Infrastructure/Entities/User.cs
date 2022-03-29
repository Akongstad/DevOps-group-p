﻿namespace MinitwitReact.Entities
{
    public class User
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        
        [EmailAddress]
        public string Email { get; init; } = null!;
        public string PwHash { get; init; } = null!;
    }
}