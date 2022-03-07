﻿namespace MASA.Auth.Service.Application.Subjects;

public class StaffItem
{
    public Guid Id { get; set; }

    public string Avatar { get; set; } = "";

    public string Name { get; set; } = "";

    public string JobNumber { get; set; } = "";

    public string Position { get; set; } = "";

    public bool Enabled { get; set; }
}
