﻿using System;
using System.Collections.Generic;

namespace MinitwitReact.Entities
{
    public partial class User
    {
        public long UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PwHash { get; set; } = null!;
    }
}